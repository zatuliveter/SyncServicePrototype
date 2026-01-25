using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess;

public class SqlServerReader(string connStr) : IDataReader
{
	private readonly string _connectionString = connStr;

	public async Task<IEnumerable<T>> QueryAsync<T>(
		string sql,
		object? parameters = null,
		CommandType commandType = CommandType.Text)
	{
		using SqlConnection conn = new SqlConnection(_connectionString);

		// Dapper works with any IDbConnection
		return await conn.QueryAsync<T>(
			sql,
			parameters,
			commandType: commandType);
	}
}
