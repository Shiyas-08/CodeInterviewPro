using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [Authorize(Roles = "Candidate")]
    [ApiController]
    [Route("api/candidate")]
    public class CandidateController : ControllerBase
    {
        private readonly IInterviewInvitationRepository _invitationRepo;

        public CandidateController(IInterviewInvitationRepository invitationRepo)
        {
            _invitationRepo = invitationRepo;
        }

        [HttpGet("interviews")]
        public async Task<IActionResult> GetMyInterviews()
        {
            var userIdStr = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = Guid.Parse(userIdStr);
            var invitations = await _invitationRepo.GetByCandidateIdAsync(userId);

            return Ok(invitations);
        }
    }
}
