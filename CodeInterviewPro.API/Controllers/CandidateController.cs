using CodeInterviewPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [ApiController]
    [Route("api/candidate")]
    [Authorize(Roles = "Candidate")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _service;

        public CandidateController(ICandidateService service)
        {
            _service = service;
        }

        [HttpGet("interviews")]
        public async Task<IActionResult> GetMyInterviews()
        {
            var result = await _service.GetMyInterviewsAsync();

            return Ok(result);
        }
    }
}