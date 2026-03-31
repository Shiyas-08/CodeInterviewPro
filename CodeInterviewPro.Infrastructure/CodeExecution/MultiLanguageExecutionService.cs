using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Enums;

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
            ProgrammingLanguage language)
        {
            var executor =
                _factory.GetExecutor(language);

            return await executor.ExecuteAsync(code, "");
        }
    }
}