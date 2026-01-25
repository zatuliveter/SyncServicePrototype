
using Dapper;
using FastMember;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess;


public class SqlServerBulkWriter : IDataWriter
{
	private readonly string _connectionString;

	public SqlServerBulkWriter(string connStr)
	{
		_connectionString = connStr;
	}

	public async Task ExecuteUpsertAsync<T>(
		string tableName,
		IEnumerable<T> data,
		string keyColumn) where T : class
	{
		TypeAccessor accessor = TypeAccessor.Create(typeof(T));
		Member[] members = accessor.GetMembers().ToArray();
		string[] columnNames = members.Select(m => m.Name).ToArray();

		using SqlConnection conn = new SqlConnection(_connectionString);
		await conn.OpenAsync();

		using SqlTransaction trans = conn.BeginTransaction();

		try
		{
			// MSSQL uses # for local temporary tables
			string tempTable = $"#temp_{tableName}";

			// Create temp table structure based on the target table
			string createSql = $"SELECT TOP 0 * INTO {tempTable} " +
							   $"FROM {tableName}";

			using SqlCommand createCmd = new SqlCommand(createSql, conn, trans);
			await createCmd.ExecuteNonQueryAsync();

			// ObjectReader creates IDataReader from IEnumerable<T>
			using (ObjectReader reader = ObjectReader.Create(data, columnNames))
			{
				using SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, trans);

				bulkCopy.DestinationTableName = tempTable;

				// Map columns to ensure exact alignment
				foreach (string col in columnNames)
				{
					bulkCopy.ColumnMappings.Add(col, col);
				}

				await bulkCopy.WriteToServerAsync(reader);
			}

			// Prepare MERGE logic
			IEnumerable<string> updateColumns = columnNames
				.Where(c => !c.Equals(keyColumn, StringComparison.OrdinalIgnoreCase));

			string updateSet = string.Join(", ", updateColumns.Select(c => $"T.[{c}] = S.[{c}]"));

			string columnsList = string.Join(", ", columnNames.Select(c => $"[{c}]"));

			string valuesList = string.Join(", ", columnNames.Select(c => $"S.[{c}]"));

			// Use MERGE for high-performance atomic Upsert
			string mergeSql = $@"
                MERGE INTO {tableName} AS T
                USING {tempTable} AS S 
                ON T.[{keyColumn}] = S.[{keyColumn}]
                WHEN MATCHED THEN UPDATE SET {updateSet}
                WHEN NOT MATCHED THEN INSERT ({columnsList}) 
                VALUES ({valuesList});";

			using SqlCommand mergeCmd = new SqlCommand(mergeSql, conn, trans);

			await mergeCmd.ExecuteNonQueryAsync();

			await trans.CommitAsync();
		}
		catch
		{
			await trans.RollbackAsync();
			throw;
		}
	}
}