using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System.Data;
using System.Data.Common;

namespace CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories
{
    public class InterviewInvitationRepository : IInterviewInvitationRepository
    {
        private readonly DapperContext _db;

        public InterviewInvitationRepository(DapperContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(InterviewInvitation invitation)
        {
            var sql = @"
INSERT INTO InterviewInvitations
(
    TenantId,
    InterviewId,
    CandidateId,
    CandidateEmail,
    Token,
    ExpiryTime,
    IsUsed
)
VALUES
(
    @TenantId,
    @InterviewId,
    @CandidateId,
    @CandidateEmail,
    @Token,
    @ExpiryTime,
    0
)";
            using var connection = _db.CreateConnection();

            await connection.ExecuteAsync(sql, invitation);
        }

        public async Task<InterviewInvitation?> GetByTokenAsync(string token)
        {
            var sql = @"
        SELECT *
        FROM InterviewInvitations
        WHERE Token = @Token
        AND ExpiryTime > GETUTCDATE()
    ";

            using var connection = _db.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<InterviewInvitation>(
                sql,
                new { Token = token });
        }

        public async Task MarkUsedAsync(long id)
        {
            var sql = @"
                UPDATE InterviewInvitations
                SET IsUsed = 1
                WHERE Id = @Id
            ";
            using var connection = _db.CreateConnection();


            await connection.ExecuteAsync(sql, new { Id = id });
        }
        public async Task UpdateAsync(InterviewInvitation invitation)
        {
            var sql = @"
        UPDATE InterviewInvitations
        SET IsUsed = @IsUsed,
            StartedAt = @StartedAt
        WHERE Id = @Id
    ";
            using var connection = _db.CreateConnection();

            await connection.ExecuteAsync(sql, invitation);
        }
        public async Task UpdateCandidateAsync(string token, Guid candidateId)
        {
            var sql = @"
        UPDATE InterviewInvitations
        SET CandidateId = @CandidateId
        WHERE Token = @Token
    ";

            using var connection = _db.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, new
            {
                CandidateId = candidateId,
                Token = token
            });

            Console.WriteLine($"Rows affected: {rows}");

            if (rows == 0)
                throw new Exception("❌ Candidate update failed");
        }

        public async Task<IEnumerable<CandidateInterviewDto>> GetByCandidateIdAsync(Guid candidateId)
        {
            var sql = @"
        SELECT
            inv.Id AS InvitationId,
            inv.Token,
            inv.IsUsed,
            inv.ExpiryTime,
            inv.StartedAt,
            inv.TenantId,

            i.Id AS InterviewId,
            i.Title,
            i.Description,
            i.DurationMinutes,
            i.StartTime,
            i.EndTime,

            CASE
                -- If Parent Interview is explicitly marked Completed or Cancelled
                WHEN i.Status = 4 THEN 'Completed'
                WHEN i.Status = 5 THEN 'Cancelled'

                -- If interview started AND still within duration → InProgress
                WHEN inv.IsUsed = 1 
                     AND inv.StartedAt IS NOT NULL
                     AND DATEADD(MINUTE, i.DurationMinutes, inv.StartedAt) > GETUTCDATE()
                    THEN 'InProgress'

                -- If not started and still valid → Pending
                WHEN inv.IsUsed = 0
                     AND inv.ExpiryTime > GETUTCDATE()
                    THEN 'Pending'

                -- If started but duration exceeded → Completed
                WHEN inv.IsUsed = 1 
                     AND inv.StartedAt IS NOT NULL
                     AND DATEADD(MINUTE, i.DurationMinutes, inv.StartedAt) <= GETUTCDATE()
                    THEN 'Completed'

                -- If never started and expired → Expired
                WHEN inv.ExpiryTime <= GETUTCDATE()
                    THEN 'Expired'

                ELSE 'Unknown'
            END AS Status

        FROM InterviewInvitations inv
        INNER JOIN Interviews i
            ON inv.InterviewId = i.Id

        WHERE inv.CandidateId = @CandidateId

        ORDER BY inv.Id DESC
    ";

            using var connection = _db.CreateConnection();

            return await connection.QueryAsync<CandidateInterviewDto>(
                sql,
                new { CandidateId = candidateId });
        }
        public async Task BindInvitesByEmail(string email, Guid userId)
        {
            var sql = @"
        UPDATE InterviewInvitations
        SET CandidateId = @UserId
        WHERE LOWER(LTRIM(RTRIM(CandidateEmail))) =
              LOWER(LTRIM(RTRIM(@Email)))
        AND CandidateId IS NULL
    ";

            using var connection = _db.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, new
            {
                Email = email,
                UserId = userId
            });

            Console.WriteLine($"Bound invites rows: {rows}");
        }
    }
}