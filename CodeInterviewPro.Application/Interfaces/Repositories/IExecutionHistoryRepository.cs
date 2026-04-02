using CodeInterviewPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Repositories
{
    public interface IExecutionHistoryRepository
    {
        Task SaveAsync(ExecutionHistory history);

    }
}
