using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using TaskFactory.Common;

namespace TaskFactory.ConsoleApp.Definitions;


internal sealed class SqlServerSyncState<T>(string name, string connectionString) : ISyncState
{
	private readonly string _connectionString = connectionString;
	private static readonly JsonSerializerOptions _jsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public async Task<object?> GetAsync()
	{
		const string sql = """
			select [value]
			from dbo.SyncState
			where name = @name;
			""";

		await using SqlConnection cnn = new(_connectionString);
		string? json = await cnn.QuerySingleOrDefaultAsync<string>(
			sql,
			new {
				name = new DbString
				{
					Value = name,
					IsAnsi = true,
					Length = 128
				}
			}
		).ConfigureAwait(false);

		if (json is null)
			return default;

		return JsonSerializer.Deserialize<T>(json, _jsonOptions);
	}

	public async Task SaveAsync(object value)
	{
		string json = JsonSerializer.Serialize(value, _jsonOptions);

		const string sql = """
			merge dbo.SyncState as t
			using (select @name as name, @value as value) as s
				on t.name = s.name
			when matched then
				update set 
					t.value = s.value,
					t.updated_utc = sysutcdatetime()
			when not matched then
				insert (name, value, updated_utc)
				values (s.name, s.value, sysutcdatetime());
			""";

		await using SqlConnection cnn = new(_connectionString);
		await cnn.ExecuteAsync(
			sql,
			new
			{
				name = new DbString
				{
					Value = name,
					IsAnsi = true,
					Length = 128
				},
				value = json
			}
		).ConfigureAwait(false);
	}
}
