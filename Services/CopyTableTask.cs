using Serilog;

namespace TaskFactory.Common;

public class CopyTableTask<TSource, TTarget>
	(
		ILogger logger
	)
	: TaskBase<CopyTableDefinition<TSource, TTarget>>
		where TSource : class
		where TTarget : class
{
	private readonly ILogger _logger = logger.ForContext<CopyTableTask<TSource, TTarget>>();

	protected override async Task ExecuteAsync(CopyTableDefinition<TSource, TTarget> definition, string taskId, IPipelineContext context, CancellationToken ct)
	{
		_logger.Information("{pipelineName}.{taskId}: Extracting data from Source.", context.PipelineName, taskId);
		IEnumerable<TSource> sourceData = await definition.SourceReader.QueryAsync<TSource>(
			definition.SourceQuery,
			definition.SourceParameters
		);

		_logger.Information(
			"{pipelineName}.{taskId}: Mapping data. RowCount={RowCount}",
			context.PipelineName, taskId, sourceData.Count()
		);
		IEnumerable<TTarget> targetData = sourceData.Select(definition.Mapper);

		_logger.Information("{pipelineName}.{taskId}: Saving data to destination.", context.PipelineName, taskId);
		await definition.TargetWriter.ExecuteUpsertAsync<TTarget>(
			definition.DestinationTableName,
			targetData,
			definition.KeyColumn
		);
	}
}

public static class CopyTableTask
{
	public static PipelineItem<CopyTableTask<TSource, TTarget>, CopyTableDefinition<TSource, TTarget>>
		PipelineItem<TSource, TTarget>(
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

