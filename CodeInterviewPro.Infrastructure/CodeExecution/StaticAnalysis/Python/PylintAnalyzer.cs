using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.StaticAnalysis.Python
{
    public class PylintAnalyzer
    {
        public async Task<List<string>> AnalyzeAsync(string code)
        {
            var tempFile =
                Path.GetTempFileName() + ".py";

            await File.WriteAllTextAsync(
                tempFile,
                code);

            var process =
                new Process();

            process.StartInfo.FileName = "pylint";
            process.StartInfo.Arguments =
                $"{tempFile} --score=n";

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();

            var output =
                await process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();

            File.Delete(tempFile);

            return output
                .Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}