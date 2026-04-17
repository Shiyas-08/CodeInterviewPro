using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class CodeSimilarityResult
    {
        public double SimilarityPercentage { get; set; }

        public bool IsSimilar { get; set; }

        public string Message { get; set; }
    }
}