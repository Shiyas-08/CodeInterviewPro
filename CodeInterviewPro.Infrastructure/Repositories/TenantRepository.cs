using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly DapperContext _context;

        public TenantRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Tenant tenant)
        {
            var query = @"INSERT INTO Tenants 
        (Id, Name, Domain, IsActive, CreatedAt)
        VALUES (@Id, @Name, @Domain, @IsActive, @CreatedAt)";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, tenant);
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Tenant>(
                "SELECT * FROM Tenants WHERE IsActive = 1");
        }

        public async Task<Tenant> GetByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Tenant>(
                "SELECT * FROM Tenants WHERE Id = @Id", new { id });
        }

        public async Task<Tenant> GetByNameAsync(string name)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Tenant>(
                "SELECT * FROM Tenants WHERE Name = @Name", new { name });
        }

        public async Task<Tenant> GetByDomainAsync(string domain)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Tenant>(
                "SELECT * FROM Tenants WHERE Domain = @Domain", new { domain });
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            var query = @"UPDATE Tenants 
        SET Name = @Name,
            Domain = @Domain,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, tenant);
        }
    }
}
