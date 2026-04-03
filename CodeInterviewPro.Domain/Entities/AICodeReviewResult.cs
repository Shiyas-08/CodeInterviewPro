using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CodeInterviewPro.Domain.Entities
{
    public class AICodeReviewResult
    {
        public int Score { get; set; }

        public string Complexity { get; set; }

        public string Feedback { get; set; }

        public List<string> Suggestions { get; set; }
            = new List<string>();

        public List<string> Issues { get; set; }
            = new List<string>();

    }
}
