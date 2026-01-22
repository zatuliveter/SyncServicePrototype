using DataAccess;

namespace Services;

public class CopyTask
{
	// The method is generic, but the class is not.
	// C# will infer TSource and TTarget from the 'definition' parameter.
	public async Task Sync<TSource, TTarget>(
		CopyTaskDefinition<TSource, TTarget> definition)
		where TSource : class
		where TTarget : class
	{
		// 1. Initialize Source Reader
		IDataReadProvider reader = CreateReader(
			definition.SourceProviderType,
			definition.SourceConnectionString
		);

		// 2. Initialize Destination Writer
		IBulkUpsertProvider writer = CreateWriter(
			definition.DestinationProviderType,
			definition.DestinationConnectionString
		);

		// 3. Extract data from Source
		IEnumerable<TSource> sourceData = await reader.QueryAsync<TSource>(
			definition.SourceQuery,
			definition.SourceParameters
		);

		// 4. Transform TSource -> TTarget
		IEnumerable<TTarget> targetData = sourceData.Select(definition.Mapper);

		// 5. Load transformed data to Destination
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
}