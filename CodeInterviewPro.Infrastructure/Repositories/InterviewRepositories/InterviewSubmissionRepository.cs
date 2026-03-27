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

        public async Task<long> CreateAsync(
            InterviewSubmission submission)
        {
            var sql = @"
                INSERT INTO InterviewSubmissions
                (
                    InterviewId,
                    QuestionId,
                    CandidateId,
                    Language,
                    Code,
                    SubmittedAt
                )
                VALUES
                (
                    @InterviewId,
                    @QuestionId,
                    @CandidateId,
                    @Language,
                    @Code,
                    @SubmittedAt
                );

                SELECT CAST(SCOPE_IDENTITY() as BIGINT);
            ";

            using var connection =
                _context.CreateConnection();

            return await connection.ExecuteScalarAsync<long>(
                sql,
                submission);
        }
    }
}