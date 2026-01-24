
namespace TaskFactory;

public record Pipeline : PipelineItemBase
{
	internal IReadOnlyCollection<PipelineItemBase> Items;
	internal RunParameters RunParameters => ((PipelineTaskParams)Parameters!).RunParameters;

	public Pipeline(
		string id,
		IReadOnlyCollection<PipelineItemBase> items,
		RunParameters? runParams = null,
		string[]? dependsOn = null
	)
		: base(
			id, 
			typeof(PipelineTask), 
			dependsOn, 
			parameters: new PipelineTaskParams(Items: items, RunParameters: runParams ?? RunParameters.Default)
		)
	{
		Items = items;		
	}
}
