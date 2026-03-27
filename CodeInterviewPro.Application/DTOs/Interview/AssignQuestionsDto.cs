using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.DTOs.Interview
{
    public class AssignQuestionsDto
    {
        public List<QuestionItem> Questions { get; set; } = new();
    }

    public class QuestionItem
    {
        public long QuestionId { get; set; }

        public int Marks { get; set; }
    }
}
