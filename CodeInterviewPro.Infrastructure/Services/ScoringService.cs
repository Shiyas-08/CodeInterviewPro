using CodeInterviewPro.Application.Interfaces.Services;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class ScoringService : IScoringService
    {
        public double Calculate(
            double executionScore,
            double aiScore,
            double similarity)
        {
            var similarityPenalty =
                similarity > 70 ? 20 :
                similarity > 50 ? 10 :
                similarity > 30 ? 5 : 0;

            var finalScore =
                executionScore * 0.5 +
                aiScore * 0.4 -
                similarityPenalty;

            return finalScore;
        }
    }
}