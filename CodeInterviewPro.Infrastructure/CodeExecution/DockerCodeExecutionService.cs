using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class DockerCodeExecutionService
    {
        public async Task<string> ExecuteAsync(string code)
        {
            var tempFile =
                Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid()}.cs");

            await File.WriteAllTextAsync(
                tempFile,
                code);

            // Copy file
            var copyProcess = new Process();

            copyProcess.StartInfo.FileName = "docker";
            copyProcess.StartInfo.Arguments =
                $"cp \"{tempFile}\" runner1:/workspace/Program.cs";

            copyProcess.StartInfo.UseShellExecute = false;
            copyProcess.Start();

            await copyProcess.WaitForExitAsync();

            // Build once
            var buildProcess = new Process();

            buildProcess.StartInfo.FileName = "docker";
            buildProcess.StartInfo.Arguments =
                $"exec runner1 dotnet build /workspace";

            buildProcess.StartInfo.UseShellExecute = false;
            buildProcess.Start();

            await buildProcess.WaitForExitAsync();

            // Run compiled dll
            var runProcess = new Process();

            runProcess.StartInfo.FileName = "docker";
            runProcess.StartInfo.Arguments =
                $"exec runner1 dotnet run --no-build --project /workspace";

            runProcess.StartInfo.RedirectStandardOutput = true;
            runProcess.StartInfo.RedirectStandardError = true;
            runProcess.StartInfo.UseShellExecute = false;

            runProcess.Start();

            var output =
                await runProcess.StandardOutput.ReadToEndAsync();

            var error =
                await runProcess.StandardError.ReadToEndAsync();

            try
            {
                File.Delete(tempFile);
            }
            catch { }

            if (!string.IsNullOrEmpty(error))
                return error;

            return output;
        }
    }
}
//using System.Diagnostics;

//namespace CodeInterviewPro.Infrastructure.CodeExecution
//{
//    public class DockerCodeExecutionService
//    {
//        public async Task<string> ExecuteAsync(string code)
//        {
//            var tempFile =
//                Path.Combine(
//                    Path.GetTempPath(),
//                    $"{Guid.NewGuid()}.cs");

//            await File.WriteAllTextAsync(
//                tempFile,
//                code);

//            // Copy file into container
//            var copyProcess = new Process();

//            copyProcess.StartInfo.FileName = "docker";
//            copyProcess.StartInfo.Arguments =
//                $"cp \"{tempFile}\" runner1:/workspace/Program.cs";

//            copyProcess.StartInfo.UseShellExecute = false;
//            copyProcess.Start();

//            await copyProcess.WaitForExitAsync();

//            // Execute code
//            var runProcess = new Process();

//            runProcess.StartInfo.FileName = "docker";
//            runProcess.StartInfo.Arguments =
//                $"exec runner1 dotnet run --project /workspace";

//            runProcess.StartInfo.RedirectStandardOutput = true;
//            runProcess.StartInfo.RedirectStandardError = true;
//            runProcess.StartInfo.UseShellExecute = false;

//            runProcess.Start();

//            var output =
//                await runProcess.StandardOutput.ReadToEndAsync();

//            var error =
//                await runProcess.StandardError.ReadToEndAsync();

//            try
//            {
//                File.Delete(tempFile);
//            }
//            catch { }

//            if (!string.IsNullOrEmpty(error))
//                return error;

//            return output;
//        }
//    }
//}