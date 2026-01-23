using System.Collections.Concurrent;
using System.Diagnostics;

namespace TaskFactory;

public sealed class PipelineRunner(
	IServiceProvider services,
	IPipelineValidator validator
) : IPipelineRunner
{
	private readonly IServiceProvider _services = services;
	private readonly IPipelineValidator _validator = validator;

	public async Task<PipelineRunResult> RunAsync(
		IPipeline pipeline,
		int parallelTaskCount,
		PipelineFailureMode failureMode,
		CancellationToken ct
	)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parallelTaskCount);

		_validator.Validate(pipeline);

		PipelineContext context = new();
		using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
		context.PipelineCancellation = cts.Token;

		Dictionary<string, TaskNode> nodes = BuildGraph(pipeline);

		ConcurrentDictionary<string, Exception> errors = new(StringComparer.OrdinalIgnoreCase);

		ConcurrentQueue<TaskNode> readyQueue = new(
			nodes.Values.Where(n => n.RemainingDependenciesCount == 0)
		);

		List<Task<TaskNode>> running = [];
		SemaphoreSlim semaphore = new(parallelTaskCount);

		async Task<TaskNode> RunNodeAsync(TaskNode node)
		{
			await semaphore.WaitAsync(cts.Token).ConfigureAwait(false);
			try
			{
				if (cts.IsCancellationRequested)
					return node;

				node.Status = TaskExecutionStatus.Running;

				object? task = _services.GetService(node.Item.TaskType);

				if (task is null)
				{
					throw new InvalidOperationException($"Cannot find {node.Item.TaskType} in DI container.");
				}

				if (task is not ITask taskInstance)
				{
					throw new InvalidOperationException($"Task type {node.Item.TaskType} must implement ITask.");
				}

				await taskInstance.ProcessAsync(
					node.Item.Parameters,
					node.Item.Id,
					context,
					cts.Token
				).ConfigureAwait(false);

				node.Status = TaskExecutionStatus.Success;
			}
			catch (Exception ex)
			{
				node.Status = TaskExecutionStatus.Failed;
				errors[node.Item.Id] = ex;

				if (failureMode == PipelineFailureMode.FailPipeline)
				{
					cts.Cancel();
				}
			}
			finally
			{
				semaphore.Release();
			}

			return node;
		}

		EnqueueReady();

		while (running.Count > 0)
		{
			Task<TaskNode> finishedTask = await Task.WhenAny(running).ConfigureAwait(false);
			running.Remove(finishedTask);

			TaskNode finishedNode = await finishedTask.ConfigureAwait(false);

			OnNodeFinished(finishedNode);

			EnqueueReady();
		}

		Debug.Assert(nodes.Values.All(n => n.Status != TaskExecutionStatus.Running));

		PipelineRunResult result = new()
		{
			Tasks = nodes.Values.ToDictionary(
				n => n.Item.Id,
				n => new TaskRunResult
				{
					Status = n.Status,
					Error = errors.TryGetValue(n.Item.Id, out Exception? ex) ? ex : null
				},
				StringComparer.OrdinalIgnoreCase
			)
		};

		result.IsSuccess = result.Tasks.Values.All(x => x.Status == TaskExecutionStatus.Success);

		return result;

		// ---------------- local helpers ----------------

		void EnqueueReady()
		{
			while (readyQueue.TryDequeue(out TaskNode? node))
			{
				if (node.Status != TaskExecutionStatus.NotStarted)
					continue;

				if (cts.IsCancellationRequested)
					return;

				node.Status = TaskExecutionStatus.Running;

				var task = RunNodeAsync(node);
				running.Add(task);
			}
		}

		void OnNodeFinished(TaskNode finished)
		{
			if (finished.Status == TaskExecutionStatus.Failed &&
				failureMode == PipelineFailureMode.SkipDependentTasks)
			{
				SkipDependentsRecursively(finished);
				return;
			}

			foreach (string depId in finished.Dependents)
			{
				TaskNode depNode = nodes[depId];

				if (depNode.Status != TaskExecutionStatus.NotStarted)
					continue;

				int newCount = Interlocked.Decrement(ref depNode.RemainingDependenciesCount);

				if (newCount == 0)
				{
					readyQueue.Enqueue(depNode);
				}
			}
		}

		void SkipDependentsRecursively(TaskNode node)
		{
			foreach (string depId in node.Dependents)
			{
				TaskNode dep = nodes[depId];

				if (dep.Status == TaskExecutionStatus.NotStarted)
				{
					dep.Status = TaskExecutionStatus.Skipped;
					SkipDependentsRecursively(dep);
				}
			}
		}
	}

	private static Dictionary<string, TaskNode> BuildGraph(IPipeline pipeline)
	{
		Dictionary<string, TaskNode> nodes = pipeline.Items.ToDictionary(
			x => x.Id,
			x => new TaskNode
			{
				Item = x,
				Dependents = [],
				RemainingDependenciesCount = x.DependsOn.Count,
				Status = TaskExecutionStatus.NotStarted
			},
			StringComparer.OrdinalIgnoreCase
		);

		foreach (TaskNode node in nodes.Values)
		{
			foreach (string dep in node.Item.DependsOn)
			{
				nodes[dep].Dependents.Add(node.Item.Id);
			}
		}

		return nodes;
	}
}
