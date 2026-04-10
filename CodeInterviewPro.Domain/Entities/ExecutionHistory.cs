namespace CodeInterviewPro.Domain.Entities
{
    public class ExecutionHistory
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Language { get; set; }

        public int Total { get; set; }

        public int Passed { get; set; }

        public int Failed { get; set; }

        public int Score { get; set; }

        public double AIScore { get; set; }

        public string AIFeedback { get; set; }

        public string AIComplexity { get; set; }
        public double FinalScore { get; set; }

        public DateTime CreatedAt { get; set; }
      
    }
}