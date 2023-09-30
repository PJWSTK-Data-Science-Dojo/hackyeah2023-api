using HackYeah_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class QueryController : ControllerBase
{
    private readonly SqlQueryExecutor _sqlQueryExecutor;

    public QueryController(SqlQueryExecutor sqlQueryExecutor)
    {
        _sqlQueryExecutor = sqlQueryExecutor;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateSql([FromBody] UserInputDto userInput)
    {
        //todo add python api integration
        // ... logic to interact with the external Python-based API
        // Assume sqlQuery is the SQL query string received from the external API

        var sqlQuery = "SELECT * FROM VAT_SPRZEDAZ;";

        var queryResult = await _sqlQueryExecutor.ExecuteQueryAsync(sqlQuery);
        if(queryResult.errorMessage.IsNullOrEmpty())
            return Ok(queryResult.result);

        return BadRequest(queryResult.errorMessage);
    }
}