//namespace TaskFactory;

//public interface ITask
//{
//	string Id { get; }

//	IReadOnlyCollection<string> DependsOn { get; }

//	Task ProcessAsync(IPipelineContext pipelineContext, CancellationToken ct);
//}


//public interface IPipelineContext
//{
//	Guid RunId { get; }

//	DateTimeOffset StartTime { get; }
//}

//public sealed class MyCustomTask(
//	ILogger logger,
//	IConnectionManager connections
//) : ITask
//{
//	public string Id => "custom_task_1";

//	public IReadOnlyCollection<string> DependsOn => [];

//	public async Task ProcessAsync(IPipelineContext pipelineContext, CancellationToken ct)
//	{
//		logger.Log("Starting task {TaskId}, run {RunId}", Id, pipelineContext.RunId);

//		var connection = await connections.GetAsync<IConnection>("source-db", ct);

//		// do work here

//		logger.Log("Finished task {TaskId}", Id);
//	}
//}

//public interface ILogger
//{
//	void Log(string message, params object[] args);

//	void LogError(Exception ex, string message, params object[] args);
//}

//public interface IConnectionManager
//{
//	ValueTask<T> GetAsync<T>(string connectionName, CancellationToken ct)
//		where T : class;
//}

//public interface IConnection
//{
//}

//public interface IPipeline
//{
//	string Name { get; }

//	IReadOnlyCollection<ITask> Tasks { get; }
//}


//public enum PipelineFailureMode
//{
//	FailPipeline,
//	SkipDependentTasks
//}

//public interface IPipelineRunner
//{
//	Task<PipelineRunResult> RunAsync(
//		IPipeline pipeline,
//		int parallelTaskCount,
//		PipelineFailureMode failureMode,
//		CancellationToken ct
//	);
//}

//public sealed class PipelineRunResult
//{
//	public bool IsSuccess { get; init; }

//	// key = TaskId
//	public IReadOnlyDictionary<string, TaskRunResult> Tasks { get; init; } = new Dictionary<string, TaskRunResult>();
//}

//public sealed class TaskRunResult
//{
//	public TaskExecutionStatus Status { get; init; }

//	public TimeSpan Duration { get; set; }

//	public Exception? Error { get; init; }
//}

//public enum TaskExecutionStatus
//{
//	NotStarted,
//	Success,
//	Failed,
//	Skipped
//}

//public interface IPipelineValidator
//{
//	void Validate(IPipeline pipeline);
//}
