namespace TaskFactory;

public interface IPipeline
{
	string Name { get; }
	IReadOnlyCollection<PipelineItemBase> Items { get; }
}
