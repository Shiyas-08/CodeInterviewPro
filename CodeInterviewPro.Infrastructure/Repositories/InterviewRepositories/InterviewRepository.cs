using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories
{
    public class InterviewRepository:IInterviewRepository
    {
        private readonly DapperContext _db;
        public InterviewRepository(DapperContext db)
        {
            _db = db;
            
        }

        public async Task<long> CreateAsync(Interview interview)
        {
            var sql = @"
                INSERT INTO Interviews
                (TenantId, Title, Description, DurationMinutes, Status, CreatedBy, CreatedAt, IsActive)
                VALUES
                (@TenantId, @Title, @Description, @DurationMinutes, @Status, @CreatedBy, GETUTCDATE(), 1);
                
                SELECT CAST(SCOPE_IDENTITY() as bigint);
            ";
            using var connection = _db.CreateConnection();

            return await connection.ExecuteScalarAsync<long>(sql,interview);
        }
        public async Task<Interview?> GetByIdAsync(long id, long tenantId)
        {
            var sql = @"
                SELECT * 
                FROM Interviews 
                WHERE Id = @Id 
                AND TenantId = @TenantId 
                AND IsActive = 1
            ";
            using var connection = _db.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Interview>(
                sql,
                new { Id = id, TenantId = tenantId });
        }

        public async Task<IEnumerable<Interview>> GetAllAsync(long tenantId)
        {
            var sql = @"
                SELECT * 
                FROM Interviews 
                WHERE TenantId = @TenantId 
                AND IsActive = 1
                ORDER BY CreatedAt DESC
            ";
            using var connection = _db.CreateConnection();

            return await connection.QueryAsync<Interview>(
                sql,
                new { TenantId = tenantId });
        }

        public async Task UpdateAsync(Interview interview)
        {
            var sql = @"
                UPDATE Interviews
                SET 
                    Title = @Title,
                    Description = @Description,
                    DurationMinutes = @DurationMinutes,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    Status = @Status
                WHERE Id = @Id 
                AND TenantId = @TenantId
            ";
            using var connection = _db.CreateConnection();

            await connection.ExecuteAsync(sql, interview);
        }
        public async Task<Interview?> GetByTokenAsync(string token)
        {
            var sql = @"
        SELECT *
        FROM Interviews
        WHERE SecureToken = @Token
    ";
            using var connection = _db.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Interview>(
                sql,
                new { Token = token }
            );
        }
    }

}
