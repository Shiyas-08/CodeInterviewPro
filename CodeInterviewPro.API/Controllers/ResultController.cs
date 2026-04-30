using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Application.Common.Responses;

[ApiController]
[Route("api/results")]
public class ResultController : ControllerBase
{
    private readonly IResultService _service;

    public ResultController(IResultService service)
    {
        _service = service;
    }

    [HttpGet("my/{interviewId}")]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> GetMyResult(Guid interviewId)
    {
        var result = await _service.GetMyResultAsync(interviewId);

        return Ok(ApiResponse<object>.SuccessResponse(result, "Result fetched"));
    }

    [HttpGet("candidate/{candidateId}/interview/{interviewId}")]
    [Authorize(Roles = "SuperAdmin,HR")]
    public async Task<IActionResult> GetCandidateResult(Guid candidateId, Guid interviewId)
    {
        var result = await _service.GetCandidateResultAsync(candidateId, interviewId);

        return Ok(ApiResponse<object>.SuccessResponse(result, "Result fetched"));
    }
}