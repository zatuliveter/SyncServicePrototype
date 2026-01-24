namespace TaskFactory;

public interface IPipelineContext
{
	string PipelineName { get; }

	Guid RunId { get; }

	DateTimeOffset StartTime { get; }

	CancellationToken PipelineCancellation { get; }
}
