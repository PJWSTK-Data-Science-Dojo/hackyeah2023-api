using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace HackYeah_API.Services;

public class SqlQueryExecutor
{
    private readonly string _connectionString;

    public SqlQueryExecutor(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SQLitePath");
    }

    public async Task<(List<Dictionary<string, object>> result, string errorMessage)> ExecuteQueryAsync(string sqlQuery)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
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

    //public async Task<(List<Dictionary<string, object>> result, string errorMessage)> ExecuteQueryAsync(string sqlQuery)
    //{
    //    try
    //    {
    //        await using var command = _context.Database.GetDbConnection().CreateCommand();
    //        command.CommandText = sqlQuery;
    //        await _context.Database.OpenConnectionAsync();
    //        await using var result = await command.ExecuteReaderAsync();
    //        var data = new List<Dictionary<string, object>>();

    //        while (await result.ReadAsync())
    //        {
    //            var row = new Dictionary<string, object>();
    //            for (var i = 0; i < result.FieldCount; i++)
    //            {
    //                row[result.GetName(i)] = result.GetValue(i);
    //            }

    //            data.Add(row);
    //        }

    //        await _context.Database.CloseConnectionAsync();
    //        return (data, null);
    //    }
    //    catch (Exception ex)
    //    {
    //        return (null, ex.Message);
    //    }
    //}
}