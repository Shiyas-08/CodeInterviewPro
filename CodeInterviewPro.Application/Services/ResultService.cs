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
        private readonly IUserContext _userContext;

        public ResultService(
            IInterviewSubmissionRepository submissionRepo,
            IExecutionHistoryRepository historyRepo,
            IUserContext userContext)
        {
            _submissionRepo = submissionRepo;
            _historyRepo = historyRepo;
            _userContext = userContext;
        }

        public async Task<InterviewResultDto> GetMyResultAsync()
        {
            var candidateId = _userContext.UserId;

            var submissions = await _submissionRepo.GetByCandidateAsync(candidateId);
            var histories = await _historyRepo.GetByCandidateAsync(candidateId);

            // latest execution per question
            var latestHistory = histories
                .GroupBy(x => x.QuestionId)
                .Select(g => g.OrderByDescending(x => x.CreatedAt).First())
                .ToList();

            var questions = submissions.Select(sub =>
            {
                var history = latestHistory
                    .FirstOrDefault(h => h.QuestionId == sub.QuestionId);

                return new QuestionResultDto
                {
                    QuestionId = sub.QuestionId,
                    Score =(int) Math.Round(history?.AIScore ?? 0),
                    AIScore = history?.AIScore ?? 0,
                    Feedback = history?.AIFeedback ?? "N/A",
                    Complexity = history?.AIComplexity ?? "N/A"
                };
            }).ToList();

            return new InterviewResultDto
            {
                TotalScore = questions.Sum(x => x.Score),
                Questions = questions
            };
        }
    }
}
