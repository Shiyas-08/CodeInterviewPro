using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepo;
        private readonly IInterviewCandidateRepository _candidateRepo;
        private readonly IInterviewInvitationRepository _invitationRepo;
        private readonly IUserRepository _userRepo;

        public InterviewService(
            IInterviewRepository interviewRepo,
            IInterviewCandidateRepository candidateRepo,
            IInterviewInvitationRepository invitationRepo,
            IUserRepository userRepo)
        {
            _interviewRepo = interviewRepo;
            _candidateRepo = candidateRepo;
            _invitationRepo = invitationRepo;
            _userRepo = userRepo;
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
            // Interview validation
            var interview = await _interviewRepo.GetByIdAsync(interviewId, 0);

            if (interview == null)
                throw new Exception("Interview not found");

            if (interview.Status == (int)InterviewStatus.Completed)
                throw new Exception("Interview already completed");

            // Candidate validation
            var candidate = await _userRepo.GetById(dto.CandidateId);

            if (candidate == null)
                throw new Exception("Candidate not found");

            if (candidate.Role != UserRole.Candidate)
                throw new Exception("Invalid candidate");

            // Duplicate check
            var existing = await _candidateRepo.GetAsync(
                interviewId,
                dto.CandidateId,
                0);

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
            var interview = await _interviewRepo.GetByIdAsync(interviewId, 0);

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
        public async Task<string> GenerateLinkAsync(long interviewId, GenerateLinkDto dto)
        {
            var interview = await _interviewRepo.GetByIdAsync(interviewId, 0);

            if (interview == null)
                throw new Exception("Interview not found");

            // Candidate assigned check
            var assigned = await _candidateRepo.GetAsync(
                interviewId,
                dto.CandidateId,
                0);

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

            return token;
        }
    }
}