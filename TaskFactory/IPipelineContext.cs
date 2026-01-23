namespace TaskFactory;

public interface IPipelineContext
{
	string PipelineName { get; }

	Guid RunId { get; }

	public DateTimeOffset StartTime { get; }

	CancellationToken PipelineCancellation { get; }
}
