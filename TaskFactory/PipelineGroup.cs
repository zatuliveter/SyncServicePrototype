
namespace TaskFactory;

public record PipelineGroup : PipelineItemBase
{
	internal IReadOnlyCollection<PipelineItemBase> Items;
	public PipelineGroup(
		string id,
		IReadOnlyCollection<PipelineItemBase> items,
		RunParameters? runParams = null,
		string[]? dependsOn = null
	)
		: base(
			id, 
			typeof(GroupTask), 
			dependsOn, 
			parameters: new GroupTaskParams(Items: items, PipelineParameters: runParams ?? RunParameters.Default)
		)
	{
		Items = items;
	}
}
