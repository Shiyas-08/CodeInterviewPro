using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;

namespace CodeInterviewPro.Infrastructure.Repositories.QuestionRepositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly DapperContext _context;

        public QuestionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(Question question)
        {
            var sql = @"INSERT INTO Questions
            (Id,TenantId,Title,Description,StarterCode,TestCases,
             TimeLimit,MemoryLimit,Language,IsActive,CreatedAt)
            VALUES
            (@Id,@TenantId,@Title,@Description,@StarterCode,@TestCases,
             @TimeLimit,@MemoryLimit,@Language,@IsActive,@CreatedAt)";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(sql, question);

            return question.Id;
        }

        public async Task<IEnumerable<Question>> GetAllAsync(Guid tenantId)
        {
            var sql = @"SELECT * 
                        FROM Questions 
                        WHERE TenantId=@TenantId 
                        AND IsActive=1";

            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Question>(sql,
                new { TenantId = tenantId });
        }

        public async Task<Question?> GetByIdAsync(Guid id, Guid tenantId)
        {
            var sql = @"SELECT * 
                        FROM Questions 
                        WHERE Id=@Id 
                        AND TenantId=@TenantId";

            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Question>(
                sql,
                new { Id = id, TenantId = tenantId });
        }

        public async Task UpdateAsync(Question question)
        {
            var sql = @"UPDATE Questions SET
                        Title=@Title,
                        Description=@Description,
                        StarterCode=@StarterCode,
                        TestCases=@TestCases,
                        Language=@Language
                        WHERE Id=@Id 
                        AND TenantId=@TenantId";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(sql, question);
        }

        public async Task DeleteAsync(Guid id, Guid tenantId)
        {
            var sql = @"UPDATE Questions
                        SET IsActive = 0
                        WHERE Id=@Id 
                        AND TenantId=@TenantId";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(sql,
                new { Id = id, TenantId = tenantId });
        }
        public async Task<IEnumerable<Question>> GetByInterviewIdAsync(Guid interviewId)
        {
            var sql = @"
        SELECT q.*
        FROM Questions q
        INNER JOIN InterviewQuestions iq
            ON q.Id = iq.QuestionId
        WHERE iq.InterviewId = @InterviewId
        AND q.IsActive = 1
        ORDER BY q.CreatedAt ASC
    ";

            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Question>(
                sql,
                new { InterviewId = interviewId });
        }
    }
}