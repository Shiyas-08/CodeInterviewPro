using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Entities
{
    public class PatternAnalysisResult
    {
        public bool UsesRecursion { get; set; }

        public bool UsesDictionary { get; set; }

        public bool UsesTwoPointer { get; set; }

        public bool UsesSlidingWindow { get; set; }

        public bool UsesDynamicProgramming { get; set; }

        public int Score { get; set; }

        public string Feedback { get; set; }
    }
}
