using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IExecutionTimeoutService
    {
        Task<T> ExecuteWithTimeout<T>(
            Func<Task<T>> task,
            int timeoutSeconds);
    }
}