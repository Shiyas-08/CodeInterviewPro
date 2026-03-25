using CodeInterviewPro.Application.Common.Responses;
using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [ApiController]
    [Route("api/interviews")]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _service;

        public InterviewsController(IInterviewService service)
        {
            _service = service;
        }

        // Create Interview
        [HttpPost]
        public async Task<IActionResult> Create(CreateInterviewDto dto)
        {
            var id = await _service.CreateAsync(dto);

            return Ok(ApiResponse<long>.SuccessResponse(
                id,
                "Interview created successfully"));
        }

        // Assign Candidate
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> Assign(
            long id,
            AssignCandidateDto dto)
        {
            await _service.AssignCandidateAsync(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse(
                null,
                "Candidate assigned successfully"));
        }

        // Schedule Interview
        [HttpPut("{id}/schedule")]
        public async Task<IActionResult> Schedule(
            long id,
            ScheduleInterviewDto dto)
        {
            await _service.ScheduleAsync(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse(
                null,
                "Interview scheduled successfully"));
        }

        // Generate Link
        [HttpPost("{id}/generate-link")]
        public async Task<IActionResult> GenerateLink(
            long id,
            GenerateLinkDto dto)
        {
            var token = await _service.GenerateLinkAsync(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse(
                token,
                "Link generated successfully"));
        }
    }
}