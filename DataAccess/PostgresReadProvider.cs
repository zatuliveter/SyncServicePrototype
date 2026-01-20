using Dapper;
using Npgsql;
using System.Data;

namespace DataAccess;

public class PostgresReadProvider : IDataReadProvider
{
	private readonly string _connectionString;

	public PostgresReadProvider(string connStr)
	{
		_connectionString = connStr;
		// Global setting to handle snake_case columns in Postgres
		DefaultTypeMap.MatchNamesWithUnderscores = true;
	}

	public async Task<IEnumerable<T>> QueryAsync<T>(
		string sql,
		object parameters = null,
		CommandType commandType = CommandType.Text)
	{
		await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);

		return await conn.QueryAsync<T>(
			sql,
			parameters,
			commandType: commandType);
	}
}