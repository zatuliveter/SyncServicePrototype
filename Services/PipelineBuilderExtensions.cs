namespace TaskFactory.Common;

public static class PipelineBuilderExtensions
{
	public static PipelineBuilder AddCopyTableTask<TSource, TTarget>(
			this PipelineBuilder builder,
			string id,
			CopyTableDefinition<TSource, TTarget> parameters,
			params string[]? dependsOn
		)
		where TSource : class
		where TTarget : class
	{
		builder.AddTask<CopyTableTask<TSource, TTarget>, CopyTableDefinition<TSource, TTarget>>(
			id, parameters, dependsOn
		);

		return builder;
	}
}

