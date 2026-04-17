using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Result
{
    public class InterviewResultDto
    {
        public int TotalScore { get; set; }

        public List<QuestionResultDto> Questions { get; set; }
    }

    public class QuestionResultDto
    {
        public Guid QuestionId { get; set; }

        public int Score { get; set; }

        public double AIScore { get; set; }

        public string Feedback { get; set; }

        public string Complexity { get; set; }
    }
}
