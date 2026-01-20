
namespace DataAccess;

public static class SampleData
{
	public static List<ProductDto> GetProducts(int count)
	{
		List<ProductDto> products = new List<ProductDto>(count);
		Random random = new Random();

		for (int i = 1; i <= count; i++)
		{
			// Generate random values for testing
			decimal randomPrice = (decimal)(random.NextDouble() * 1000);
			int randomStock = random.Next(0, 500);

			ProductDto product = new ProductDto
			{
				product_id = i,
				name = $"Product_{i}",
				price = Math.Round(randomPrice, 2),
				stock_count = randomStock,
				last_updated = DateTime.Now
			};

			products.Add(product);
		}

		return products;
	}
}