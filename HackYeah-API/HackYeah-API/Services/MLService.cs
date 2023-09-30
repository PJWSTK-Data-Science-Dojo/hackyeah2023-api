using HackYeah_API.Services.Interfaces;

namespace HackYeah_API.Services;

public class MLService : IMLService
{
    public string APIUrl { get; init; }
    private readonly IHttpClientFactory _httpClientFactory;

    public MLService(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        APIUrl = config.GetValue<string>("MLAPIUrl");
        ArgumentException.ThrowIfNullOrEmpty(APIUrl, nameof(APIUrl));
        _httpClientFactory = httpClientFactory;
    }

    public Task SendDDL(string ddl)
    {
        //todo implement
        return null;
    }

    public Task<string> RequestForSQLPrompt(string naturalLanguagePrompt)
    {
        //todo implement
        return Task.Run(()=>"SELECT * FROM VAT_SPRZEDAZ;");
    }
}