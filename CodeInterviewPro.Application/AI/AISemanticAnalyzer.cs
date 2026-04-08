using System.Text.RegularExpressions;

namespace CodeInterviewPro.Application.AI
{
    public class AISemanticAnalyzer
    {
        public SemanticAnalysisResult Analyze(string code)
        {
            var result = new SemanticAnalysisResult
            {
                Complexity = DetectComplexity(code),
                HasRecursion = DetectRecursion(code),
                HasNestedLoop = DetectNestedLoop(code),
                UsesDictionary = DetectDictionary(code),
                UsesLinq = DetectLinq(code)
            };

            result.Score = CalculateScore(result);
            result.Feedback = GenerateFeedback(result);

            return result;
        }

        private string DetectComplexity(string code)
        {
            var loops =
                Regex.Matches(code, @"for\s*\(").Count +
                Regex.Matches(code, @"while\s*\(").Count;

            if (loops >= 2)
                return "O(n^2)";

            if (loops == 1)
                return "O(n)";

            return "O(1)";
        }

        private bool DetectRecursion(string code)
        {
            var method =
                Regex.Match(code, @"(int|void|string|bool)\s+(\w+)\s*\(");

            if (!method.Success)
                return false;

            var name =
                method.Groups[2].Value;

            return code.Contains($"{name}(");
        }

        private bool DetectNestedLoop(string code)
        {
            var loops =
                Regex.Matches(code, @"for\s*\(").Count;

            return loops >= 2;
        }

        private bool DetectDictionary(string code)
        {
            return code.Contains("Dictionary");
        }

        private bool DetectLinq(string code)
        {
            return code.Contains(".Select(") ||
                   code.Contains(".Where(") ||
                   code.Contains(".OrderBy(");
        }

        private int CalculateScore(SemanticAnalysisResult result)
        {
            var score = 100;

            if (result.HasNestedLoop)
                score -= 15;

            if (result.HasRecursion)
                score += 5;

            if (result.UsesDictionary)
                score += 10;

            if (result.UsesLinq)
                score += 5;

            return score;
        }

        private string GenerateFeedback(SemanticAnalysisResult result)
        {
            var feedback = $"Complexity: {result.Complexity}\n";

            if (result.HasNestedLoop)
                feedback += "Nested loops detected\n";

            if (result.HasRecursion)
                feedback += "Recursion used\n";

            if (result.UsesDictionary)
                feedback += "Good use of Dictionary\n";

            if (result.UsesLinq)
                feedback += "LINQ usage detected\n";

            return feedback;
        }
    }

    public class SemanticAnalysisResult
    {
        public string Complexity { get; set; }

        public bool HasRecursion { get; set; }

        public bool HasNestedLoop { get; set; }

        public bool UsesDictionary { get; set; }

        public bool UsesLinq { get; set; }

        public int Score { get; set; }

        public string Feedback { get; set; }
    }
}