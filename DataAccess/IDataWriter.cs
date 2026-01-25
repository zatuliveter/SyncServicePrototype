namespace DataAccess;

public interface IDataWriter
{
	Task ExecuteUpsertAsync<T>(string tableName, IEnumerable<T> data, string keyColumn) where T : class;
}

