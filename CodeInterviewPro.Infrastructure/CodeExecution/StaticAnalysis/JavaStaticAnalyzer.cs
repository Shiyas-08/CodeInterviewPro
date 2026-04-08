using CodeInterviewPro.Application.Interfaces.Services;
using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis
{
    public class JavaStaticAnalyzer : IStaticCodeAnalyzer
    {
        public async Task<string> AnalyzeAsync(string code)
        {
            var tempFolder =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            var filePath =
                Path.Combine(tempFolder, "Main.java");

            // FIX: Wrap inside class
            var wrappedCode = $@"
public class Main
{{
    {code}
}}
";

            await File.WriteAllTextAsync(filePath, wrappedCode);

            var process = new Process();

            process.StartInfo.FileName = "docker";

            process.StartInfo.Arguments =
                $"run --rm " +
                $"--network none " +
                $"-v \"{tempFolder}:/app\" " +
                $"-w /app " +
                $"eclipse-temurin:17 " +
                $"javac Main.java";

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            var output =
                await process.StandardOutput.ReadToEndAsync();

            var error =
                await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            return error + output;
        }
    }
}