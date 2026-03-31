using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class DockerCodeExecutionService
    {
        public async Task<string> ExecuteAsync(string code)
        {
            var tempFolder =
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            // Create project inside Docker
            var createProcess = new Process();

            createProcess.StartInfo.FileName = "docker";
            createProcess.StartInfo.Arguments =
                $"run --rm -v \"{tempFolder}:/app\" -w /app mcr.microsoft.com/dotnet/sdk:8.0 dotnet new console";

            createProcess.StartInfo.RedirectStandardOutput = true;
            createProcess.StartInfo.UseShellExecute = false;

            createProcess.Start();
            await createProcess.WaitForExitAsync();

            // Replace Program.cs
            var programFile =
                Path.Combine(tempFolder, "Program.cs");

            var fullCode = $@"
                using System;

                class Program
                {{
                    static void Main()
                    {{
                        {code}
                    }}
                }}";

            await File.WriteAllTextAsync(programFile, fullCode);

            // Run project inside Docker
            var runProcess = new Process();

            runProcess.StartInfo.FileName = "docker";
            runProcess.StartInfo.Arguments =
                $"run --rm -v \"{tempFolder}:/app\" -w /app mcr.microsoft.com/dotnet/sdk:8.0 dotnet run";

            runProcess.StartInfo.RedirectStandardOutput = true;
            runProcess.StartInfo.RedirectStandardError = true;
            runProcess.StartInfo.UseShellExecute = false;

            runProcess.Start();

            var output =
                await runProcess.StandardOutput.ReadToEndAsync();

            var error =
                await runProcess.StandardError.ReadToEndAsync();

            await runProcess.WaitForExitAsync();

            if (!string.IsNullOrEmpty(error))
                return error;

            return output;
        }
    }
}