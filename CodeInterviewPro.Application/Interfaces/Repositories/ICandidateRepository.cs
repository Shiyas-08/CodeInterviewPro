using CodeInterviewPro.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories
{
    public interface ICandidateRepository
    {
        Task<IEnumerable<CandidateInterviewDto>> GetMyInterviewsAsync(Guid candidateId);
    }
}
