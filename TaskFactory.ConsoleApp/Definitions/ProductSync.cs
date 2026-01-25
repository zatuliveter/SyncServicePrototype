using DataAccess;
using TaskFactory.Data;

namespace TaskFactory.ConsoleApp.Definitions;

public static class ProductSync
{
	public static readonly CopyTableParameters<ProductDto, ProductDto> Params = new()
	{
		SourceReader = new SqlServerReader("Server=.;Database=sync_demo;Integrated Security=True;TrustServerCertificate=True;"),
		SourceQuery = "SELECT product_id, name, price, stock_count, last_updated FROM dbo.Products",

		// Using same type, so mapper is an identity function
		Mapper = source => source,

		TargetWriter = new PostgresBulkWriter("Host=localhost;Port=5433;Username=postgres;Password=123123;Database=sync_demo"),
		DestinationTableName = "products",
		KeyColumn = "product_id"
	};

}
