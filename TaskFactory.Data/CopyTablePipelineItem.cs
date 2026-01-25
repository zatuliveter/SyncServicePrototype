namespace TaskFactory.Data;

public record CopyTablePipelineItem<TSource, TTarget> : PipelineItem
	where TSource : class
	where TTarget : class
{
	public CopyTablePipelineItem(
		string id,
		CopyTableDefinition<TSource, TTarget> parameters,
		string[]? dependsOn = null
	)
	: base(id, typeof(CopyTableTask<TSource, TTarget>), dependsOn, parameters)
	{
	}
}

