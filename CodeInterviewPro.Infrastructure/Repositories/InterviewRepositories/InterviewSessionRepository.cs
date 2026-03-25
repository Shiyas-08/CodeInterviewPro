using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System.Data;

namespace CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories
{
    public class InterviewSessionRepository : IInterviewSessionRepository
    {
        private readonly DapperContext _db;

        public InterviewSessionRepository(DapperContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(InterviewSession session)
        {
            var sql = @"
                INSERT INTO InterviewSessions
                (TenantId, InterviewId, CandidateId, StartTime, Status)
                VALUES
                (@TenantId, @InterviewId, @CandidateId, @StartTime, @Status)
            ";
            using var connection = _db.CreateConnection();


            await connection.ExecuteAsync(sql, session);
        }

        public async Task<InterviewSession?>
            GetAsync(long interviewId, long candidateId, long tenantId)
        {
            var sql = @"
                SELECT *
                FROM InterviewSessions
                WHERE InterviewId = @InterviewId
                AND CandidateId = @CandidateId
                AND TenantId = @TenantId
            ";
            using var connection = _db.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<InterviewSession>(
                sql,
                new
                {
                    InterviewId = interviewId,
                    CandidateId = candidateId,
                    TenantId = tenantId
                });
        }

        public async Task UpdateAsync(InterviewSession session)
        {
            var sql = @"
                UPDATE InterviewSessions
                SET
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    Status = @Status
                WHERE Id = @Id
            ";
            using var connection = _db.CreateConnection();

            await connection.ExecuteAsync(sql, session);
        }
    }
}