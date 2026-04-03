using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IExecutionResourceService
    {
        Task<T> ExecuteWithLimits<T>(
            Func<Task<T>> task,
            int memoryMb,
            int cpuLimit);
    }
}