using CodeInterviewPro.Application.Interfaces.Services;
using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis
{
    public class GoStaticAnalyzer : IStaticCodeAnalyzer
    {
        public async Task<string> AnalyzeAsync(string code)
        {
            var tempFolder =
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            var filePath =
                Path.Combine(tempFolder, "main.go");

            // Wrap for static analysis only
            var wrappedCode = $@"
package main

{code}

func main() {{}}
";

            await File.WriteAllTextAsync(filePath, wrappedCode);

            var process = new Process();

            process.StartInfo.FileName = "docker";

            process.StartInfo.Arguments =
                $"run --rm " +
                $"-v \"{tempFolder}:/app\" " +
                $"-w /app " +
                $"golang:1.21 " +
                $"go build main.go";

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