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

        public InterviewService(
            IInterviewRepository interviewRepo,
            IInterviewCandidateRepository candidateRepo,
            IInterviewInvitationRepository invitationRepo,
            IUserRepository userRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _interviewRepo = interviewRepo;
            _candidateRepo = candidateRepo;
            _invitationRepo = invitationRepo;
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        private long GetTenantId()
        {
            var tenantId = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("tid")?.Value;

            return string.IsNullOrEmpty(tenantId)
                ? 0
                : long.Parse(tenantId);
        }

        // CREATE INTERVIEW
        public async Task<long> CreateAsync(CreateInterviewDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new Exception("Interview title required");

            if (dto.DurationMinutes <= 0)
                throw new Exception("Invalid duration");

            var interview = new Interview
            {
                TenantId = GetTenantId(),
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
        public async Task AssignCandidateAsync(long interviewId, AssignCandidateDto dto)
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
                InterviewId = interviewId,
                CandidateId = dto.CandidateId,
                Status = (int)InterviewCandidateStatus.Invited,
                AssignedAt = DateTime.UtcNow
            };

            await _candidateRepo.AssignCandidateAsync(entity);
        }

        // SCHEDULE
        public async Task ScheduleAsync(long interviewId, ScheduleInterviewDto dto)
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
        public async Task<GenerateLinkResponse> GenerateLinkAsync(long interviewId, GenerateLinkDto dto)
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
    }
}