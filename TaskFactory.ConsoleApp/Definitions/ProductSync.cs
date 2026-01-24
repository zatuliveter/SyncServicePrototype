using TaskFactory.Common;

namespace TaskFactory.ConsoleApp.Definitions;

public static class ProductSync
{
	public static readonly CopyTableDefinition<ProductDto, ProductDto> Definition = new()
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

}

public class ProductDto
{
	public int product_id { get; set; }
	public string name { get; set; } = string.Empty;
	public decimal price { get; set; }
	public int stock_count { get; set; } // Will be mapped to smallint
	public DateTime last_updated { get; set; }
}

