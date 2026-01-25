using DataAccess;
using TaskFactory.Common;

namespace TaskFactory.ConsoleApp.Definitions;

public static class ProductChangesSync
{
	static readonly string sourceConnectionString = "Server=.;Database=sync_demo;Integrated Security=True;TrustServerCertificate=True;";
	
	public static readonly CopyTableChangesDefinition<ProductDto, ProductDto> Definition = new()
	{
		SourceReader = new SqlServerReader(sourceConnectionString),
		ModificationTimeColumn = "last_updated",
		SourceTable = "dbo.Products",
		ReadBatchSize = 1,

		SyncState = new SqlServerSyncState<DateTime>("products_table_state", sourceConnectionString),

		// Using same type, so mapper is an identity function
		Mapper = source => source,

		TargetWriter = new PostgresBulkWriter("Host=localhost;Port=5433;Username=postgres;Password=123123;Database=sync_demo"),
		DestinationTableName = "products",
		KeyColumn = "product_id"
	};
}
