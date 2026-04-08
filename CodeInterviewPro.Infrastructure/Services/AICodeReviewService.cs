using System.Text.RegularExpressions;
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

            result.Complexity = DetectComplexity(code);

            result.Score = CalculateScore(code);

            result.Feedback = GenerateFeedback(code);

            result.Suggestions = GenerateSuggestions(code);

            result.Issues = DetectIssues(code);

            return await Task.FromResult(result);
        }

        private int CalculateScore(string code)
        {
            var score = 100;

            if (DetectNestedLoop(code))
                score -= 20;

            if (DetectRecursion(code))
                score += 5;

            if (DetectDictionary(code))
                score += 10;

            if (DetectLongMethod(code))
                score -= 10;

            return score;
        }

        private string DetectComplexity(string code)
        {
            var loopCount =
                Regex.Matches(code, @"for\s*\(").Count +
                Regex.Matches(code, @"while\s*\(").Count;

            if (loopCount >= 2)
                return "O(n^2)";

            if (loopCount == 1)
                return "O(n)";

            return "O(1)";
        }

        private bool DetectNestedLoop(string code)
        {
            var loopCount =
                Regex.Matches(code, @"for\s*\(").Count;

            return loopCount >= 2;
        }

        private bool DetectRecursion(string code)
        {
            var method =
                Regex.Match(code, @"int\s+(\w+)\s*\(");

            if (!method.Success)
                return false;

            var name =
                method.Groups[1].Value;

            return code.Contains($"{name}(");
        }

        private bool DetectDictionary(string code)
        {
            return code.Contains("Dictionary");
        }

        private bool DetectLongMethod(string code)
        {
            return code.Length > 800;
        }

        private string GenerateFeedback(string code)
        {
            var feedback = "AI Code Review:\n";

            if (DetectNestedLoop(code))
                feedback += "Nested loops detected\n";

            if (DetectRecursion(code))
                feedback += "Recursion detected\n";

            if (DetectDictionary(code))
                feedback += "Good use of Dictionary\n";

            return feedback;
        }

        private List<string> GenerateSuggestions(string code)
        {
            var suggestions =
                new List<string>();

            if (DetectNestedLoop(code))
                suggestions.Add(
                    "Consider optimizing nested loops");

            if (!DetectDictionary(code))
                suggestions.Add(
                    "Consider using Dictionary for lookup");

            return suggestions;
        }

        private List<string> DetectIssues(string code)
        {
            var issues =
                new List<string>();

            if (DetectLongMethod(code))
                issues.Add("Long method detected");

            if (DetectNestedLoop(code))
                issues.Add("Nested loop detected");

            return issues;
        }
    }
}