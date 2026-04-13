using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IFinalFeedbackService
    {
        string Generate(
            string aiFeedback,
            string deepFeedback,
            string codeBertFeedback,
            string chatGptFeedback,
            double similarity,
            double score,
            string complexity);
    }
}