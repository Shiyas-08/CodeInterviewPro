using System.Diagnostics;

namespace CodeInterviewPro.Infrastructure.StaticAnalysis.Go
{
    public class GoVetAnalyzer
    {
        public async Task<List<string>> AnalyzeAsync(string code)
        {
            var tempFile =
                Path.GetTempFileName() + ".go";

            await File.WriteAllTextAsync(
                tempFile,
                code);

            var process =
                new Process();

            process.StartInfo.FileName = "go";
            process.StartInfo.Arguments =
                $"vet {tempFile}";

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