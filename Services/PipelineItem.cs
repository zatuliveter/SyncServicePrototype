namespace TaskFactory.Common;

public static class PipelineItem
{
	public static PipelineItem<CopyTableTask<TSource, TTarget>, CopyTableDefinition<TSource, TTarget>>
		CopyTable<TSource, TTarget>(
			string id,
			CopyTableDefinition<TSource, TTarget> parameters,
			params string[]? dependsOn)
		where TSource : class
		where TTarget : class
	{
		return new PipelineItem<CopyTableTask<TSource, TTarget>, CopyTableDefinition<TSource, TTarget>>(
			id, parameters, dependsOn);
	}
}