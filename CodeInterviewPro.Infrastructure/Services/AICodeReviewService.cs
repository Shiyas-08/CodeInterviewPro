using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class AICodeReviewService : IAICodeReviewService
    {
        public async Task<AICodeReviewResult> ReviewAsync(
            string code,
            ProgrammingLanguage language)
        {
            var result = new AICodeReviewResult();

            // Simple mock AI logic (Phase 1)
            result.Score = CalculateScore(code);

            result.Complexity = DetectComplexity(code);

            result.Feedback = GenerateFeedback(code);

            result.Suggestions = GenerateSuggestions(code);

            result.Issues = DetectIssues(code);

            return await Task.FromResult(result);
        }

        private int CalculateScore(string code)
        {
            var score = 100;

            if (code.Length > 500)
                score -= 10;

            if (code.Contains("for") &&
                code.Contains("for"))
                score -= 10;

            return score;
        }

        private string DetectComplexity(string code)
        {
            if (code.Contains("for") &&
                code.Contains("for"))
                return "O(n²)";

            if (code.Contains("for"))
                return "O(n)";

            return "O(1)";
        }

        private string GenerateFeedback(string code)
        {
            return "Code reviewed successfully. Consider improving readability.";
        }

        private List<string> GenerateSuggestions(string code)
        {
            return new List<string>
            {
                "Improve variable naming",
                "Reduce nested loops",
                "Add comments"
            };
        }

        private List<string> DetectIssues(string code)
        {
            return new List<string>
            {
                "Long method detected",
                "Possible nested loops"
            };
        }
    }
}
