using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;

public class ResultPdfService : IResultPdfService
{
    private readonly IInterviewSubmissionRepository _submissionRepo;
    private readonly IExecutionHistoryRepository _historyRepo;


    public ResultPdfService(
        IInterviewSubmissionRepository submissionRepo,
        IExecutionHistoryRepository historyRepo)
    {
        _submissionRepo = submissionRepo;
        _historyRepo = historyRepo;
    }

    public async Task<byte[]> GenerateAsync(Guid candidateId)
    {
        // 🥇 STEP 2 — FETCH DATA
        var submissions = await _submissionRepo.GetByCandidateAsync(candidateId);
        var histories = await _historyRepo.GetByCandidateAsync(candidateId);

        // 🥈 STEP 3 — LATEST ATTEMPT PER QUESTION
        var latestByQuestion = histories
            .GroupBy(x => x.QuestionId)
            .Select(g => g.OrderByDescending(x => x.CreatedAt).First())
            .ToList();

        // 🥉 STEP 4 — MERGE DATA
        var merged = submissions.Select(sub =>
        {
            var history = latestByQuestion
                .FirstOrDefault(h => h.QuestionId == sub.QuestionId);

            return new
            {
                sub.QuestionId,
                FinalScore = history?.FinalScore ?? 0,  // Score lives in ExecutionHistory, not InterviewSubmission

                Passed = history?.Passed ?? 0,
                Failed = history?.Failed ?? 0,
                AIScore = history?.AIScore ?? 0,
                Feedback = history?.AIFeedback ?? "N/A",
                Complexity = history?.AIComplexity ?? "N/A"
            };
        }).ToList();

        // 🧾 BUILD PDF CONTENT (SIMPLE VERSION)
        var totalScore = merged.Sum(x => x.FinalScore);

        var content = $@"
==============================
INTERVIEW REPORT
==============================

Candidate: {candidateId}
Total Score: {totalScore}

==============================
QUESTION DETAILS
==============================
";

        foreach (var q in merged)
        {
            content += $@"

Question: {q.QuestionId}
Score: {q.FinalScore}
Passed: {q.Passed}
Failed: {q.Failed}
AI Score: {q.AIScore}
Complexity: {q.Complexity}
Feedback: {q.Feedback}

------------------------------
";
        }

        // RETURN AS PDF (TEMP TEXT → PDF)
        return System.Text.Encoding.UTF8.GetBytes(content);
    }
}