        using CodeInterviewPro.Application.Common.Responses;
        using CodeInterviewPro.Application.Interfaces.Services;
        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;

        namespace CodeInterviewPro.API.Controllers
        {
    [ApiController]
    [Route("api/interview-session")]
    public class InterviewSessionController : ControllerBase
    {
        private readonly IInterviewSessionService _service;

        public InterviewSessionController(IInterviewSessionService service)
        {
            _service = service;
        }

        // Candidate check session
        [Authorize(Roles = "Candidate,HR,SuperAdmin")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(string token)
        {
            var result = await _service.GetSessionAsync(token);

            return Ok(ApiResponse<object>.SuccessResponse(
                result,
                "Session fetched"));
        }

        //  HR stop interview
        [Authorize(Roles = "HR")]
        [HttpPost("stop")]
        public async Task<IActionResult> Stop(string token)
        {
            await _service.StopSessionAsync(token);

            return Ok(ApiResponse<string>.SuccessResponse(
                null,
                "Interview stopped"));
        }
        // Candidate resume interview room
        [Authorize(Roles = "Candidate")]
        [HttpGet("resume")]
        public async Task<IActionResult> Resume(string token)
        {
            var result = await _service.ResumeSessionAsync(token);

            return Ok(ApiResponse<object>.SuccessResponse(
                result,
                "Interview resumed"));
        }

    }
}
        