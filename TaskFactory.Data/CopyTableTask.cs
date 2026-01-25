using Serilog;

namespace TaskFactory.Data;

public class CopyTableTask<TSource, TTarget>
	(
		ILogger logger
	)
	: TaskBase<CopyTableParameters<TSource, TTarget>>
		where TSource : class
		where TTarget : class
{
	private readonly ILogger _logger = logger.ForContext<CopyTableTask<TSource, TTarget>>();

	protected override async Task ExecuteAsync(CopyTableParameters<TSource, TTarget> definition, string taskId, IPipelineContext context, CancellationToken ct)
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
		await definition.TargetWriter.ExecuteUpsertAsync(
			definition.DestinationTableName,
			targetData,
			definition.KeyColumn
		);
	}
}

