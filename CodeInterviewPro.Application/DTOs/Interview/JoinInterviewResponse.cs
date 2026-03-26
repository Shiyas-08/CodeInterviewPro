using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class JoinInterviewResponse
    {
        public long InterviewId { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime? StartTime { get; set; }

        public int DurationMinutes { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
