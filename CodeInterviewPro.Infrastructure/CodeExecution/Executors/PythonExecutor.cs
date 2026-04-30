using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Application.Interfaces.Services;
using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution.Executors
{
    public class PythonExecutor : ILanguageExecutor
    {
        public async Task<string> ExecuteAsync(string code, string input)
        {
            var tempFolder =
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            var filePath =
                Path.Combine(tempFolder, "main.py");

            await File.WriteAllTextAsync(filePath, code);

            var process = new Process();

            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments =
                $"run --rm -v \"{tempFolder}:/app\" python:3.10 python /app/main.py";

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            var output =
                await process.StandardOutput.ReadToEndAsync();

            var error =
                await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                return !string.IsNullOrEmpty(error) ? error : "Process exited with code " + process.ExitCode;
            }

            return output + error;
        }
    }
}