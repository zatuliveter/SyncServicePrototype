using Serilog;
using System.Text;

namespace TaskFactory.Common;

public class CopyTableChangesTask<TSource, TTarget>
	(
		ILogger logger
	)
	: TaskBase<CopyTableChangesDefinition<TSource, TTarget>>
		where TSource : class
		where TTarget : class
{
	private readonly ILogger _logger = logger.ForContext<CopyTableChangesTask<TSource, TTarget>>();

	protected override async Task ExecuteAsync(
		CopyTableChangesDefinition<TSource, TTarget> definition, 
		string taskId, 
		IPipelineContext context, 
		CancellationToken ct
	)
	{
		_logger.Information("{pipelineName}.{taskId}: Extracting data from Source.", context.PipelineName, taskId);

		object? lastModified = await definition.SyncState.GetAsync().ConfigureAwait(false);

		uint rowCount = definition.ReadBatchSize == 0 ? int.MaxValue : definition.ReadBatchSize;

		StringBuilder query = new();

		query.AppendLine($"""
			select top(@row_count) with ties
				{definition.SourceColumns}
			from {definition.SourceTable}
			"""
		);
		
		if (lastModified != null)
		{
			query.AppendLine($"where {definition.ModificationTimeColumn} > @lastModified");
		}

		query.AppendLine($"order by { definition.ModificationTimeColumn})");

		IEnumerable<TSource> sourceData = await definition.SourceReader.QueryAsync<TSource>(
			query.ToString(),
			new { rowCount, lastModified }
		);

		_logger.Information(
			"{pipelineName}.{taskId}: Rows read count: {RowCount}",
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