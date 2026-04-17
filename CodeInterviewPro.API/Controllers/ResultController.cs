using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CodeInterviewPro.Application.Interfaces.Services;

[ApiController]
[Route("api/results")]
[Authorize(Roles = "Candidate")]
public class ResultController : ControllerBase
{
    private readonly IResultService _service;

    public ResultController(IResultService service)
    {
        _service = service;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyResult()
    {
        var result = await _service.GetMyResultAsync();

        return Ok(result);
    }
}