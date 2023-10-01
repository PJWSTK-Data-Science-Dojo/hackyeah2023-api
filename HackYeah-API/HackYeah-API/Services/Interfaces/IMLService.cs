using HackYeah_API.Models;

namespace HackYeah_API.Services.Interfaces;

public interface IMLService
{
    string APIUrl { get; init; }

    public Task SendDDL(string ddl);
    public Task<string> RequestForSQLPrompt(UserNlpInputDto naturalLanguagePrompt);
}