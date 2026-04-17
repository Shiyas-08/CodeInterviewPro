using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class ExecutionResult
    {
        public int Total { get; set; }

        public int Passed { get; set; }

        public int Failed { get; set; }

        public double Score { get; set; }

        public double AIScore { get; set; }

        public string AIFeedback { get; set; }

        public string AIComplexity { get; set; }
        public double Similarity { get; set; }

        public string SimilarityMessage { get; set; }

        public double FinalScore { get; set; }
    }
}
