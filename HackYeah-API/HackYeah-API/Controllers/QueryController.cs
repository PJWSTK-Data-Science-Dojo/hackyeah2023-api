using HackYeah_API.Services;
using HackYeah_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    public async Task<IActionResult> GenerateSql([FromBody] UserInputDto userInput)
    {
        var sqlQuery = await _mlService.RequestForSQLPrompt(userInput.NaturalLanguageInput);
        var queryResult = await _sqlQueryExecutor.ExecuteQueryAsync(sqlQuery);
        if(queryResult.errorMessage.IsNullOrEmpty())
            return Ok(queryResult.result);

        return BadRequest(queryResult.errorMessage);
    }
}