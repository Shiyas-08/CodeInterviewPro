using CodeInterviewPro.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Executors
{

    public class CSharpExecutor : ILanguageExecutor
    {
        private readonly DockerCodeExecutionService _dockerService;

        public CSharpExecutor(DockerCodeExecutionService dockerService)
        {
            _dockerService = dockerService;
        }

        public async Task<string> ExecuteAsync(string code, string input)
        {
            return await _dockerService.ExecuteAsync(code);
        }
    }
}
