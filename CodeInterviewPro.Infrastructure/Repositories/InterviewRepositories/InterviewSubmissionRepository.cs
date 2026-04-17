using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;

namespace CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories
{
    public class InterviewSubmissionRepository: IInterviewSubmissionRepository
    {
        private readonly DapperContext _context;

        public InterviewSubmissionRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<Guid> CreateAsync(InterviewSubmission submission)
        {
            var sql = @"
        DECLARE @NewId UNIQUEIDENTIFIER = NEWID();

        INSERT INTO InterviewSubmissions
        (
            Id,
            InterviewId,
            QuestionId,
            CandidateId,
            Language,
            Code,
            SubmittedAt
        )
        VALUES
        (
            @NewId,
            @InterviewId,
            @QuestionId,
            @CandidateId,
            @Language,
            @Code,
            @SubmittedAt
        );

        SELECT @NewId;
    ";

            using var connection = _context.CreateConnection();

            return await connection.ExecuteScalarAsync<Guid>(sql, submission);
        }

        //public async Task<long> CreateAsync(
        //    InterviewSubmission submission)
        //{

        //    var sql = @"
        //        INSERT INTO InterviewSubmissions
        //        (
        //            InterviewId,
        //            QuestionId,
        //            CandidateId,
        //            Language,
        //            Code,
        //            SubmittedAt
        //        )
        //        VALUES
        //        (
        //            @InterviewId,
        //            @QuestionId,
        //            @CandidateId,
        //            @Language,
        //            @Code,
        //            @SubmittedAt
        //        );

        //        SELECT CAST(SCOPE_IDENTITY() as BIGINT);
        //    ";

        //    using var connection =
        //        _context.CreateConnection();

        //    return await connection.ExecuteScalarAsync<long>(
        //        sql,
        //        submission);
        //}
        public async Task<IEnumerable<InterviewSubmission>> GetByCandidateAsync(Guid candidateId)
        {
            //    var sql = @"
            //SELECT *
            //FROM InterviewSubmissions
            //WHERE CandidateId = @CandidateId";
            var sql = @"
    SELECT *
    FROM (
        SELECT *,
               ROW_NUMBER() OVER (
                   PARTITION BY QuestionId
                   ORDER BY SubmittedAt DESC
               ) AS rn
        FROM InterviewSubmissions
        WHERE CandidateId = @CandidateId
    ) t
    WHERE t.rn = 1";

            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<InterviewSubmission>(
                sql,
                new { CandidateId = candidateId });
        }
    }
}