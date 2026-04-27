using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Enums;
using System.Text.RegularExpressions;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class MethodNameDetectorService : CodeInterviewPro.Application.Interfaces.Services.IMethodNameDetectorService
    {
        public string Detect(string candidateCode, string? starterCode, ProgrammingLanguage language)
        {
            // 1. Try detect from starter code (Priority A)
            if (!string.IsNullOrWhiteSpace(starterCode))
            {
                var detected = DetectFromSingleCode(starterCode, language);
                if (!string.IsNullOrWhiteSpace(detected))
                    return detected;
            }

            // 2. Try detect from candidate code (Priority B)
            if (!string.IsNullOrWhiteSpace(candidateCode))
            {
                var detected = DetectFromSingleCode(candidateCode, language);
                if (!string.IsNullOrWhiteSpace(detected))
                    return detected;
            }

            // 3. Fallback (Priority C)
            return string.Empty;
        }

        private string DetectFromSingleCode(string code, ProgrammingLanguage language)
        {
            string pattern = language switch
            {
                ProgrammingLanguage.Python => @"def\s+([a-zA-Z_]\w*)\s*\(",
                
                ProgrammingLanguage.CSharp => @"public\s+(?:static\s+)?(?:\w+(?:<[^>]+>)?\s+)?([a-zA-Z_]\w*)\s*\(",
                
                ProgrammingLanguage.Java => @"public\s+(?:static\s+)?(?:\w+(?:<[^>]+>)?\s+)?([a-zA-Z_]\w*)\s*\(",
                
                ProgrammingLanguage.JavaScript => @"(?:function\s+([a-zA-Z_]\w*)\s*\(|const\s+([a-zA-Z_]\w*)\s*=\s*\(|let\s+([a-zA-Z_]\w*)\s*=\s*\()",
                
                ProgrammingLanguage.Go => @"func\s+(?:\([^)]+\)\s+)?([a-zA-Z_]\w*)\s*\(",
                
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(pattern)) return string.Empty;

            var match = Regex.Match(code, pattern);
            if (match.Success)
            {
                // Capture group 1 is usually the name
                // For JS, we might have multiple groups due to OR (|)
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    if (match.Groups[i].Success && !string.IsNullOrEmpty(match.Groups[i].Value))
                        return match.Groups[i].Value.Trim();
                }
            }

            return string.Empty;
        }
    }
}
