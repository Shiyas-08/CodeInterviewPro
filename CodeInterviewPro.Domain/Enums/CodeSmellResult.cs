using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Domain.Enums
{
    public class CodeSmellResult
    {
        public bool HasLongMethod { get; set; }

        public bool HasTooManyParameters { get; set; }

        public bool HasDuplicateCode { get; set; }

        public bool HasPoorNaming { get; set; }

        public int Score { get; set; }

        public string Feedback { get; set; }
    }
}
