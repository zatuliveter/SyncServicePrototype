
namespace TaskFactory;

internal sealed class PipelineContext : IPipelineContext
{
	public Guid RunId { get; } = Guid.NewGuid();
	public DateTimeOffset StartTime { get; } = DateTimeOffset.UtcNow;
	public CancellationToken PipelineCancellation { get; internal set; }
}
