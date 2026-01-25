
using DataAccess;

namespace TaskFactory.Data;

public class CopyTableDiffParameters<TSource, TTarget>
{
	// Source configuration
	public required IDataReader SourceReader { get; init; }
	public required string ModificationTimeColumn { get; set; }
	public required string SourceTable { get; set; }
	public int ReadBatchSize { get; set; } = 0;
	public string[] SourceColumns { get; set; } = ["*"];


	public required ISyncState SyncState { get; set; }
	public required Func<TSource, object> GetNextSyncState{ get; set; }

	// Destination configuration
	public required IDataWriter TargetWriter { get; set; }

	public required string DestinationTableName { get; set; }
	public required string KeyColumn { get; set; }

	// Transformation logic
	public required Func<TSource, TTarget> Mapper { get; set; }
}
