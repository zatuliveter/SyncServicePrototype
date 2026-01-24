
namespace TaskFactory;

public record PipelineGroup : PipelineItemBase
{
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
	}
}
