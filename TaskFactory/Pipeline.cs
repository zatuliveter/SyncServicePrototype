namespace TaskFactory;

public class Pipeline : IPipeline
{
	public required string Name { get; init; }

	public required IReadOnlyCollection<PipelineItemBase> Items { get; init; }
}