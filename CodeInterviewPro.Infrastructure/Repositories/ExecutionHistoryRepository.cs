using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System.Data;

namespace CodeInterviewPro.Infrastructure.Repositories
{
    public class ExecutionHistoryRepository : IExecutionHistoryRepository
    {
        private readonly IDbConnection _connection;

        public ExecutionHistoryRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task SaveAsync(ExecutionHistory history)
        {
            var sql = @"
                INSERT INTO ExecutionHistory
                (
                    Id,
                    CandidateId,
                    InterviewId,
                    QuestionId,
                    Token,
                    Code,
                    Language,
                    Total,
                    Passed,
                    Failed,
                    Score,
                    AIScore,
                    AIFeedback,
                    AIComplexity,
                    FinalScore,
                    CreatedAt
                )
                VALUES
                (
                    @Id,
                    @CandidateId,
                    @InterviewId,
                    @QuestionId, 
                    @Token,
                    @Code,
                    @Language,
                    @Total,
                    @Passed,
                    @Failed,
                    @Score,
                    @AIScore,
                    @AIFeedback,
                    @AIComplexity,
                    @FinalScore,
                    @CreatedAt
                )";

            await _connection.ExecuteAsync(sql, history);
        }

        public async Task<IEnumerable<ExecutionHistory>> GetByCandidateAsync(Guid candidateId)
        {
            var sql = @"
                SELECT *
                FROM ExecutionHistory
                WHERE CandidateId = @CandidateId
                ORDER BY CreatedAt DESC";

            return await _connection.QueryAsync<ExecutionHistory>(
                sql,
                new { CandidateId = candidateId });
        }

        public async Task<IEnumerable<ExecutionHistory>> GetByInterviewAsync(Guid interviewId)
        {
            var sql = @"
                SELECT *
                FROM ExecutionHistory
                WHERE InterviewId = @InterviewId";

            return await _connection.QueryAsync<ExecutionHistory>(
                sql,
                new { InterviewId = interviewId });
        }

        public async Task<IEnumerable<ExecutionHistory>> GetByInterviewAndCandidateAsync(Guid interviewId, Guid candidateId)
        {
            var sql = @"
                SELECT *
                FROM ExecutionHistory
                WHERE InterviewId = @InterviewId AND CandidateId = @CandidateId
                ORDER BY CreatedAt DESC";

            return await _connection.QueryAsync<ExecutionHistory>(
                sql,
                new { InterviewId = interviewId, CandidateId = candidateId });
        }
        public async Task<IEnumerable<ExecutionHistory>> GetAllAsync()
        {
            var sql =
                "SELECT * FROM ExecutionHistory";

            return await _connection.QueryAsync<ExecutionHistory>(sql);
        }
    }
}

//using CodeInterviewPro.Application.Interfaces.Repositories;
//using CodeInterviewPro.Domain.Entities;
//using Dapper;
//using System.Data;

//namespace CodeInterviewPro.Infrastructure.Repositories
//{
//    public class ExecutionHistoryRepository : IExecutionHistoryRepository
//    {
//        private readonly IDbConnection _connection;

//        public ExecutionHistoryRepository(
//            IDbConnection connection)
//        {
//            _connection = connection;
//        }

//        public async Task SaveAsync(ExecutionHistory history)
//        {
//            var sql = @"
//                INSERT INTO ExecutionHistory
//                (
//                    Id,
//                    Code,
//                    Language,
//                    Total,
//                    Passed,
//                    Failed,
//                    Score,
//                    AIScore,
//                    AIFeedback,
//                    AIComplexity,
//                    CreatedAt
//                )
//                VALUES
//                (
//                    @Id,
//                    @Code,
//                    @Language,
//                    @Total,
//                    @Passed,
//                    @Failed,
//                    @Score,
//                    @AIScore,
//                    @AIFeedback,
//                    @AIComplexity,
//                    @CreatedAt
//                )";

//            await _connection.ExecuteAsync(sql, history);
//        }
//        public async Task<IEnumerable<ExecutionHistory>> GetAllAsync()
//        {
//            var sql =
//                "SELECT * FROM ExecutionHistory";

//            return await _connection.QueryAsync<ExecutionHistory>(sql);
//        }
//    }
//}