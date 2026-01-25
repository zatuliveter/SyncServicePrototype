using Dapper;
using Serilog;
using System.Data;
using System.Text;

namespace TaskFactory.Data;

public class CopyTableDiffTask<TSource, TTarget>
	(
		ILogger logger
	)
	: TaskBase<CopyTableDiffParameters<TSource, TTarget>>
		where TSource : class
		where TTarget : class
{
	private readonly ILogger _logger = logger.ForContext<CopyTableDiffTask<TSource, TTarget>>();

	protected override async Task ExecuteAsync(
		CopyTableDiffParameters<TSource, TTarget> definition,
		string taskId,
		IPipelineContext context,
		CancellationToken ct
	)
	{
		_logger.Information("{pipelineName}.{taskId}: Extracting data from Source.", context.PipelineName, taskId);

		while (true)
		{
			object? lastModified = await definition.SyncState.GetAsync().ConfigureAwait(false);

			int rowCount = definition.ReadBatchSize == 0 ? int.MaxValue : definition.ReadBatchSize;

			StringBuilder query = new();

			query.AppendLine($"""
				select top(@rowCount) with ties
					{string.Join(", ", definition.SourceColumns)}
				from {definition.SourceTable}
				"""
			);

			if (lastModified != null)
			{
				query.AppendLine($"where {definition.ModificationTimeColumn} > @lastModified");
			}

			query.AppendLine($"order by {definition.ModificationTimeColumn}");

			var queryParams = new DynamicParameters();
			queryParams.Add("@rowCount", rowCount);
			queryParams.Add("@lastModified", lastModified, dbType: DbType.DateTime2);

			IEnumerable<TSource> sourceData = await definition.SourceReader.QueryAsync<TSource>(
				query.ToString(),
				queryParams
			);

			int rowsRead = sourceData.Count();

			_logger.Information(
				"{pipelineName}.{taskId}: Rows read count: {RowCount}",
				context.PipelineName, taskId, rowsRead
			);

			if (rowsRead == 0)
			{
				return;
			}

			IEnumerable<TTarget> targetData = sourceData.Select(definition.Mapper);

			_logger.Information("{pipelineName}.{taskId}: Saving data to destination.", context.PipelineName, taskId);

			await definition.TargetWriter.ExecuteUpsertAsync(
				definition.DestinationTableName,
				targetData,
				definition.KeyColumn
			);

			object newSyncState = definition.GetNextSyncState(sourceData.Last());

			await definition.SyncState.SaveAsync(newSyncState).ConfigureAwait(false);

			if (rowsRead < rowCount)
			{
				return;
			}
		}
	}
}