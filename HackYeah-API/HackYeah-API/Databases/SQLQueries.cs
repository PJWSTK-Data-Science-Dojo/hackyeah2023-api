using System;
namespace HackYeah_API.Services;

public class SQLQueries
{
    public string QueryGetDatabases { get; private set; }
    public string QueryGetDBInfo { get; private set; }
    public string QueryGetTables { get; private set; }
    public string QueryGetTablesMetadata { get; private set; }
    public SQLQueries(IConfiguration config)
    {
        string queriesFolderPath = config.GetValue<string>("QueriesPath");
        ArgumentException.ThrowIfNullOrEmpty(queriesFolderPath, nameof(queriesFolderPath));

        QueryGetDatabases = ReadQueryFromFile(Path.Combine(queriesFolderPath, "get_databases.sql"));
        QueryGetDBInfo = ReadQueryFromFile(Path.Combine(queriesFolderPath, "get_db_info.sql"));
        QueryGetTables = ReadQueryFromFile(Path.Combine(queriesFolderPath, "get_tables.sql"));
        QueryGetTablesMetadata = ReadQueryFromFile(Path.Combine(queriesFolderPath, "get_tables_metadata.sql"));
    }

    private static string ReadQueryFromFile(string filePath)
    {
        try
        {
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading query from file: {ex.Message}");
            return string.Empty;
        }
    }
}
