using System;
using System.Collections.Generic;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class StartInterviewResponse
    {
        public Guid SessionId { get; set; }
        public Guid InterviewId { get; set; }

        public DateTime StartTime { get; set; }

        public int DurationMinutes { get; set; }

        public List<InterviewQuestionDto> Questions { get; set; }
            = new();
    }
}