namespace TaskFactory;

public sealed class TaskRunResult
{
	public TaskExecutionStatus Status { get; init; }
	public Exception? Error { get; init; }
}
