using CodeInterviewPro.Application.Interfaces.Services;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class ExecutionTimeoutService
        : IExecutionTimeoutService
    {
        public async Task<T> ExecuteWithTimeout<T>(
             Func<CancellationToken, Task<T>> task,
             int timeoutSeconds)
        {
            using var cts =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                return await task(cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException(
                    "Execution timeout exceeded");
            }
        }
    }
}