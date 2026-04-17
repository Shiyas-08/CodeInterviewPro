using CodeInterviewPro.Application.Interfaces.Services;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class ExecutionResourceService
        : IExecutionResourceService
    {
        private static readonly SemaphoreSlim _semaphore =
            new SemaphoreSlim(3);

        public async Task<T> ExecuteWithLimits<T>(
            Func<Task<T>> task,
            int memoryMb,
            int cpuLimit)
        {
            await _semaphore.WaitAsync();

            try
            {
                return await task();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}