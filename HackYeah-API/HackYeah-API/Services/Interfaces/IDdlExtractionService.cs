namespace HackYeah_API.Services.Interfaces;

public interface IDdlExtractionService
{
    Task<string> ExtractDdl(IFormFile sqlLiteFile);
}