using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess;

public class SqlServerReader : IDataReader
{
	private readonly string _connectionString;

	public SqlServerReader(string connStr)
	{
		_connectionString = connStr;
	}

	public async Task<IEnumerable<T>> QueryAsync<T>(
		string sql,
		object? parameters = null,
		CommandType commandType = CommandType.Text)
	{
		using SqlConnection conn = new(_connectionString);

		return await conn.QueryAsync<T>(
			sql,
			parameters,
			commandType: commandType);
	}
}
