using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories
{
    public interface IInterviewQuestionRepository
    {
        Task<IEnumerable<InterviewQuestionDto>> GetByInterviewIdAsync(Guid interviewId);

        Task AssignQuestionsAsync(
            Guid interviewId,
            Guid tenantId,
            List<QuestionItem> questions);
        Task<Question?> GetByIdAsync(Guid questionId, Guid tenantId);
    }


}
