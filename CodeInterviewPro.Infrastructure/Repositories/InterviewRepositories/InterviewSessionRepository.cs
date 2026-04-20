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

        // CREATE SESSION
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
)";

            await _db.ExecuteAsync(sql, session);
        }

        // BASIC SESSION
        public async Task<InterviewSession?> GetByTokenAsync(string token)
        {
            var sql = @"
SELECT *
FROM InterviewSessions
WHERE Token = @Token
AND IsActive = 1";

            return await _db.QueryFirstOrDefaultAsync<InterviewSession>(
                sql,
                new { Token = token });
        }

        // UPDATE SESSION
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
WHERE Id = @Id";

            await _db.ExecuteAsync(sql, session);
        }

        // FULL SESSION DETAILS WITH QUESTIONS
        public async Task<object?> GetSessionDetailsAsync(string token)
        {
            var sql = @"
SELECT
    s.InterviewId,
    s.RemainingSeconds,
    s.DurationMinutes,

    q.Id           AS QuestionId,
    q.Title,
    q.Description,
    iq.Marks

FROM InterviewSessions s

INNER JOIN InterviewQuestions iq
    ON s.InterviewId = iq.InterviewId

INNER JOIN Questions q
    ON iq.QuestionId = q.Id

WHERE s.Token = @Token
AND s.IsActive = 1
AND q.IsActive = 1";

            var rows = (await _db.QueryAsync(sql, new
            {
                Token = token
            })).ToList();

            if (!rows.Any())
                return null;

            var first = rows.First();

            return new
            {
                InterviewId = first.InterviewId,
                RemainingSeconds = first.RemainingSeconds,
                DurationMinutes = first.DurationMinutes,

                Questions = rows.Select(x => new
                {
                    QuestionId = x.QuestionId,
                    Title = x.Title,
                    Description = x.Description,
                    Marks = x.Marks
                }).ToList()
            };
        }
    }
}