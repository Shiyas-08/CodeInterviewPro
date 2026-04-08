using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories;
using Microsoft.AspNetCore.Http;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepo;
        private readonly IInterviewCandidateRepository _candidateRepo;
        private readonly IInterviewInvitationRepository _invitationRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInterviewQuestionRepository _questionRepo;

        public InterviewService(
            IInterviewRepository interviewRepo,
            IInterviewCandidateRepository candidateRepo,
            IInterviewInvitationRepository invitationRepo,
            IUserRepository userRepo,
            IHttpContextAccessor httpContextAccessor,
            IInterviewQuestionRepository questionRepo)
        {
            _interviewRepo = interviewRepo;
            _candidateRepo = candidateRepo;
            _invitationRepo = invitationRepo;
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
            _questionRepo = questionRepo;
        }
        private Guid GetTenantId()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims;

            var tenantClaim = claims?
                .FirstOrDefault(x =>
                    x.Type == "tid" ||
                    x.Type == "tenantid" ||
                    x.Type == "http://schemas.microsoft.com/identity/claims/tenantid");

            if (tenantClaim == null)
                throw new Exception("TenantId not found in token");

            return Guid.Parse(tenantClaim.Value);
        }

        // CREATE INTERVIEW
        public async Task<Guid> CreateAsync(CreateInterviewDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new Exception("Interview title required");

            if (dto.DurationMinutes <= 0)
                throw new Exception("Invalid duration");

            var userId = GetUserId();   // ADD THIS

            var interview = new Interview
            {
                TenantId = GetTenantId(),
                CreatedBy = userId,     // ADD THIS
                Title = dto.Title,
                Description = dto.Description,
                DurationMinutes = dto.DurationMinutes,
                Status = (int)InterviewStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return await _interviewRepo.CreateAsync(interview);
        }

        // ASSIGN CANDIDATE
        public async Task AssignCandidateAsync(Guid interviewId, AssignCandidateDto dto)
        {
            var tenantId = GetTenantId();

            var interview = await _interviewRepo.GetByIdAsync(interviewId, tenantId);

            if (interview == null)
                throw new Exception("Interview not found");

            if (interview.Status == (int)InterviewStatus.Completed)
                throw new Exception("Interview already completed");

            var candidate = await _userRepo.GetById(dto.CandidateId);

            if (candidate == null)
                throw new Exception("Candidate not found");

            if (candidate.Role != UserRole.Candidate)
                throw new Exception("Invalid candidate");

            var existing = await _candidateRepo.GetAsync(
                interviewId,
                dto.CandidateId,
                tenantId);

            if (existing != null)
                throw new Exception("Candidate already assigned");

            var entity = new InterviewCandidate
            {
                TenantId = tenantId,   
                InterviewId = interviewId,
                CandidateId = dto.CandidateId,
                Status = (int)InterviewCandidateStatus.Invited,
                AssignedAt = DateTime.UtcNow
            };

            await _candidateRepo.AssignCandidateAsync(entity);
        }

        // SCHEDULE
        public async Task ScheduleAsync(Guid interviewId, ScheduleInterviewDto dto)
        {
            var tenantId = GetTenantId();

            var interview = await _interviewRepo.GetByIdAsync(interviewId, tenantId);

            if (interview == null)
                throw new Exception("Interview not found");

            if (dto.StartTime >= dto.EndTime)
                throw new Exception("Invalid schedule");

            if (dto.StartTime < DateTime.UtcNow)
                throw new Exception("Start time cannot be past");

            interview.StartTime = dto.StartTime;
            interview.EndTime = dto.EndTime;
            interview.Status = (int)InterviewStatus.Scheduled;

            await _interviewRepo.UpdateAsync(interview);
        }

        // GENERATE LINK
        public async Task<GenerateLinkResponse> GenerateLinkAsync(Guid interviewId, GenerateLinkDto dto)
        {
            var tenantId = GetTenantId();

            var interview = await _interviewRepo.GetByIdAsync(interviewId, tenantId);

            if (interview == null)
                throw new Exception("Interview not found");

            var assigned = await _candidateRepo.GetAsync(
                interviewId,
                dto.CandidateId,
                tenantId);

            if (assigned == null)
                throw new Exception("Candidate not assigned");

            if (dto.ExpiryTime <= DateTime.UtcNow)
                throw new Exception("Invalid expiry time");

            var token = Guid.NewGuid().ToString();

            var invitation = new InterviewInvitation
            {
                TenantId = tenantId,  // FIXED
                InterviewId = interviewId,
                CandidateId = dto.CandidateId,
                Token = token,
                ExpiryTime = dto.ExpiryTime,
                IsUsed = false
            };

            await _invitationRepo.CreateAsync(invitation);

            return new GenerateLinkResponse
            {
                Token = token,
                Link = $"https://localhost:5001/interview/{token}",
                ExpiryTime = dto.ExpiryTime
            };
        }

        // ASSIGN QUESTIONS
        public async Task AssignQuestionsAsync(
            Guid interviewId,
            AssignQuestionsDto dto)
        {
            var tenantId = GetTenantId();

            var interview = await _interviewRepo
                .GetByIdAsync(interviewId, tenantId);

            if (interview == null)
                throw new Exception("Interview not found");

            if (dto.Questions == null || !dto.Questions.Any())
                throw new Exception("Questions required");

            await _questionRepo.AssignQuestionsAsync(
                interviewId,
                tenantId,
                dto.Questions);
        }
        private Guid GetUserId()
        {
            var claims = _httpContextAccessor.HttpContext?.User?.Claims;

            var userClaim = claims?
                .FirstOrDefault(x =>
                    x.Type == "sub" ||
                    x.Type == "uid" ||
                    x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (userClaim == null)
                throw new Exception("UserId not found in token");

            return Guid.Parse(userClaim.Value);
        }
    }
}