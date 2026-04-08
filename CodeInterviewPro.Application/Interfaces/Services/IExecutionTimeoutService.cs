using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeInterviewPro.Application.Interfaces.Services
{
    public interface IExecutionTimeoutService
    {
        Task<T> ExecuteWithTimeout<T>(
       Func<CancellationToken, Task<T>> task,
       int timeoutSeconds);
    }
}