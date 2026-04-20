using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly DapperContext _context;

        public CandidateRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CandidateInterviewDto>> GetMyInterviewsAsync(Guid candidateId)
        {
            var sql = @"
SELECT
    i.Id,
    i.Title,
    i.Description,
    i.StartTime,
    i.EndTime,
    i.DurationMinutes,
    ic.Status,
    inv.Token
FROM InterviewCandidates ic

INNER JOIN Interviews i
    ON ic.InterviewId = i.Id

LEFT JOIN InterviewInvitations inv
    ON inv.InterviewId = ic.InterviewId
   AND inv.CandidateId = ic.CandidateId

WHERE ic.CandidateId = @CandidateId
AND i.IsActive = 1

ORDER BY i.StartTime ASC";

            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<CandidateInterviewDto>(
                sql,
                new { CandidateId = candidateId });
        }
    }
}

