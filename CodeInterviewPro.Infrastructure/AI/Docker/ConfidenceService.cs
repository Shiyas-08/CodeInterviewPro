using CodeInterviewPro.Application.Interfaces.Services;

namespace CodeInterviewPro.Infrastructure.AI
{
    public class ConfidenceService : IConfidenceService
    {
        public double CalculateConfidence(
            double aiScore,
            double deepScore,
            double codeBertScore)
        {
            var aiWeight = 0.2;
            var deepWeight = 0.3;
            var codeBertWeight = 0.5;

            var confidence =
                (aiScore * aiWeight) +
                (deepScore * deepWeight) +
                (codeBertScore * codeBertWeight);

            return confidence;
        }
    }
}