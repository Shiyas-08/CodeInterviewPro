using CodeInterviewPro.Application.Interfaces.Services;
using System.Text;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class FinalFeedbackService : IFinalFeedbackService
    {
        public string Generate(
            string aiFeedback,
            string deepFeedback,
            string codeBertFeedback,
            string chatGptFeedback,
            double similarity,
            double score,
            string complexity)
        {
            var builder = new StringBuilder();

            builder.AppendLine("===== Candidate AI Evaluation Report =====");
            builder.AppendLine();

            builder.AppendLine("Overall Performance:");
            builder.AppendLine($"Candidate achieved a score of {score}.");
            builder.AppendLine();

            builder.AppendLine("Algorithm Analysis:");
            builder.AppendLine($"Detected Complexity: {complexity}");
            builder.AppendLine();

            builder.AppendLine("Technical Evaluation:");
            builder.AppendLine(aiFeedback);
            builder.AppendLine();

            builder.AppendLine("Deep Analysis:");
            builder.AppendLine(deepFeedback);
            builder.AppendLine();

            builder.AppendLine("Semantic Code Understanding:");
            builder.AppendLine(codeBertFeedback);
            builder.AppendLine();

            builder.AppendLine("AI Interview Feedback:");
            builder.AppendLine(chatGptFeedback);
            builder.AppendLine();

            builder.AppendLine("Code Similarity:");
            builder.AppendLine($"Similarity Percentage: {similarity}%");
            builder.AppendLine();

            if (similarity > 70)
            {
                builder.AppendLine("Warning: High similarity detected. Possible copied code.");
            }
            else
            {
                builder.AppendLine("Low similarity detected. Original implementation.");
            }

            builder.AppendLine();

            builder.AppendLine("Candidate Skill Assessment:");

            if (score >= 85)
            {
                builder.AppendLine("Strong problem-solving skills.");
                builder.AppendLine("Good algorithmic understanding.");
                builder.AppendLine("Clean coding practices observed.");
            }
            else if (score >= 65)
            {
                builder.AppendLine("Moderate problem-solving skills.");
                builder.AppendLine("Some improvements required.");
            }
            else
            {
                builder.AppendLine("Needs improvement in algorithmic thinking.");
                builder.AppendLine("Basic coding skills observed.");
            }

            builder.AppendLine();

            builder.AppendLine("Final Recommendation:");

            if (score >= 85)
            {
                builder.AppendLine("Strong Hire");
            }
            else if (score >= 65)
            {
                builder.AppendLine("Hire / Consider");
            }
            else
            {
                builder.AppendLine("Not Recommended");
            }

            builder.AppendLine();
            builder.AppendLine("===== End of AI Report =====");

            return builder.ToString();
        }
    }
}