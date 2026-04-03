using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeInterviewPro.Application.Interfaces.Services;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class ExecutionResourceService
        : IExecutionResourceService
    {
        public async Task<T> ExecuteWithLimits<T>(
            Func<Task<T>> task,
            int memoryMb,
            int cpuLimit)
        {
            try
            {
                var result = await task();

                return result;
            }
            catch (OutOfMemoryException)
            {
                throw new Exception(
                    "Memory limit exceeded");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Execution error: {ex.Message}");
            }
        }
    }
}
