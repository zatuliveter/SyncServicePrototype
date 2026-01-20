namespace DataAccess;

public interface IBulkUpsertProvider
{
	Task ExecuteUpsertAsync<T>(string tableName, IEnumerable<T> data, string keyColumn) where T : class;
}

