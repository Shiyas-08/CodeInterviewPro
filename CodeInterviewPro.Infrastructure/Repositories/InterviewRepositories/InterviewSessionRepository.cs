using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System.Data;

namespace CodeInterviewPro.Infrastructure.Repositories
{
    public class InterviewSessionRepository : IInterviewSessionRepository
    {
        private readonly IDbConnection _db;

        public InterviewSessionRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<long> CreateAsync(InterviewSession session)
        {
            var sql = @"
                INSERT INTO InterviewSessions
                (
                    TenantId,
                    InterviewId,
                    CandidateId,
                    Token,
                    StartTime,
                    DurationMinutes,
                    RemainingSeconds,
                    Status,
                    IsActive,
                    CreatedAt
                )
                VALUES
                (
                    @TenantId,
                    @InterviewId,
                    @CandidateId,
                    @Token,
                    @StartTime,
                    @DurationMinutes,
                    @RemainingSeconds,
                    @Status,
                    @IsActive,
                    @CreatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() as bigint);
            ";

            return await _db.ExecuteScalarAsync<long>(sql, session);
        }

        public async Task<InterviewSession?> GetByTokenAsync(string token)
        {
            var sql = @"
                SELECT * 
                FROM InterviewSessions
                WHERE Token = @Token
                AND IsActive = 1
            ";

            return await _db.QueryFirstOrDefaultAsync<InterviewSession>(
                sql,
                new { Token = token });
        }

        public async Task UpdateAsync(InterviewSession session)
        {
            var sql = @"
                UPDATE InterviewSessions
                SET
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    RemainingSeconds = @RemainingSeconds,
                    Status = @Status
                WHERE Id = @Id
            ";

            await _db.ExecuteAsync(sql, session);
        }
    }
}