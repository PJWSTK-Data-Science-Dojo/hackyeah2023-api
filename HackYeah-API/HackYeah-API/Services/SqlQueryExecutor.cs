using Microsoft.Data.Sqlite;

namespace HackYeah_API.Services;

public class SqlQueryExecutor
{
    private readonly string _connectionString;

    public SqlQueryExecutor(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SQLitePath");
        ArgumentException.ThrowIfNullOrEmpty(_connectionString,nameof(_connectionString));
    }

    public async Task<(List<Dictionary<string, object>> result, string errorMessage)> ExecuteQueryAsync(string sqlQuery, string? connectionString = null)
    {
        try
        {
            await using var connection = new SqliteConnection(connectionString  ?? _connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            var data = new List<Dictionary<string, object>>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                data.Add(row);
            }

            return (data, null);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }
}