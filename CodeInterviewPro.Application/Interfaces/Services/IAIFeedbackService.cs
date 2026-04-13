namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IAIFeedbackService
    {
        Task<string> GenerateAsync(
            string question,
            string description,
            string code,
            string language,
            string complexity,
            double score);
    }
}