using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeInterviewPro.Application.Interfaces.Services;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class ExecutionTimeoutService:IExecutionTimeoutService
    {
        public async Task<T> ExecuteWithTimeout<T>(
            Func<Task<T>> task,
            int timeoutSeconds)
        {
            using var cts =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(timeoutSeconds));

            var delayTask =
                Task.Delay(
                    Timeout.Infinite,
                    cts.Token);

            var executionTask =
                task();

            var completedTask =
                await Task.WhenAny(
                    executionTask,
                    delayTask);

            if (completedTask == delayTask)
            {
                throw new TimeoutException(
                    "Execution timeout exceeded");
            }

            return await executionTask;
        }
    }
}
