namespace TaskFactory.ConsoleApp.Definitions;

public class ProductDto
{
	public int product_id { get; set; }
	public string name { get; set; } = string.Empty;
	public decimal price { get; set; }
	public int stock_count { get; set; } // Will be mapped to smallint
	public DateTime last_updated { get; set; }
}

