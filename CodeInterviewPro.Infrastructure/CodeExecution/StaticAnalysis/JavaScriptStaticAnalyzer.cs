using CodeInterviewPro.Application.Interfaces.Services;
using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis
{
    public class JavaScriptStaticAnalyzer : IStaticCodeAnalyzer
    {
        public async Task<string> AnalyzeAsync(string code)
        {
            var tempFile =
                Path.GetTempFileName() + ".js";

            await File.WriteAllTextAsync(tempFile, code);

            var process = new Process();

            process.StartInfo.FileName = "node";
            process.StartInfo.Arguments =
                $"--check {tempFile}";

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            var error =
                await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            return error;
        }
    }
}