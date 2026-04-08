using CodeInterviewPro.Domain.Enums;
using System.Text.RegularExpressions;

namespace CodeInterviewPro.Application.AI
{
    public class CodeSmellAnalyzer
    {
        public CodeSmellResult Analyze(string code)
        {
            var result = new CodeSmellResult
            {
                HasLongMethod = DetectLongMethod(code),
                HasTooManyParameters = DetectTooManyParameters(code),
                HasDuplicateCode = DetectDuplicateCode(code),
                HasPoorNaming = DetectPoorNaming(code)
            };

            result.Score = CalculateScore(result);

            result.Feedback = GenerateFeedback(result);

            return result;
        }

        private bool DetectLongMethod(string code)
        {
            return code.Length > 800;
        }

        private bool DetectTooManyParameters(string code)
        {
            var match =
                Regex.Match(code, @"\((.*?)\)");

            if (!match.Success)
                return false;

            var parameters =
                match.Groups[1].Value
                    .Split(',');

            return parameters.Length >= 4;
        }

        private bool DetectDuplicateCode(string code)
        {
            var lines =
                code.Split('\n');

            return lines
                .GroupBy(x => x.Trim())
                .Any(g => g.Count() > 2);
        }

        private bool DetectPoorNaming(string code)
        {
            return Regex.IsMatch(
                code,
                @"\b[a-z]\b");
        }

        private int CalculateScore(CodeSmellResult result)
        {
            var score = 100;

            if (result.HasLongMethod)
                score -= 10;

            if (result.HasTooManyParameters)
                score -= 10;

            if (result.HasDuplicateCode)
                score -= 15;

            if (result.HasPoorNaming)
                score -= 5;

            return score;
        }

        private string GenerateFeedback(CodeSmellResult result)
        {
            var feedback = "";

            if (result.HasLongMethod)
                feedback += "Long method detected\n";

            if (result.HasTooManyParameters)
                feedback += "Too many parameters\n";

            if (result.HasDuplicateCode)
                feedback += "Duplicate code detected\n";

            if (result.HasPoorNaming)
                feedback += "Poor variable naming\n";

            return feedback;
        }
    }

    
}