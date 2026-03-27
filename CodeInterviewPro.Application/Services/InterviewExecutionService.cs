using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories;
namespace CodeInterviewPro.Application.Services
{
    public class InterviewExecutionService : IInterviewExecutionService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IInterviewInvitationRepository _invitationRepository;
        private readonly IInterviewQuestionRepository _questionRepository;
        private readonly IInterviewSubmissionRepository _submissionRepository;
        private readonly IInterviewSessionRepository _sessionRepository;
        public InterviewExecutionService(
            IInterviewRepository interviewRepository,
            IInterviewInvitationRepository invitationRepository,
            IInterviewQuestionRepository questionRepository,
            IInterviewSubmissionRepository submissionRepository,
            IInterviewSessionRepository sessionRepository)
        {
            _interviewRepository = interviewRepository;
            _invitationRepository = invitationRepository;
            _questionRepository = questionRepository;
            _submissionRepository = submissionRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<JoinInterviewResponse> JoinInterviewAsync(string token)
        {
            var invitation = await _invitationRepository.GetByTokenAsync(token);

            if (invitation == null)
                throw new Exception("Invalid interview link");

            if (invitation.ExpiryTime < DateTime.UtcNow)
                throw new Exception("Interview link expired");

            var interview = await _interviewRepository.GetByIdAsync(invitation.InterviewId,invitation.TenantId);

            return new JoinInterviewResponse
            {
                InterviewId = interview.Id,
                Title = interview.Title,
                StartTime = interview.StartTime,
                DurationMinutes = interview.DurationMinutes,
                Status = interview.Status.ToString()
            };
        }
        public async Task<StartInterviewResponse> StartInterviewAsync(string token)
        {
            var invitation = await _invitationRepository.GetByTokenAsync(token);

            if (invitation == null)
                throw new Exception("Invalid interview link");

            if (invitation.ExpiryTime < DateTime.UtcNow)
                throw new Exception("Interview link expired");

            if (invitation.IsUsed)
                throw new Exception("Interview already started");

            var interview = await _interviewRepository.GetByIdAsync(
                invitation.InterviewId,
                invitation.TenantId);

            invitation.IsUsed = true;
            invitation.StartedAt = DateTime.UtcNow;

            await _invitationRepository.UpdateAsync(invitation);

            var session = new InterviewSession
            {
                TenantId = invitation.TenantId,
                InterviewId = invitation.InterviewId,
                CandidateId = invitation.CandidateId,
                Token = token,
                StartTime = DateTime.UtcNow,
                DurationMinutes = interview.DurationMinutes,
                RemainingSeconds = interview.DurationMinutes * 60,
                Status = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _sessionRepository.CreateAsync(session);

            return new StartInterviewResponse
            {
                InterviewId = interview.Id,
                StartTime = DateTime.UtcNow,
                DurationMinutes = interview.DurationMinutes,
                Status = "Started"
            };
        }
        public async Task<List<GetQuestionsResponse>> GetQuestionsAsync(string token)
        {
            var invitation = await _invitationRepository.GetByTokenAsync(token);

            if (invitation == null)
                throw new Exception("Invalid interview link");

            var questions = await _questionRepository
                .GetByInterviewIdAsync(invitation.InterviewId);

            return questions.Select(x => new GetQuestionsResponse
            {
                QuestionId = x.QuestionId,
                Title = x.Title,
                Description = x.Description,
                Marks = x.Marks
            }).ToList();
        }
        public async Task<SubmitCodeResponse> SubmitCodeAsync(SubmitCodeRequest request)
        {
            var invitation = await
                _invitationRepository.GetByTokenAsync(request.Token);

            if (invitation == null)
                throw new Exception("Invalid token");

            var submission = new InterviewSubmission
            {
                InterviewId = invitation.InterviewId,
                CandidateId = invitation.CandidateId,
                QuestionId = request.QuestionId,
                Language = request.Language,
                Code = request.Code,
                SubmittedAt = DateTime.UtcNow
            };

            await _submissionRepository.CreateAsync(submission);

            return new SubmitCodeResponse
            {
                Success = true,
                Message = "Code submitted successfully"
            };
        }
    }
}