using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System.Data;

namespace CodeInterviewPro.Infrastructure.Repositories
{
    public class ExecutionHistoryRepository : IExecutionHistoryRepository
    {
        private readonly IDbConnection _connection;

        public ExecutionHistoryRepository(
            IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task SaveAsync(ExecutionHistory history)
        {
            var sql = @"
                INSERT INTO ExecutionHistory
                (
                    Id,
                    Code,
                    Language,
                    Total,
                    Passed,
                    Failed,
                    Score,
                    CreatedAt
                )
                VALUES
                (
                    @Id,
                    @Code,
                    @Language,
                    @Total,
                    @Passed,
                    @Failed,
                    @Score,
                    @CreatedAt
                )";

            await _connection.ExecuteAsync(sql, history);
        }
    }
}