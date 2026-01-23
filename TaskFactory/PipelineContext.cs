
namespace TaskFactory;

internal sealed class PipelineContext : IPipelineContext
{
	public required string PipelineName { get; internal init; }
	public Guid RunId { get; } = Guid.NewGuid();
	public required DateTimeOffset StartTime { get; internal init; }
	public CancellationToken PipelineCancellation { get; internal init; }
}
