using System.Text;
using HackYeah_API.Models;
using HackYeah_API.Services.Interfaces;
using Newtonsoft.Json;

namespace HackYeah_API.Services;

public class MLService : IMLService
{
    public string APIUrl { get; init; }
    private readonly IHttpClientFactory _httpClientFactory;
    private const string _premiumQueryEndpoint = "/chat/v2";
    private const string _queryEndpoint = "/chat";
    private const string _ddlEndpoint = "/context";
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

        var response = await client.PostAsync($"http://{APIUrl}{_ddlEndpoint}", jsonContent);
    }

    public async Task<string> RequestForSQLPrompt(UserNlpInputDto naturalLanguagePrompt)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMinutes(5);
        var payload = new
        {
            query = naturalLanguagePrompt.NaturalLanguageInput,
            hash = _contextGuid
        };

        var payloadJson = JsonConvert.SerializeObject(payload);
        var jsonContent = new StringContent(payloadJson, Encoding.UTF8, "application/json");
        string endpoint = naturalLanguagePrompt.IsPremiumModel ? _premiumQueryEndpoint : _queryEndpoint;
        var request = await client.PostAsync($"http://{APIUrl}{endpoint}", jsonContent);
        var response = await request.Content.ReadFromJsonAsync<MLApiResponse>();
        return response?.output ?? "ERROR";
    }
}