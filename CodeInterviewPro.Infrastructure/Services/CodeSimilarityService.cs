using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Entities;
using System.Text.RegularExpressions;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class CodeSimilarityService : ICodeSimilarityService
    {
        private readonly IExecutionHistoryRepository _repository;

        public CodeSimilarityService(
            IExecutionHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CodeSimilarityResult> CheckSimilarityAsync(
            string code,
            string language)
        {
            var histories =
                await _repository.GetAllAsync();

            double maxSimilarity = 0;

            foreach (var history in histories)
            {
                if (history.Language != language)
                    continue;

                var similarity =
                    CalculateSimilarity(
                        code,
                        history.Code);

                if (similarity > maxSimilarity)
                    maxSimilarity = similarity;
            }

            return new CodeSimilarityResult
            {
                SimilarityPercentage = maxSimilarity,
                IsSimilar = maxSimilarity > 70,
                Message =
                    maxSimilarity > 70
                    ? "Possible plagiarism detected"
                    : "Code appears original"
            };
        }

        private double CalculateSimilarity(
            string codeA,
            string codeB)
        {
            codeA = Normalize(codeA);
            codeB = Normalize(codeB);

            var wordsA =
                codeA.Split(' ');

            var wordsB =
                codeB.Split(' ');

            var common =
                wordsA.Intersect(wordsB).Count();

            var total =
                Math.Max(wordsA.Length, wordsB.Length);

            return (double)common / total * 100;
        }

        private string Normalize(string code)
        {
            code = Regex.Replace(
                code,
                @"\s+",
                " ");

            return code.ToLower();
        }
    }
}