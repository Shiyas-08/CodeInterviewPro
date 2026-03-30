using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.CodeExecution
{
    public class CodeExecutionService
    {
        public async Task<string> ExecuteCode(string code)
        {
            var tempFolder =
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            // Create project
            var createProject = new Process();
            createProject.StartInfo.FileName = "dotnet";
            createProject.StartInfo.Arguments = "new console";
            createProject.StartInfo.WorkingDirectory = tempFolder;
            createProject.StartInfo.RedirectStandardOutput = true;
            createProject.StartInfo.UseShellExecute = false;

            createProject.Start();
            await createProject.WaitForExitAsync();

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

            // Run project
            var runProcess = new Process();

            runProcess.StartInfo.FileName = "dotnet";
            runProcess.StartInfo.Arguments = "run";
            runProcess.StartInfo.WorkingDirectory = tempFolder;
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