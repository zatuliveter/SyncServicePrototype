using DataAccess;
using TaskFactory.Data;

namespace TaskFactory.ConsoleApp.Definitions;

public static class ProductDiffParameters
{
	static readonly string sourceConnectionString = "Server=.;Database=sync_demo;Integrated Security=True;TrustServerCertificate=True;";

	public static readonly CopyTableDiffParameters<ProductDto, ProductDto> Params = new()
	{
		SourceReader = new SqlServerReader(sourceConnectionString),
		ModificationTimeColumn = "last_updated",
		SourceTable = "dbo.Products",
		ReadBatchSize = 2,

		SyncState = new SqlServerSyncState<DateTime>("products_table_state", sourceConnectionString),
		GetNextSyncState = row => row.last_updated,

		// Using same type, so mapper is an identity function
		Mapper = source => source,

		TargetWriter = new PostgresBulkWriter("Host=localhost;Port=5433;Username=postgres;Password=123123;Database=sync_demo"),
		DestinationTableName = "products",
		KeyColumn = "product_id"
	};
}
