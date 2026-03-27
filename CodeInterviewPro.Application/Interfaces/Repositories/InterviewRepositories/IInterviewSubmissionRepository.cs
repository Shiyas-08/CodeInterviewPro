using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories
{
    public interface IInterviewSubmissionRepository
    {
        Task<long> CreateAsync(InterviewSubmission submission);

    }
}
