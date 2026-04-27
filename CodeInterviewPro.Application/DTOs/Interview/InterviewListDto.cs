using System;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class InterviewListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Token { get; set; }
        public Guid? CandidateId { get; set; }
        public string? CandidateEmail { get; set; }
    }
}
