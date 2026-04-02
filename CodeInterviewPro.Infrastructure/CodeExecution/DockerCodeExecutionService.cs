using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class DockerCodeExecutionService
    {
        public async Task<string> ExecuteAsync(string code)
        {
            var tempFolder =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            try
            {
                // Create project inside Docker
                var createProcess = new Process();

                createProcess.StartInfo.FileName = "docker";
                createProcess.StartInfo.Arguments =
                    $"run --rm " +
                    $"--network none " +
                    $"--memory 256m " +
                    $"--cpus 1 " +
                    $"--pids-limit 50 " +
                    $"--cap-drop ALL " +
                    $"--security-opt no-new-privileges " +
                    $"-v \"{tempFolder}:/app\" " +
                    $"-w /app " +
                    $"mcr.microsoft.com/dotnet/sdk:8.0 " +
                    $"dotnet new console";

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

                await File.WriteAllTextAsync(
                    programFile,
                    fullCode);


                // Run project inside Docker
                var runProcess = new Process();

                runProcess.StartInfo.FileName = "docker";
                runProcess.StartInfo.Arguments =
                    $"run --rm " +
                    $"--network none " +
                    $"--memory 256m " +
                    $"--cpus 1 " +
                    $"--pids-limit 50 " +
                    $"--cap-drop ALL " +
                    $"--security-opt no-new-privileges " +
                    $"-v \"{tempFolder}:/app\" " +
                    $"-w /app " +
                    $"mcr.microsoft.com/dotnet/sdk:8.0 " +
                    $"dotnet run";

                runProcess.StartInfo.RedirectStandardOutput = true;
                runProcess.StartInfo.RedirectStandardError = true;
                runProcess.StartInfo.UseShellExecute = false;

                runProcess.Start();

                // Timeout (5 seconds)
                var timeoutTask =
                    Task.Delay(TimeSpan.FromSeconds(5));

                var processTask =
                    runProcess.WaitForExitAsync();

                var completed =
                    await Task.WhenAny(
                        processTask,
                        timeoutTask);

                if (completed == timeoutTask)
                {
                    try
                    {
                        runProcess.Kill();
                    }
                    catch { }

                    return "Execution Timeout";
                }

                var output =
                    await runProcess.StandardOutput.ReadToEndAsync();

                var error =
                    await runProcess.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(error))
                    return error;

                return output;
            }
            finally
            {
                // Cleanup temp folder
                try
                {
                    Directory.Delete(
                        tempFolder,
                        true);
                }
                catch { }
            }
        }
    }
}