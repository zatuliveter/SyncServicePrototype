using DataAccess;
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
		_logger.Information("{pipelineName}.{taskId}: Initializing Source Reader.", context.PipelineName, taskId);
		IDataReadProvider reader = CreateReader(
			definition.SourceProviderType,
			definition.SourceConnectionString
		);

		_logger.Information("{pipelineName}.{taskId}: Initializing Destination Writer.", context.PipelineName, taskId);
		IBulkUpsertProvider writer = CreateWriter(
			definition.DestinationProviderType,
			definition.DestinationConnectionString
		);

		_logger.Information("{pipelineName}.{taskId}: Extracting data from Source.", context.PipelineName, taskId);
		IEnumerable<TSource> sourceData = await reader.QueryAsync<TSource>(
			definition.SourceQuery,
			definition.SourceParameters
		);

		_logger.Information("{pipelineName}.{taskId}: Mapping data.", context.PipelineName, taskId);
		IEnumerable<TTarget> targetData = sourceData.Select(definition.Mapper);

		_logger.Information("{pipelineName}.{taskId}: Saving data to destination.", context.PipelineName, taskId);
		await writer.ExecuteUpsertAsync<TTarget>(
			definition.DestinationTableName,
			targetData,
			definition.KeyColumn
		);
	}

	private static IDataReadProvider CreateReader(ProviderType type, string connStr)
	{
		return type switch
		{
			ProviderType.SqlServer => new SqlServerReadProvider(connStr),
			ProviderType.PostgreSql => new PostgresReadProvider(connStr),
			_ => throw new NotSupportedException($"Reader {type} not supported")
		};
	}

	private static IBulkUpsertProvider CreateWriter(ProviderType type, string connStr)
	{
		return type switch
		{
			ProviderType.SqlServer => new SqlServerBulkProvider(connStr),
			ProviderType.PostgreSql => new PostgresBulkProvider(connStr),
			_ => throw new NotSupportedException($"Writer {type} not supported")
		};
	}

	public static PipelineItem<CopyTableTask<TSource, TTarget>, CopyTableDefinition<TSource, TTarget>>
		CreatePipelineItem(
			string id,
			CopyTableDefinition<TSource, TTarget> parameters,
			params string[] dependsOn
		)
	{
		return new PipelineItem<CopyTableTask<TSource, TTarget>, CopyTableDefinition<TSource, TTarget>>(
			id, parameters, dependsOn);
	}
}