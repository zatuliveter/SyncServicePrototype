using DataAccess;

namespace TaskFactory.Data;

public class CopyTableParameters<TSource, TTarget>
{
	// Source configuration
	public required IDataReader SourceReader { get; init; }
	public required string SourceQuery { get; set; }
	public object? SourceParameters { get; set; }

	// Destination configuration
	public required IDataWriter TargetWriter { get; set; }

	public required string DestinationTableName { get; set; }
	public required string KeyColumn { get; set; }

	// Transformation logic
	public required Func<TSource, TTarget> Mapper { get; set; }
}