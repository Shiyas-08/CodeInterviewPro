using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System.Data;

namespace CodeInterviewPro.Infrastructure.Repositories
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
        (
            Id,
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
            @Id,
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
    ";

            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync(sql, session);
        }
        public async Task<InterviewSession?> GetByTokenAsync(string token)
        {
            var sql = @"
                SELECT * 
                FROM InterviewSessions
                WHERE (Token = @Token OR Id = CAST(@Token AS UNIQUEIDENTIFIER))
                AND IsActive = 1
            ";

            using var connection = _db.CreateConnection();
            try
            {
                return await connection.QueryFirstOrDefaultAsync<InterviewSession>(
                    sql,
                    new { Token = token });
            }
            catch
            {
                // If CAST fails (not a GUID), just check Token
                var fallbackSql = "SELECT * FROM InterviewSessions WHERE Token = @Token AND IsActive = 1";
                return await connection.QueryFirstOrDefaultAsync<InterviewSession>(
                    fallbackSql,
                    new { Token = token });
            }
        }

        public async Task UpdateAsync(InterviewSession session)
        {
            var sql = @"
        UPDATE InterviewSessions
        SET
            StartTime = @StartTime,
            EndTime = @EndTime,
            RemainingSeconds = @RemainingSeconds,
            Status = @Status,
            IsActive = @IsActive
        WHERE Id = @Id
    ";

            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync(sql, session);
        }
    }
}