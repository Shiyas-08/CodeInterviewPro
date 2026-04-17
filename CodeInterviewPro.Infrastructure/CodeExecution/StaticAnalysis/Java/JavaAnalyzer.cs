using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.StaticAnalysis.Java
{
    public class JavaAnalyzer
    {
        public async Task<List<string>> AnalyzeAsync(string code)
        {
            var tempFolder =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            var filePath =
                Path.Combine(tempFolder, "Main.java");

            await File.WriteAllTextAsync(
                filePath,
                code);

            var process =
                new Process();

            process.StartInfo.FileName = "javac";
            process.StartInfo.Arguments =
                $"-Xlint {filePath}";

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            var error =
                await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            Directory.Delete(
                tempFolder,
                true);

            return error
                .Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}
