
using System.Data;
namespace DataAccess;

public interface IDataReader
{
	Task<IEnumerable<T>> QueryAsync<T>(
		string sql,
		object? parameters = null,
		CommandType commandType = CommandType.Text);
}


