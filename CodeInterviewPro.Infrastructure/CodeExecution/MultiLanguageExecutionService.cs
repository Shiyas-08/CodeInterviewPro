using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.CodeExecution.Templates;
using CodeInterviewPro.Domain.Entities;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class MultiLanguageExecutionService
    {
        private readonly ILanguageExecutionFactory _factory;

        public MultiLanguageExecutionService(
            ILanguageExecutionFactory factory)
        {
            _factory = factory;
        }

        public async Task<string> ExecuteAsync(
            string code,
            ProgrammingLanguage language,
            List<TestCase> testCases,
            string methodName)
        {
            // FIX: pass enum directly
            var template =
                TemplateFactory.GetTemplate(language);

            var wrappedCode =
                template.WrapCode(
                    code,
                    testCases,
                    methodName);

            var executor =
                _factory.GetExecutor(language);

            return await executor.ExecuteAsync(
                wrappedCode,
                "");
        }
    }
}