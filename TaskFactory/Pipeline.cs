
namespace TaskFactory;

public record Pipeline : PipelineItemBase
{
	internal IReadOnlyCollection<PipelineItemBase> Items;
	internal RunParameters RunParameters => ((SupPipelineTaskParams)Parameters!).RunParameters;

	public Pipeline(
		string id,
		IReadOnlyCollection<PipelineItemBase> items,
		RunParameters? runParams = null,
		string[]? dependsOn = null
	)
		: base(
			id, 
			typeof(SubPipelineTask), 
			dependsOn, 
			parameters: new SupPipelineTaskParams(Items: items, RunParameters: runParams ?? RunParameters.Default)
		)
	{
		Items = items;		
	}
}
