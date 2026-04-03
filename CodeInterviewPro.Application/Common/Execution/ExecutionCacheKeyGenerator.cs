using System.Security.Cryptography;
using System.Text;

namespace CodeInterviewPro.Application.Common.Execution
{
    public static class ExecutionCacheKeyGenerator
    {
        public static string Generate(
            string code,
            string language,
            string testCases)
        {
            var raw = $"{language}:{code}:{testCases}";

            using var sha = SHA256.Create();

            var bytes = sha.ComputeHash(
                Encoding.UTF8.GetBytes(raw));

            return Convert.ToBase64String(bytes);
        }
    }
}