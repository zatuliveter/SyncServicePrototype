using FastMember;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace DataAccess;

public class PostgresBulkProvider(string connStr) : IBulkUpsertProvider
{
	private readonly string _connectionString = connStr;

	public async Task ExecuteUpsertAsync<T>(
		string tableName,
		IEnumerable<T> data,
		string keyColumn) where T : class
	{
		TypeAccessor accessor = TypeAccessor.Create(typeof(T));
		// Get members in a fixed order
		Member[] members = accessor.GetMembers().ToArray();

		await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);

		await conn.OpenAsync();

		// Metadata lookup must be case-insensitive
		// This metadata can be cached. 
		Dictionary<string, NpgsqlDbType> columnTypes = await GetColumnMetadata(conn, tableName);

		await using NpgsqlTransaction trans = await conn.BeginTransactionAsync();
		try
		{
			string tempTable = $"temp_{tableName}";
			string createSql = $"CREATE TEMP TABLE {tempTable} " +
							   $"(LIKE {tableName} INCLUDING DEFAULTS) " +
							   "ON COMMIT DROP";

			await new NpgsqlCommand(createSql, conn, trans)
				.ExecuteNonQueryAsync();

			// Explicitly define column order for COPY and loop
			string[] columnNames = members
				.Select(m => m.Name.ToLower())
				.ToArray();

			string columnsSql = string.Join(", ", columnNames);
			string copySql = $"COPY {tempTable} ({columnsSql}) " +
							 "FROM STDIN (FORMAT BINARY)";

			await using (NpgsqlBinaryImporter writer =
				await conn.BeginBinaryImportAsync(copySql))
			{
				foreach (T item in data)
				{
					await writer.StartRowAsync();

					// Use the same order as in columnsSql
					for (int i = 0; i < members.Length; i++)
					{
						string memberName = members[i].Name;
						object value = accessor[item, memberName];
						string colName = columnNames[i];

						if (columnTypes.TryGetValue(colName, out NpgsqlDbType dbType))
						{
							await writer.WriteAsync(value, dbType);
						}
						else
						{
							// Fallback for types not in our map
							await writer.WriteAsync(value);
						}
					}
				}
				await writer.CompleteAsync();
			}

			string updateSet = string.Join(", ", columnNames
				.Where(c => c != keyColumn.ToLower())
				.Select(c => $"{c} = EXCLUDED.{c}"));

			string upsertSql = $@"
                INSERT INTO {tableName} ({columnsSql})
                SELECT {columnsSql} FROM {tempTable}
                ON CONFLICT ({keyColumn.ToLower()}) DO UPDATE SET {updateSet};";

			await new NpgsqlCommand(upsertSql, conn, trans)
				.ExecuteNonQueryAsync();

			await trans.CommitAsync();
		}
		catch
		{
			await trans.RollbackAsync();
			throw;
		}
	}

	private async Task<Dictionary<string, NpgsqlDbType>> GetColumnMetadata(NpgsqlConnection conn, string tableName)
	{		
		var types = new Dictionary<string, NpgsqlDbType>();
		// Query to get UDT names for columns
		var sql = @"SELECT column_name, udt_name 
                    FROM information_schema.columns 
                    WHERE table_name = @tab";

		using var cmd = new NpgsqlCommand(sql, conn);
		cmd.Parameters.AddWithValue("tab", tableName.ToLower());

		using var reader = await cmd.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			var colName = reader.GetString(0);
			var udtName = reader.GetString(1);
			types[colName] = MapUdtToNpgsqlDbType(udtName);
		}
		return types;
	}

	private NpgsqlDbType MapUdtToNpgsqlDbType(string udtName)
	{
		return udtName.ToLower() switch
		{
			// Integers
			"int2" => NpgsqlDbType.Smallint,
			"int4" => NpgsqlDbType.Integer,
			"int8" => NpgsqlDbType.Bigint,

			// Floating point and Numerics
			"float4" => NpgsqlDbType.Real,
			"float8" => NpgsqlDbType.Double,
			"numeric" => NpgsqlDbType.Numeric,
			"money" => NpgsqlDbType.Money,

			// Textual
			"text" => NpgsqlDbType.Text,
			"varchar" => NpgsqlDbType.Varchar,
			"bpchar" => NpgsqlDbType.Char, // Fixed-length char
			"citext" => NpgsqlDbType.Citext,

			// Date and Time
			"date" => NpgsqlDbType.Date,
			"timestamp" => NpgsqlDbType.Timestamp,
			"timestamptz" => NpgsqlDbType.TimestampTz,
			"time" => NpgsqlDbType.Time,
			"timetz" => NpgsqlDbType.TimeTz,
			"interval" => NpgsqlDbType.Interval,

			// Network and UUID
			"uuid" => NpgsqlDbType.Uuid,
			"inet" => NpgsqlDbType.Inet,
			"macaddr" => NpgsqlDbType.MacAddr,

			// JSON
			"json" => NpgsqlDbType.Json,
			"jsonb" => NpgsqlDbType.Jsonb,

			// Others
			"bool" => NpgsqlDbType.Boolean,
			"bytea" => NpgsqlDbType.Bytea,
			"xml" => NpgsqlDbType.Xml,

			// Default
			_ => NpgsqlDbType.Unknown
		};
	}
}