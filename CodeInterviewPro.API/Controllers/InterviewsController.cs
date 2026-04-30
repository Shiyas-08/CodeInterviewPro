using CodeInterviewPro.Application.Common.Responses;
using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [ApiController]
    [Route("api/interviews")]
    [Authorize(Roles = "SuperAdmin,HR")]    
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _service;

        public InterviewsController(IInterviewService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var interviews = await _service.GetAllAsync();
            return Ok(ApiResponse<object>.SuccessResponse(interviews, "Interviews fetched successfully"));
        }

        // Create Interview
        [HttpPost]
        public async Task<IActionResult> Create(CreateInterviewDto dto)
        {
            var id = await _service.CreateAsync(dto);

            return Ok(ApiResponse<Guid>.SuccessResponse(
                id,
                "Interview created successfully"));
        }

        // Assign Candidate
        //[HttpPost("{id}/assign")]
        //public async Task<IActionResult> Assign(
        //    Guid id,
        //    AssignCandidateDto dto)
        //{
        //    await _service.AssignCandidateAsync(id, dto);

        //    return Ok(ApiResponse<string>.SuccessResponse(
        //        null,
        //        "Candidate assigned successfully"));
        //}

      
        

        [HttpPost("{id}/questions")]
        public async Task<IActionResult> AssignQuestions( Guid id, AssignQuestionsDto dto)
        {
            await _service.AssignQuestionsAsync(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse(
                null,
                "Questions assigned successfully"));
        }
        // Schedule Interview
        [HttpPut("{id}/schedule")]
        public async Task<IActionResult> Schedule(
            Guid id,
            ScheduleInterviewDto dto)
        {
            await _service.ScheduleAsync(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse(
                null,
                "Interview scheduled successfully"));
        }


        // Invite Candidate (Email Based)
        [HttpPost("{id}/invite")]
        public async Task<IActionResult> Invite(
            Guid id,
            InviteCandidateDto dto)
        {
            var link = await _service.InviteCandidateAsync(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse(
                link,
                "Candidate invited successfully"));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Interview removed successfully"));
        }
    }
}