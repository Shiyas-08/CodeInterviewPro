using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalInterviews { get; set; }
        public int TotalCandidates { get; set; }
        public int CompletedInterviews { get; set; }
        public double AverageScore { get; set; }
        public double SuccessRate { get; set; }
    }
}
