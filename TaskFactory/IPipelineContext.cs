namespace TaskFactory;

public interface IPipelineContext
{
	Guid RunId { get; }

	CancellationToken PipelineCancellation { get; }
}
