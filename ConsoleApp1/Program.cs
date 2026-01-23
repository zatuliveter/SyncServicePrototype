using ConsoleApp1.Data;
using DataAccess;

// Initialize provider
IBulkUpsertProvider pg_provider = new PostgresBulkProvider(
	"Host=localhost;Port=5433;Username=postgres;Password=123123;Database=sync_demo"
);

IBulkUpsertProvider mssql_provider = new SqlServerBulkProvider(
	"Server=.;Database=sync_demo;Integrated Security=True;TrustServerCertificate=True;"
);

// Prepare data
List<ProductDto> products = SampleData.GetProducts(100);

try
{
	Console.WriteLine("Starting bulk upsert...");

	foreach (IBulkUpsertProvider provider in (IBulkUpsertProvider[])[pg_provider, mssql_provider])
	{
		Console.WriteLine(provider.GetType().Name);
		await provider.ExecuteUpsertAsync<ProductDto>(
			"products",
			products,
			"product_id"
		);

		Console.WriteLine("Bulk upsert completed successfully.");
	}
}
catch (Exception ex)
{
	Console.WriteLine($"Error: {ex.Message}");
}