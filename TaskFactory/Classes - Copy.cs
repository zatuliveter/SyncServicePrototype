//namespace TaskFactory;


//public interface ITask
//{
//	string Id { get; }

//	IReadOnlyCollection<string> DependsOn { get; }

//	Task<TaskResult> ProcessAsync(IPipelineContext pipelineContext, CancellationToken ct);
//}

//public class MyCustomTask(
//		ILogger _logger
//	  , IConnectionManager _connections
//	)
//	: ITask
//{
//	public string Id => "custom_taks1";

//	public IReadOnlyCollection<string> DependsOn => [];

//	public Task<TaskResult> ProcessAsync(IPipelineContext pipelineContext, CancellationToken ct)
//	{
//		throw new NotImplementedException();
//	}
//}

//public interface ILogger { }

//public interface IPipelineContext
//{
//	Guid RunId { get; }

//	DateTimeOffset StartTime { get; }
//}

//public sealed class TaskResult
//{
//	public bool IsSuccess { get; }

//	public Exception? Error { get; }
//}

//public interface IPipeline
//{
//	string Name { get; }

//	IReadOnlyCollection<ITask> Tasks { get; }
//}

//public interface IPipelineRunner
//{
//	Task<PipelineRunResult> RunAsync(IPipeline pipeline, int parallelTaskCount, CancellationToken ct);
//}

//public sealed class PipelineRunResult
//{
//	public bool IsSuccess { get; }

//	public IReadOnlyDictionary<string, TaskResult> TaskResults { get; } = new Dictionary<string, TaskResult>();
//}

//public interface IConnectionManager
//{
//	IConnection Get(string connectionName);
//}

//public interface IConnection { }

//public enum FailurePolicy
//{
//	FailPipeline,
//	SkipDependents
//}

//public interface IPipelineValidator
//{
//	void Validate(IPipeline pipeline);
//}