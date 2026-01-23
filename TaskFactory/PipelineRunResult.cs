namespace TaskFactory;

public sealed class PipelineRunResult
{
	public bool IsSuccess { get; set; }

	// key = TaskId
	public IReadOnlyDictionary<string, TaskRunResult> Tasks { get; init; }
		= new Dictionary<string, TaskRunResult>();
}
