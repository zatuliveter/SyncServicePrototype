
using Services;

namespace SyncExample;

public class ProductsSync
{
	public async Task RunSync()
	{
		// Define mapping from External to Target DTO
		CopyTaskDefinition<ProductDto, ProductDto> definition = new()
		{
			SourceConnectionString = "Server=.;Database=sync_demo;Integrated Security=True;TrustServerCertificate=True;",
			SourceProviderType = ProviderType.SqlServer,
			SourceQuery = "SELECT product_id, name, price, stock_count, last_updated FROM dbo.Products",

			// Using same type, so mapper is an identity function
			Mapper = source => source,

			DestinationConnectionString = "Host=localhost;Port=5433;Username=postgres;Password=123123;Database=sync_demo",
			DestinationProviderType = ProviderType.PostgreSql,
			DestinationTableName = "products",
			KeyColumn = "product_id"
		};

		// Initialize generic task with definition
		CopyTask task = new();

		// Execute synchronization
		await task.Sync(definition);
	}
}
