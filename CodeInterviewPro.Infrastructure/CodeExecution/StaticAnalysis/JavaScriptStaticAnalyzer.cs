using CodeInterviewPro.Application.Interfaces.Services;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.CodeExecution.StaticAnalysis
{
    public class JavaScriptStaticAnalyzer : IStaticCodeAnalyzer
    {
        public async Task<string> AnalyzeAsync(string code)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            
            var tempFile = Path.Combine(tempFolder, "script.js");
            await File.WriteAllTextAsync(tempFile, code);

            var process = new Process();
            process.StartInfo.FileName = "docker";
            process.StartInfo.Arguments = $"run --rm -v \"{tempFolder}:/app\" -w /app node:18-alpine node --check script.js";
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            try { Directory.Delete(tempFolder, true); } catch { }

            return process.ExitCode == 0 ? string.Empty : error;
        }
    }
}