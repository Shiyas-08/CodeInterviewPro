using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs
{
    public class ResumeInterviewResponse
    {
        public Guid SessionId { get; set; }
        public Guid InterviewId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }
        public int RemainingSeconds { get; set; }

        public int Status { get; set; }

        public List<Question> Questions { get; set; } = new();
    }
}
