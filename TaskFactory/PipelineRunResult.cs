namespace TaskFactory;

public sealed class PipelineRunResult
{
	public required bool IsSuccess { get; init; }

	public required DateTimeOffset PipelineStartTime { get; init; }

	public required DateTimeOffset PipelineEndTime { get; init; }

	public TimeSpan Duration => PipelineEndTime - PipelineStartTime;

	public required IReadOnlyDictionary<string, TaskRunResult> Tasks { get; init; }
		= new Dictionary<string, TaskRunResult>();
}
