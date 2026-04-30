using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly DapperContext _db;

        public InterviewRepository(DapperContext db)
        {
            _db = db;
        }

        public async Task<Guid> CreateAsync(Interview interview)
        {
            var sql = @"
        INSERT INTO Interviews
        (Id, TenantId, Title, Description, DurationMinutes, Status, CreatedBy, CreatedAt, IsActive, SecureToken)
        VALUES
        (@Id, @TenantId, @Title, @Description, @DurationMinutes, @Status, @CreatedBy, GETUTCDATE(), 1, @SecureToken);
    ";

            interview.Id = Guid.NewGuid();

            //  FIX
            interview.SecureToken = Guid.NewGuid().ToString();

            using var connection = _db.CreateConnection();

            await connection.ExecuteAsync(sql, interview);

            return interview.Id;
        }

        public async Task<Interview?> GetByIdAsync(Guid id, Guid tenantId)
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

        public async Task<IEnumerable<Interview>> GetAllAsync(Guid tenantId)
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

        public async Task<IEnumerable<dynamic>> GetAllWithInvitationAsync(Guid tenantId)
        {
            var sql = @"
                SELECT i.*, inv.Token, inv.CandidateId, inv.CandidateEmail
                FROM Interviews i
                LEFT JOIN InterviewInvitations inv ON i.Id = inv.InterviewId
                WHERE i.TenantId = @TenantId 
                AND i.IsActive = 1
                ORDER BY i.CreatedAt DESC
            ";

            using var connection = _db.CreateConnection();

            return await connection.QueryAsync(sql, new { TenantId = tenantId });
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

        public async Task DeleteAsync(Guid id, Guid tenantId)
        {
            var sql = "UPDATE Interviews SET IsActive = 0 WHERE Id = @Id AND TenantId = @TenantId";
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync(sql, new { Id = id, TenantId = tenantId });
        }
    }
}