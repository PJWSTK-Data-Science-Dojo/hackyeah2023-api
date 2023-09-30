using HackYeah_API.Models;
using HackYeah_API.Services;
using HackYeah_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class QueryController : ControllerBase
{
    private readonly SqlQueryExecutor _sqlQueryExecutor;
    private readonly IMLService _mlService;
    public QueryController(SqlQueryExecutor sqlQueryExecutor, IMLService mlService)
    {
        _sqlQueryExecutor = sqlQueryExecutor;
        _mlService = mlService;
    }

    [HttpPost("/prompt")]
    public async Task<IActionResult> PromptForSqlQuery([FromBody] UserNlpInputDto userNlpInput)
    {
        var sqlQuery = await _mlService.RequestForSQLPrompt(userNlpInput.NaturalLanguageInput);
        return Ok(new {sql = sqlQuery});
    }

    [HttpPost("/execute")]
    public async Task<IActionResult> ExecuteSql([FromBody] UserSqlInputDto userSqlInput)
    {
        var queryResult = await _sqlQueryExecutor.ExecuteQueryAsync(userSqlInput.SqlQuery);
        if (queryResult.errorMessage.IsNullOrEmpty())
            return Ok(new {data = queryResult.result});

        return BadRequest(new {error = queryResult.errorMessage});
    }

    [HttpGet("/dbName")]
    public async Task<IActionResult> GetCurrentDbName()
    {
        return Ok(new {data = _sqlQueryExecutor.DatabaseName});
    }
}