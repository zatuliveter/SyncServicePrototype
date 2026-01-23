using System.Collections.Concurrent;
using System.Diagnostics;

namespace TaskFactory;

public sealed class PipelineRunner(
		IServiceProvider services,
		IPipelineValidator validator
	)
	: IPipelineRunner
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
			nodes.Values.Where(n => n.RemainingDependencies.Count == 0)
		);

		HashSet<Task> runningTasks = new();
		SemaphoreSlim semaphore = new(parallelTaskCount);

		async Task RunNodeAsync(TaskNode node)
		{
			await semaphore.WaitAsync(cts.Token).ConfigureAwait(false);
			try
			{
				if (cts.IsCancellationRequested)
					return;

				node.Status = TaskExecutionStatus.Running;

				object? task = _services.GetService(node.Item.TaskType);

				if (task is null)
				{
					throw new Exception($"Cannot find {node.Item.TaskType} in dependencies.");
				}
				
				if (task is not ITask taskInstance)
				{
					throw new Exception($"Task type {node.Item.TaskType} must implement ITask.");
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
		}

		while (true)
		{
			// Schedule ready tasks
			while (readyQueue.TryDequeue(out TaskNode? node))
			{
				if (node.Status != TaskExecutionStatus.NotStarted)
					continue;

				if (cts.IsCancellationRequested)
					break;

				Task task = RunNodeAsync(node);

				lock (runningTasks)
				{
					runningTasks.Add(task);
				}				

				_ = task.ContinueWith(t =>
				{
					lock (runningTasks)
					{
						runningTasks.Remove(t);
					}

					OnNodeFinished(node, failureMode, nodes, readyQueue);
				}, TaskScheduler.Default);
			}

			if (runningTasks.Count == 0 && readyQueue.IsEmpty)
				break;

			await Task.Delay(10, ct).ConfigureAwait(false);
		}

		Debug.Assert(nodes.Values.All(n => n.Status != TaskExecutionStatus.Running));

		// Build result
		PipelineRunResult result = new()
		{
			Tasks = nodes.Values.ToDictionary(
				n => n.Item.Id,
				n => new TaskRunResult
				{
					Status = n.Status,
					Error = errors.TryGetValue(n.Item.Id, out Exception? ex) ? ex : null
				}
			)
		};

		result.IsSuccess = result.Tasks.Values.All(x => x.Status == TaskExecutionStatus.Success);

		return result;
	}

	private static void SkipDependentsRecursively(TaskNode node, Dictionary<string, TaskNode> nodes)
	{
		foreach (string depId in node.Dependents)
		{
			TaskNode dep = nodes[depId];
			if (dep.Status == TaskExecutionStatus.NotStarted)
			{
				dep.Status = TaskExecutionStatus.Skipped;
				SkipDependentsRecursively(dep, nodes);
			}
		}
	}

	private static void OnNodeFinished(
		TaskNode finished, 
		PipelineFailureMode failureMode, 
		Dictionary<string, TaskNode> nodes, 
		ConcurrentQueue<TaskNode> readyQueue
	)
	{
		if (finished.Status == TaskExecutionStatus.Failed &&
			failureMode == PipelineFailureMode.SkipDependentTasks)
		{
			SkipDependentsRecursively(finished, nodes);
		}

		foreach (string depId in finished.Dependents)
		{
			TaskNode depNode = nodes[depId];
			depNode.RemainingDependencies.Remove(finished.Item.Id);

			if (depNode.RemainingDependencies.Count == 0 &&
				depNode.Status == TaskExecutionStatus.NotStarted)
			{
				readyQueue.Enqueue(depNode);
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
				RemainingDependencies = new HashSet<string>(x.DependsOn, StringComparer.OrdinalIgnoreCase),
				Dependents = [],
				Status = TaskExecutionStatus.NotStarted
			},
			StringComparer.OrdinalIgnoreCase
		);

		foreach (TaskNode? node in nodes.Values)
		{
			foreach (string dep in node.Item.DependsOn)
			{
				nodes[dep].Dependents.Add(node.Item.Id);
			}
		}

		return nodes;
	}
}
