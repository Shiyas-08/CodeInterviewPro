using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories
    {
        public class InterviewQuestionRepository : IInterviewQuestionRepository
    {
            private readonly DapperContext _context;

            public InterviewQuestionRepository(DapperContext context)
            {
                _context = context;
            }
        public async Task<IEnumerable<InterviewQuestionDto>>
    GetByInterviewIdAsync(long interviewId)
        {
            var query = @"
        SELECT 
            iq.QuestionId,
            q.Title,
            q.Description,
            iq.Marks
        FROM InterviewQuestions iq
        INNER JOIN Questions q 
            ON iq.QuestionId = q.Id
        WHERE iq.InterviewId = @InterviewId
    ";

            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<InterviewQuestionDto>(
                query,
                new { InterviewId = interviewId });
        }
        public async Task AssignQuestionsAsync( long interviewId,Guid tenantId,List<QuestionItem> questions)
        {
            var sql = @"
        INSERT INTO InterviewQuestions
        (TenantId, InterviewId, QuestionId, Marks)
        VALUES
        (@TenantId, @InterviewId, @QuestionId, @Marks)";

            using var connection = _context.CreateConnection();

            foreach (var q in questions)
            {
                await connection.ExecuteAsync(sql, new
                {
                    TenantId = tenantId,
                    InterviewId = interviewId,
                    QuestionId = q.QuestionId,
                    Marks = q.Marks
                });
            }
        }

    }
    }

