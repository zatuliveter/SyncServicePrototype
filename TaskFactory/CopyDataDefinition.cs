namespace TaskFactory;

public sealed record CopyDataDefinition
{
	public required string SourceTableName { get; init; }
	// ... other properties.
}
