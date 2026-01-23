
namespace TaskFactory;

internal sealed class PipelineContext : IPipelineContext
{
	public required string PipelineName { get; internal set; }
	public Guid RunId { get; } = Guid.NewGuid();
	public required DateTimeOffset StartTime { get; internal set; }
	public CancellationToken PipelineCancellation { get; internal set; }
}
