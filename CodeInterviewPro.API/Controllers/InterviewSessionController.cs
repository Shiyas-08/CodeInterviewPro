        using CodeInterviewPro.Application.Common.Responses;
        using CodeInterviewPro.Application.Interfaces.Services;
        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;

        namespace CodeInterviewPro.API.Controllers
        {
            [ApiController]
            [Route("api/interview-session")]
            [AllowAnonymous] // Candidate access
            public class InterviewSessionController : ControllerBase
            {
                private readonly IInterviewSessionService _service;

                public InterviewSessionController(
                    IInterviewSessionService service)
                {
                    _service = service;
                }

                // Start Session
                [HttpPost("start")]
                public async Task<IActionResult> Start(string token)
                {
                    var result = await _service.StartSessionAsync(token);

                    return Ok(ApiResponse<object>.SuccessResponse(
                        result,
                        "Session started"));
                }

                // Get Session
                [HttpGet("get")]
                public async Task<IActionResult> Get(string token)
                {
                    var result = await _service.GetSessionAsync(token);

                    return Ok(ApiResponse<object>.SuccessResponse(
                        result,
                        "Session fetched"));
                }

                // End Session
                [HttpPost("end")]
                public async Task<IActionResult> End(string token)
                {
                    await _service.EndSessionAsync(token);

                    return Ok(ApiResponse<string>.SuccessResponse(
                        null,
                        "Session ended"));
                }
            }
        }