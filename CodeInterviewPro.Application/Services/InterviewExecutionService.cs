using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Common.Interfaces;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories;
using System.Text.Json;

namespace CodeInterviewPro.Application.Services
{
    public class InterviewExecutionService : IInterviewExecutionService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IInterviewInvitationRepository _invitationRepository;
        private readonly IInterviewQuestionRepository _questionRepository;
        private readonly IInterviewSubmissionRepository _submissionRepository;
        private readonly IInterviewSessionRepository _sessionRepository;
        private readonly IExecutionPipelineService _pipeline;
        private readonly IUserContext _userContext;
        private readonly IExecutionHistoryRepository _historyRepository;

        public InterviewExecutionService(
            IInterviewRepository interviewRepository,
            IInterviewInvitationRepository invitationRepository,
            IInterviewQuestionRepository questionRepository,
            IInterviewSubmissionRepository submissionRepository,
            IInterviewSessionRepository sessionRepository,
            IExecutionPipelineService pipeline,
            IUserContext userContext,IExecutionHistoryRepository historyRepository) 
        {
            _interviewRepository = interviewRepository;
            _invitationRepository = invitationRepository;
            _questionRepository = questionRepository;
            _submissionRepository = submissionRepository;
            _sessionRepository = sessionRepository;
            _pipeline = pipeline;
            _userContext = userContext;
            _historyRepository = historyRepository;
        }

        public async Task<StartInterviewResponse> StartInterviewAsync(string token)
        {
            var invitation = await _invitationRepository.GetByTokenAsync(token);

            if (invitation == null)
                throw new Exception("Invalid interview link");

            // Candidate validation
            var userId = _userContext.UserId;

            if (userId != invitation.CandidateId)
                throw new Exception("Unauthorized access");

            if (invitation.ExpiryTime < DateTime.UtcNow)
                throw new Exception("Interview link expired");

            if (invitation.IsUsed)
                throw new Exception("Interview already started");

            var interview = await _interviewRepository.GetByIdAsync(
                invitation.InterviewId,
                invitation.TenantId);

            if (interview == null)
                throw new Exception("Interview not found");

            // Time validation
            if (DateTime.UtcNow < interview.StartTime)
                throw new Exception("Interview not started yet");

            if (DateTime.UtcNow > interview.EndTime)
                throw new Exception("Interview expired");

            // mark invitation used
            invitation.IsUsed = true;
            invitation.StartedAt = DateTime.UtcNow;

            await _invitationRepository.UpdateAsync(invitation);

            // create session
            if (invitation.CandidateId == null)
                throw new Exception("Candidate not registered yet");

            var session = new InterviewSession
            {
                Id = Guid.NewGuid(),
                TenantId = invitation.TenantId,
                InterviewId = invitation.InterviewId,
                CandidateId = invitation.CandidateId.Value,
                Token = token,
                StartTime = DateTime.UtcNow,
                DurationMinutes = interview.DurationMinutes,
                RemainingSeconds = interview.DurationMinutes * 60,
                Status = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _sessionRepository.CreateAsync(session);

            var questions = await _questionRepository
                .GetByInterviewIdAsync(invitation.InterviewId);

            return new StartInterviewResponse
            {
                InterviewId = interview.Id,
                StartTime = DateTime.UtcNow,
                DurationMinutes = interview.DurationMinutes,
                Questions = questions.ToList()
            };
        }

        public async Task<ExecutionResult> SubmitCodeAsync(SubmitCodeRequest request)
        {
            var invitation =
                await _invitationRepository.GetByTokenAsync(request.Token);

            if (invitation == null)
                throw new Exception("Invalid token");

            var question =
                await _questionRepository.GetByIdAsync(
                    request.QuestionId,
                    invitation.TenantId);

            if (question == null)
                throw new Exception("Question not found");

            var testCases =
                System.Text.Json.JsonSerializer
                .Deserialize<List<TestCase>>(question.TestCases);

            var result =
                await _pipeline.ExecuteAsync(
                    request.Code,
                    question.Language,
                    testCases,
                    question.MethodName);
            var history = new ExecutionHistory
            {
                Id = Guid.NewGuid(),

                CandidateId = invitation.CandidateId.Value,
                InterviewId = invitation.InterviewId,
                QuestionId = request.QuestionId,
                Token = request.Token,

                Code = request.Code,
                Language = question.Language.ToString(),

                Total = testCases.Count,
                Passed = result.Passed,
                Failed = result.Failed,

                Score = (int)result.FinalScore,
                AIScore = result.AIScore,

                AIFeedback = result.AIFeedback,
                AIComplexity = result.AIComplexity,

                FinalScore = result.FinalScore,

                CreatedAt = DateTime.UtcNow
            };

            await _historyRepository.SaveAsync(history);

            var submission = new InterviewSubmission
            {
                InterviewId = invitation.InterviewId,
                CandidateId = invitation.CandidateId.Value,
                QuestionId = request.QuestionId,
                Language = question.Language.ToString(),
                Code = request.Code,
                SubmittedAt = DateTime.UtcNow,
                Score = result.FinalScore
            };

            await _submissionRepository.CreateAsync(submission);

            return result;
        }
        public async Task<ExecutionResult> RunCodeAsync(
    SubmitCodeRequest request)
        {
            var invitation =
                await _invitationRepository
                .GetByTokenAsync(request.Token);

            if (invitation == null)
                throw new Exception("Invalid token");

            var question =
                await _questionRepository.GetByIdAsync(
                    request.QuestionId,
                    invitation.TenantId);

            if (question == null)
                throw new Exception("Question not found");

            var testCases =
                JsonSerializer.Deserialize<List<TestCase>>(
                    question.TestCases);

            var result =
                await _pipeline.ExecuteAsync(
                    request.Code,
                    question.Language,
                    testCases,
                    question.MethodName);

            return result;
        }
    }
}
//using CodeInterviewPro.Application.DTOs.Interview;
//using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
//using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
//using CodeInterviewPro.Application.Interfaces.Services;
//using CodeInterviewPro.Domain.Common.Interfaces;
//using CodeInterviewPro.Domain.Entities;
//using CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories;

