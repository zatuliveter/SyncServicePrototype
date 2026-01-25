namespace TaskFactory.Common;

public record CopyTablePipelineItem<TSource, TTarget> : PipelineItemBase
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

