
using System.Data;
namespace DataAccess;

public interface IDataReadProvider
{
	Task<IEnumerable<T>> QueryAsync<T>(
		string sql,
		object? parameters = null,
		CommandType commandType = CommandType.Text);
}