//namespace CodeInterviewPro.Application.Services
//{
//    public class InterviewExecutionService : IInterviewExecutionService
//    {
//        private readonly IInterviewRepository _interviewRepository;
//        private readonly IInterviewInvitationRepository _invitationRepository;
//        private readonly IInterviewQuestionRepository _questionRepository;
//        private readonly IInterviewSubmissionRepository _submissionRepository;
//        private readonly IInterviewSessionRepository _sessionRepository;
//        private readonly IExecutionPipelineService _pipeline;
//        private readonly IUserContext _userContext; // ✅ NEW

//        public InterviewExecutionService(
//            IInterviewRepository interviewRepository,
//            IInterviewInvitationRepository invitationRepository,
//            IInterviewQuestionRepository questionRepository,
//            IInterviewSubmissionRepository submissionRepository,
//            IInterviewSessionRepository sessionRepository,
//            IExecutionPipelineService pipeline,
//            IUserContext userContext) // ✅ NEW
//        {
//            _interviewRepository = interviewRepository;
//            _invitationRepository = invitationRepository;
//            _questionRepository = questionRepository;
//            _submissionRepository = submissionRepository;
//            _sessionRepository = sessionRepository;
//            _pipeline = pipeline;
//            _userContext = userContext; // ✅ NEW
//        }

//        public async Task<StartInterviewResponse> StartInterviewAsync(string token)
//        {
//            var invitation = await _invitationRepository.GetByTokenAsync(token);

//            if (invitation == null)
//                throw new Exception("Invalid interview link");

//            // ✅ Candidate validation
//            var userId = _userContext.UserId;

//            if (userId != invitation.CandidateId)
//                throw new Exception("Unauthorized access");

//            if (invitation.ExpiryTime < DateTime.UtcNow)
//                throw new Exception("Interview link expired");

//            if (invitation.IsUsed)
//                throw new Exception("Interview already started");

//            var interview = await _interviewRepository.GetByIdAsync(
//                invitation.InterviewId,
//                invitation.TenantId);

//            if (interview == null)
//                throw new Exception("Interview not found");

//            // ✅ Time validation
//            if (DateTime.UtcNow < interview.StartTime)
//                throw new Exception("Interview not started yet");

//            if (DateTime.UtcNow > interview.EndTime)
//                throw new Exception("Interview expired");

//            // mark invitation used
//            invitation.IsUsed = true;
//            invitation.StartedAt = DateTime.UtcNow;

//            await _invitationRepository.UpdateAsync(invitation);

//            // create session
//            if (invitation.CandidateId == null)
//                throw new Exception("Candidate not registered yet");

//            var session = new InterviewSession
//            {
//                TenantId = invitation.TenantId,
//                InterviewId = invitation.InterviewId,
//                CandidateId = invitation.CandidateId.Value, // ✅ FIX
//                Token = token,
//                StartTime = DateTime.UtcNow,
//                DurationMinutes = interview.DurationMinutes,
//                RemainingSeconds = interview.DurationMinutes * 60,
//                Status = 1,
//                IsActive = true,
//                CreatedAt = DateTime.UtcNow
//            };

//            await _sessionRepository.CreateAsync(session);

//            var questions = await _questionRepository
//                .GetByInterviewIdAsync(invitation.InterviewId);

//            return new StartInterviewResponse
//            {
//                InterviewId = interview.Id,
//                StartTime = DateTime.UtcNow,
//                DurationMinutes = interview.DurationMinutes,
//                Questions = questions.ToList()
//            };
//        }

//        public async Task<ExecutionResult> SubmitCodeAsync(SubmitCodeRequest request)
//        {
//            var invitation =
//                await _invitationRepository.GetByTokenAsync(request.Token);

//            if (invitation == null)
//                throw new Exception("Invalid token");

//            var question =
//                await _questionRepository.GetByIdAsync(
//                    request.QuestionId,
//                    invitation.TenantId);

//            if (question == null)
//                throw new Exception("Question not found");

//            var testCases =
//                System.Text.Json.JsonSerializer
//                .Deserialize<List<TestCase>>(question.TestCases);

//            var result =
//                await _pipeline.ExecuteAsync(
//                    request.Code,
//                    question.Language,
//                    testCases,
//                    question.MethodName);

//            var submission = new InterviewSubmission
//            {
//                InterviewId = invitation.InterviewId,
//                CandidateId = invitation.CandidateId.Value,
//                QuestionId = request.QuestionId,
//                Language = question.Language.ToString(),
//                Code = request.Code,
//                SubmittedAt = DateTime.UtcNow,
//                Score = result.FinalScore
//            };

//            await _submissionRepository.CreateAsync(submission);

//            return result;
//        }
//    }
//}
