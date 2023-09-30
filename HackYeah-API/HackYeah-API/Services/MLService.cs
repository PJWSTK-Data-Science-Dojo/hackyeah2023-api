using System.Text;
using HackYeah_API.Models;
using HackYeah_API.Services.Interfaces;
using Newtonsoft.Json;

namespace HackYeah_API.Services;

public class MLService : IMLService
{
    public string APIUrl { get; init; }
    private readonly IHttpClientFactory _httpClientFactory;
    private const string _ddlEndpoint = "/chat";
    private const string _queryEndpoint = "/context";
    private readonly string _contextGuid = Guid.NewGuid().ToString();
    public MLService(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        APIUrl = config.GetValue<string>("MLAPIUrl");
        ArgumentException.ThrowIfNullOrEmpty(APIUrl, nameof(APIUrl));
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendDDL(string ddl)
    {
        var client = _httpClientFactory.CreateClient();

        var payload = new
        {
            structure = ddl
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{APIUrl}{_ddlEndpoint}", jsonContent);
    }

    public async Task<string> RequestForSQLPrompt(string naturalLanguagePrompt)
    {
        var client = _httpClientFactory.CreateClient();

        var payload = new
        {
            query = naturalLanguagePrompt,
            hash = _contextGuid
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        var request = await client.PostAsync($"{APIUrl}{_queryEndpoint}", jsonContent);
        var response = await request.Content.ReadFromJsonAsync<MLApiResponse>();
        return response?.Output ?? "ERROR";
    }
}