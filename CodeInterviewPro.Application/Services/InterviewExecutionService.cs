using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories;
namespace CodeInterviewPro.Application.Services
{
    public class InterviewExecutionService : IInterviewExecutionService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IInterviewInvitationRepository _invitationRepository;
        private readonly IInterviewQuestionRepository _questionRepository;
        public InterviewExecutionService(
            IInterviewRepository interviewRepository,
            IInterviewInvitationRepository invitationRepository,
            IInterviewQuestionRepository questionRepository)
        {
            _interviewRepository = interviewRepository;
            _invitationRepository = invitationRepository;
            _questionRepository = questionRepository;
        }

        public async Task<JoinInterviewResponse> JoinInterviewAsync(string token)
        {
            var invitation = await _invitationRepository.GetByTokenAsync(token);

            if (invitation == null)
                throw new Exception("Invalid interview link");

            if (invitation.ExpiryTime < DateTime.UtcNow)
                throw new Exception("Interview link expired");

            var interview = await _interviewRepository.GetByIdAsync(
                invitation.InterviewId,
                0);

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
                0);

            invitation.IsUsed = true;
            invitation.StartedAt = DateTime.UtcNow;

            await _invitationRepository.UpdateAsync(invitation);

            return new StartInterviewResponse
            {
                InterviewId = interview.Id,
                StartTime = invitation.StartedAt.Value,
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
    }
}