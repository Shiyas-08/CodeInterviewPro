using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CodeInterviewPro.Domain.Entities
{
    public class TestCaseResult
    {
        public bool Passed { get; set; }

        public string Output { get; set; }

        public string Expected { get; set; }
    }
}
