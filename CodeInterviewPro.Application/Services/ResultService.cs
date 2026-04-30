using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Services
{
    using CodeInterviewPro.Application.DTOs.Result;
    using CodeInterviewPro.Application.Interfaces.Repositories;
    using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
    using CodeInterviewPro.Application.Interfaces.Services;
    using CodeInterviewPro.Domain.Common.Interfaces;

    public class ResultService : IResultService
    {
        private readonly IInterviewSubmissionRepository _submissionRepo;
        private readonly IExecutionHistoryRepository _historyRepo;
        private readonly IInterviewQuestionRepository _questionRepo;
        private readonly IUserContext _userContext;

        public ResultService(
            IInterviewSubmissionRepository submissionRepo,
            IExecutionHistoryRepository historyRepo,
            IInterviewQuestionRepository questionRepo,
            IUserContext userContext)
        {
            _submissionRepo = submissionRepo;
            _historyRepo = historyRepo;
            _questionRepo = questionRepo;
            _userContext = userContext;
        }

        public async Task<InterviewResultDto> GetMyResultAsync(Guid interviewId)
        {
            var candidateId = _userContext.UserId;
            return await GetCandidateResultAsync(candidateId, interviewId);
        }

        public async Task<InterviewResultDto> GetCandidateResultAsync(Guid candidateId, Guid interviewId)
        {
            var submissions = (await _submissionRepo.GetByInterviewAndCandidateAsync(interviewId, candidateId)).ToList();
            var histories = (await _historyRepo.GetByInterviewAndCandidateAsync(interviewId, candidateId)).ToList();
            var assignedQuestions = (await _questionRepo.GetByInterviewIdAsync(interviewId)).ToList();

            // latest execution per question
            var latestHistory = histories
                .GroupBy(x => x.QuestionId)
                .Select(g => g.OrderByDescending(x => x.CreatedAt).First())
                .ToList();

            // Use assigned questions as the base source to ensure all questions are listed
            var questions = assignedQuestions.Select(aq =>
            {
                var history = latestHistory.FirstOrDefault(h => h.QuestionId == aq.QuestionId);
                var submission = submissions.FirstOrDefault(s => s.QuestionId == aq.QuestionId);

                return new QuestionResultDto
                {
                    QuestionId = aq.QuestionId,
                    Score = (int)Math.Round(history?.AIScore ?? 0),
                    AIScore = history?.AIScore ?? 0,
                    Feedback = history?.AIFeedback ?? (submission != null ? "Awaiting AI evaluation..." : "No attempt made."),
                    Complexity = history?.AIComplexity ?? "N/A"
                };
            }).ToList();

            return new InterviewResultDto
            {
                TotalScore = questions.Count > 0 ? (int)questions.Average(x => x.Score) : 0,
                Questions = questions
            };
        }
    }
}
