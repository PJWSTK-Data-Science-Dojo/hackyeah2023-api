﻿using HackYeah_API.Services.Interfaces;
using System.Text;
using HackYeah_API.Databases;
using Microsoft.IdentityModel.Tokens;

namespace HackYeah_API.Services;

public class DdlExtractionService : IDdlExtractionService
{
    private readonly IMLService _mlService;
    private string _databasesPath;
    private readonly SqlQueryExecutor _queryExecutor;
    private readonly SQLQueries _sqlQueries;

    public DdlExtractionService(IConfiguration config, IMLService mlService, SqlQueryExecutor queryExecutor, SQLQueries sqlQueries)
    {
        _databasesPath = config.GetValue<string>("StoragePath");
        ArgumentException.ThrowIfNullOrEmpty(_databasesPath, nameof(_databasesPath));
        _mlService = mlService;
        _queryExecutor = queryExecutor;
        _sqlQueries = sqlQueries;
    }

    public async Task<string> ExtractDdl(IFormFile sqlLiteFile)
    {
        await SaveFile(sqlLiteFile);

        string sql = _sqlQueries.QueryGetTablesMetadata;

        var result = await _queryExecutor.ExecuteQueryAsync(sql);

        if (!result.errorMessage.IsNullOrEmpty())
        {
            return result.errorMessage;
        }

        Dictionary<string, List<string>> tableColumns = new Dictionary<string, List<string>>();

        foreach (var dict in result.result)
        {
            string tableName = dict["table_name"]?.ToString();
            string columnName = dict["column_name"]?.ToString();

            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(columnName))
                continue;

            if (!tableColumns.ContainsKey(tableName))
            {
                tableColumns[tableName] = new List<string>();
            }

            tableColumns[tableName].Add(columnName);
        }

        StringBuilder stringBuilder = new StringBuilder();
        foreach (var kvp in tableColumns)
        {
            stringBuilder.Append($"{kvp.Key}: {string.Join(", ", kvp.Value)}\n");
        }

        string stringifyResult = stringBuilder.ToString();
        await _mlService.SendDDL(stringifyResult);
        return await Task.Run(() => stringifyResult);
    }

    private async Task<string> SaveFile(IFormFile sqlLiteFile)
    {
        _queryExecutor.DatabaseName = sqlLiteFile.FileName;
        var newFilename = Path.GetRandomFileName();
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _databasesPath, newFilename);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await sqlLiteFile.CopyToAsync(fileStream);
        _queryExecutor.UpdateConnectionString(filePath);
        return filePath;
    }
}