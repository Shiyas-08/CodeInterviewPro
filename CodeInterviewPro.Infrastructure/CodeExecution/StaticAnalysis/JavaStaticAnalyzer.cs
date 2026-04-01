using CodeInterviewPro.Application.Interfaces.Services;
using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis
{
    public class JavaStaticAnalyzer : IStaticCodeAnalyzer
    {
        public async Task<string> AnalyzeAsync(string code)
        {
            var tempFolder =
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            var filePath =
                Path.Combine(tempFolder, "Main.java");

            await File.WriteAllTextAsync(filePath, code);

            var process = new Process();

            process.StartInfo.FileName = "javac";
            process.StartInfo.Arguments = filePath;

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