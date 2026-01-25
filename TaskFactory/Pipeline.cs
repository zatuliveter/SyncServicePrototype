
namespace TaskFactory;

public record Pipeline : PipelineItem
{
	internal IReadOnlyCollection<PipelineItem> Items;
	internal RunParameters RunParameters => ((SubPipelineTaskParams)Parameters!).RunParameters;

	public Pipeline(
		string id,
		IReadOnlyCollection<PipelineItem> items,
		RunParameters? runParams = null,
		string[]? dependsOn = null
	)
		: base(
			id,
			typeof(SubPipelineTask),
			dependsOn,
			parameters: new SubPipelineTaskParams(Items: items, RunParameters: runParams ?? RunParameters.Default)
		)
	{
		Items = items;
	}
}
