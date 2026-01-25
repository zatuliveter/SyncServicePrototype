namespace TaskFactory.Data;

public static class PipelineBuilderExtensions
{
	public static PipelineBuilder AddCopyTableTask<TSource, TTarget>(
			this PipelineBuilder builder,
			string id,
			CopyTableParameters<TSource, TTarget> parameters,
			params string[]? dependsOn
		)
		where TSource : class
		where TTarget : class
	{
		builder.AddTask<CopyTableTask<TSource, TTarget>, CopyTableParameters<TSource, TTarget>>(
			id, parameters, dependsOn
		);

		return builder;
	}

	public static PipelineBuilder AddCopyTableDiffTask<TSource, TTarget>(
		this PipelineBuilder builder,
		string id,
		CopyTableDiffParameters<TSource, TTarget> parameters,
		params string[]? dependsOn
	)
	where TSource : class
	where TTarget : class
	{
		builder.AddTask<CopyTableDiffTask<TSource, TTarget>, CopyTableDiffParameters<TSource, TTarget>>(
			id, parameters, dependsOn
		);

		return builder;
	}
}

