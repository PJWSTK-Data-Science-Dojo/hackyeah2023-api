using HackYeah_API.Services.Interfaces;
using System.Text;

namespace HackYeah_API.Services;

public class DdlExtractionService : IDdlExtractionService
{
    private readonly IMLService _mlService;
    private string _databasesPath;
    private SqlQueryExecutor _queryExecutor;
    public DdlExtractionService(IConfiguration config, IMLService mlService, SqlQueryExecutor queryExecutor)
    {
        _databasesPath = config.GetValue<string>("StoragePath");
        ArgumentException.ThrowIfNullOrEmpty(_databasesPath, nameof(_databasesPath));
        _mlService = mlService;
        _queryExecutor = queryExecutor;
    }

    public async Task<string> ExtractDdl(IFormFile sqlLiteFile)
    {
        await SaveFile(sqlLiteFile);

        string sql = @"
                    SELECT
                        m.name AS table_name,
                        p.name AS column_name,
                        p.[type] AS data_type,
                        p.[notnull] AS not_null,
                        p.[dflt_value] AS default_value,
                        f.[table] AS referenced_table,
                        f.[to] AS referenced_column
                    FROM
                        sqlite_master AS m
                    JOIN
                        pragma_table_info(m.name) AS p
                    LEFT JOIN
                        pragma_foreign_key_list(m.name) AS f
                    ON
                        p.name = f.'from'
                    WHERE
                        m.type = 'table'
                    ORDER BY
                        m.name, p.cid;
                ";
        var result = await _queryExecutor.ExecuteQueryAsync(sql);

        if ( result.errorMessage is not null )
        {
            return result.errorMessage;
        } 
        else
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var dict in result.result)
            {
                foreach (var kvp in dict)
                {
                    stringBuilder.Append($"{kvp.Key}: {kvp.Value}\n");
                }
                stringBuilder.Append("\n"); // Add a separator between dictionaries
            }

            await _mlService.SendDDL(stringBuilder.ToString());
        }
        
        return await Task.Run(() => "null");
    }

    private async Task<string> SaveFile(IFormFile sqlLiteFile)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _databasesPath, sqlLiteFile.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await sqlLiteFile.CopyToAsync(fileStream);
        return filePath;
    }
}