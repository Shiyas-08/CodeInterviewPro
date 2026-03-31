using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Application.Interfaces.Services;
using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Executors
{
    public class GoExecutor : ILanguageExecutor
    {
        public async Task<string> ExecuteAsync(string code, string input)
        {
            var tempFolder =
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            var filePath =
                Path.Combine(tempFolder, "main.go");

            var fullCode = $@"
package main
import ""fmt""

func main() {{
    {code}
}}
";

            await File.WriteAllTextAsync(filePath, fullCode);

            var process = new Process();

            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments =
                $"run --rm -v \"{tempFolder}:/app\" golang:1.21 go run /app/main.go";

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            var output =
                await process.StandardOutput.ReadToEndAsync();

            var error =
                await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrEmpty(error))
                return error;

            return output;
        }
    }
}