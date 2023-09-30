using HackYeah_API.Services.Interfaces;

namespace HackYeah_API.Services;

public class DdlExtractionService : IDdlExtractionService
{
    private readonly IMLService _mlService;
    private string _databasesPath;
    public DdlExtractionService(IConfiguration config, IMLService mlService)
    {
        _databasesPath = config.GetValue<string>("StoragePath");
        ArgumentException.ThrowIfNullOrEmpty(_databasesPath, nameof(_databasesPath));
        _mlService = mlService;
    }

    public async Task<string> ExtractDdl(IFormFile sqlLiteFile)
    {
        await SaveFile(sqlLiteFile);

        //todo read sql file
        string ddl = "null";
        await _mlService.SendDDL(ddl);
        return await Task.Run(() => "null");
    }

    private async Task SaveFile(IFormFile sqlLiteFile)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), _databasesPath, sqlLiteFile.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await sqlLiteFile.CopyToAsync(fileStream);
    }
}