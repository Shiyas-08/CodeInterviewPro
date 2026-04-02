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

        public double Score { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}