using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
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
                (TenantId, InterviewId, CandidateId, Token, ExpiryTime, IsUsed)
                VALUES
                (@TenantId, @InterviewId, @CandidateId, @Token, @ExpiryTime, 0)
            ";
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
    }
}