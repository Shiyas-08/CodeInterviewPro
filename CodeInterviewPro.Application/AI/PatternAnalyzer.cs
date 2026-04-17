using CodeInterviewPro.Domain.Entities;
using System.Text.RegularExpressions;

namespace CodeInterviewPro.Application.AI
{
    public class PatternAnalyzer
    {
        public PatternAnalysisResult Analyze(string code)
        {
            var result = new PatternAnalysisResult
            {
                UsesRecursion = DetectRecursion(code),
                UsesDictionary = DetectDictionary(code),
                UsesTwoPointer = DetectTwoPointer(code),
                UsesSlidingWindow = DetectSlidingWindow(code),
                UsesDynamicProgramming = DetectDynamicProgramming(code)
            };

            result.Score = CalculateScore(result);

            result.Feedback = GenerateFeedback(result);

            return result;
        }

        private bool DetectRecursion(string code)
        {
            var match =
                Regex.Match(code,
                @"(int|void|string|bool)\s+(\w+)\s*\(");

            if (!match.Success)
                return false;

            var name =
                match.Groups[2].Value;

            return code.Contains($"{name}(");
        }

        private bool DetectDictionary(string code)
        {
            return code.Contains("Dictionary") ||
                   code.Contains("Map") ||
                   code.Contains("HashMap");
        }

        private bool DetectTwoPointer(string code)
        {
            return code.Contains("left") &&
                   code.Contains("right");
        }

        private bool DetectSlidingWindow(string code)
        {
            return code.Contains("window") ||
                   code.Contains("start") &&
                   code.Contains("end");
        }

        private bool DetectDynamicProgramming(string code)
        {
            return code.Contains("dp") ||
                   code.Contains("memo");
        }

        private int CalculateScore(
            PatternAnalysisResult result)
        {
            var score = 100;

            if (result.UsesRecursion)
                score += 5;

            if (result.UsesDictionary)
                score += 10;

            if (result.UsesDynamicProgramming)
                score += 15;

            if (result.UsesTwoPointer)
                score += 5;

            return score;
        }

        private string GenerateFeedback(
            PatternAnalysisResult result)
        {
            var feedback = "";

            if (result.UsesRecursion)
                feedback += "Recursion pattern detected\n";

            if (result.UsesDictionary)
                feedback += "HashMap pattern detected\n";

            if (result.UsesDynamicProgramming)
                feedback += "Dynamic programming detected\n";

            if (result.UsesTwoPointer)
                feedback += "Two pointer pattern detected\n";

            if (result.UsesSlidingWindow)
                feedback += "Sliding window pattern detected\n";

            return feedback;
        }
    }

    
}