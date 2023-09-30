﻿using HackYeah_API.Services.Interfaces;
using System.Text;
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

        StringBuilder stringBuilder = new StringBuilder();
        foreach (var dict in result.result)
        {
            foreach (var kvp in dict)
            {
                if(kvp.Value.ToString().IsNullOrEmpty())
                    continue;

                stringBuilder.Append($"{kvp.Key}: {kvp.Value}\n");
            }
            stringBuilder.Append("\n"); 
        }

        string stringifyResult = stringBuilder.ToString();
        await _mlService.SendDDL(stringifyResult);
        return await Task.Run(() => stringifyResult);
    }

    private async Task<string> SaveFile(IFormFile sqlLiteFile)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _databasesPath, sqlLiteFile.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await sqlLiteFile.CopyToAsync(fileStream);
        _queryExecutor.UpdateConnectionString(filePath);
        return filePath;
    }
}