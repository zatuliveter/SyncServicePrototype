namespace TaskFactory.Common;

public class CopyTableDefinition<TSource, TTarget>
{
	// Source configuration
	public required string SourceConnectionString { get; set; }
	public ProviderType SourceProviderType { get; set; }
	public required string SourceQuery { get; set; }
	public object? SourceParameters { get; set; }

	// Destination configuration
	public required string DestinationConnectionString { get; set; }
	public ProviderType DestinationProviderType { get; set; }
	public required string DestinationTableName { get; set; }
	public required string KeyColumn { get; set; }

	// Transformation logic
	public required Func<TSource, TTarget> Mapper { get; set; }
}