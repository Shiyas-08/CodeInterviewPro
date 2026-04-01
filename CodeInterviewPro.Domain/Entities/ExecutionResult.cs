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
    }
}
