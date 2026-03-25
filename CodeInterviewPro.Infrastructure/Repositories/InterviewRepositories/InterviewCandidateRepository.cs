using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System.Data;

namespace CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories
{
    public class InterviewCandidateRepository : IInterviewCandidateRepository
    {
        private readonly DapperContext _db;

        public InterviewCandidateRepository(DapperContext db)
        {
            _db = db;
        }

        public async Task AssignCandidateAsync(InterviewCandidate candidate)
        {
            var sql = @"
                INSERT INTO InterviewCandidates
                (TenantId, InterviewId, CandidateId, Status, AssignedAt)
                VALUES
                (@TenantId, @InterviewId, @CandidateId, @Status, GETUTCDATE())
            ";
            using var connection = _db.CreateConnection();


            await connection.ExecuteAsync(sql, candidate);
        }

        public async Task<IEnumerable<InterviewCandidate>>
            GetByInterviewIdAsync(long interviewId, long tenantId)
        {
            var sql = @"
                SELECT *
                FROM InterviewCandidates
                WHERE InterviewId = @InterviewId
                AND TenantId = @TenantId
            ";
            using var connection = _db.CreateConnection();

            return await connection.QueryAsync<InterviewCandidate>(
                sql,
                new { InterviewId = interviewId, TenantId = tenantId });
        }

        public async Task<InterviewCandidate?>
            GetAsync(long interviewId, long candidateId, long tenantId)
        {
            var sql = @"
                SELECT *
                FROM InterviewCandidates
                WHERE InterviewId = @InterviewId
                AND CandidateId = @CandidateId
                AND TenantId = @TenantId
            ";
            using var connection = _db.CreateConnection();


            return await connection.QueryFirstOrDefaultAsync<InterviewCandidate>(
                sql,
                new
                {
                    InterviewId = interviewId,
                    CandidateId = candidateId,
                    TenantId = tenantId
                });
        }
    }
}