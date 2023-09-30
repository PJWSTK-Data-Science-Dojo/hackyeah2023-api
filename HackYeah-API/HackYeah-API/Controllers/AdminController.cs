using HackYeah_API.Models;
using HackYeah_API.Services;
using HackYeah_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackYeah_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IDdlExtractionService _ddlExtractionService;
    private readonly IMLService _mlService;
    public AdminController(IDdlExtractionService ddlExtractionService, IMLService mlService)
    {
        _ddlExtractionService = ddlExtractionService;
        _mlService = mlService;
    }

    [HttpPost("upload-database")]
    public async Task<IActionResult> UploadDatabase([FromForm] FileUploadModel fileUpload)
    {
        if (fileUpload.File == null || fileUpload.File.Length == 0)
            return BadRequest("No file uploaded.");

        var ddl = await _ddlExtractionService.ExtractDdl(fileUpload.File);
        await _mlService.SendDDL(ddl);

        return Ok(new {data = ddl});
    }
}