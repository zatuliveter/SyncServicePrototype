using Dapper;
using Npgsql;
using System.Data;

namespace DataAccess;

public class PostgresReader : IDataReader
{
	private readonly string _connectionString;

	public PostgresReader(string connStr)
	{
		_connectionString = connStr;
	}

	public async Task<IEnumerable<T>> QueryAsync<T>(
		string sql,
		object? parameters = null,
		CommandType commandType = CommandType.Text)
	{
		await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);

		return await conn.QueryAsync<T>(
			sql,
			parameters,
			commandType: commandType);
	}
}