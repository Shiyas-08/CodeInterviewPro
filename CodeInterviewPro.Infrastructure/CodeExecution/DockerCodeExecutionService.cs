using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class DockerCodeExecutionService
    {
        private const string ImageName = "code-runner"; // Ensure this image exists (from Dockerfile.runner)
        private const int TimeoutSeconds = 30;

        public async Task<string> ExecuteAsync(string code)
        {
            var executionId = Guid.NewGuid().ToString();
            var tempDir = Path.Combine(Path.GetTempPath(), "CodeExecution", executionId);
            Directory.CreateDirectory(tempDir);

            var tempFile = Path.Combine(tempDir, "Program.cs");
            await File.WriteAllTextAsync(tempFile, code);

            // Create a basic project file if it doesn't exist in the container's workspace
            // Or assume the image already has a .csproj that we can use.
            // Since Dockerfile.runner does 'dotnet new console', it has a .csproj.

            try
            {
                var runProcess = new Process();
                runProcess.StartInfo.FileName = "docker";
                
                // --rm: Remove container after exit
                // --memory: Limit memory to 256MB
                // --cpus: Limit CPU to 0.5
                // -v: Mount the temp directory to /workspacedocke
                runProcess.StartInfo.Arguments = 
                    $"run --rm " +
                    $"--memory=\"256m\" " +
                    $"--cpus=\"0.5\" " +
                    $"-v \"{tempDir}:/workspace\" " +
                    $"{ImageName} " +
                    $"dotnet run --project /workspace";

                runProcess.StartInfo.RedirectStandardOutput = true;
                runProcess.StartInfo.RedirectStandardError = true;
                runProcess.StartInfo.UseShellExecute = false;
                runProcess.StartInfo.CreateNoWindow = true;

                runProcess.Start();

                var outputTask = runProcess.StandardOutput.ReadToEndAsync();
                var errorTask = runProcess.StandardError.ReadToEndAsync();

                if (await Task.WhenAny(runProcess.WaitForExitAsync(), Task.Delay(TimeSpan.FromSeconds(TimeoutSeconds))) == Task.Delay(TimeSpan.FromSeconds(TimeoutSeconds)))
                {
                    runProcess.Kill(true);
                    return "Execution timed out.";
                }

                var output = await outputTask;
                var error = await errorTask;

                if (!string.IsNullOrEmpty(error))
                    return error;

                return output;
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                }
                catch { }
            }
        }
    }
}
