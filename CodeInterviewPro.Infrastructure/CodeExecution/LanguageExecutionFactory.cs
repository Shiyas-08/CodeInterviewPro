using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.CodeExecution.Executors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class LanguageExecutionFactory : ILanguageExecutionFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public LanguageExecutionFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public ILanguageExecutor GetExecutor(
            ProgrammingLanguage language)
        {
            return language switch
            {
                ProgrammingLanguage.CSharp =>
                    _serviceProvider.GetRequiredService<CSharpExecutor>(),

                ProgrammingLanguage.Python =>
                    _serviceProvider.GetRequiredService<PythonExecutor>(),

                ProgrammingLanguage.JavaScript =>
                    _serviceProvider.GetRequiredService<NodeExecutor>(),

                ProgrammingLanguage.Java =>
                    _serviceProvider.GetRequiredService<JavaExecutor>(),

                ProgrammingLanguage.Go =>
                    _serviceProvider.GetRequiredService<GoExecutor>(),

                _ => throw new NotSupportedException()
            };
        }
    }
}