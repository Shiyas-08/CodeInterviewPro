using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Domain.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DapperContext _context;

        public RefreshTokenRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(RefreshToken token)
        {
            var sql = @"INSERT INTO RefreshTokens
                    (UserId,Token,ExpiryDate,IsRevoked,CreatedAt)
                    VALUES
                    (@UserId,@Token,@ExpiryDate,0,@CreatedAt)";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(sql, token);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var sql = "SELECT * FROM RefreshTokens WHERE Token=@Token";

            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<RefreshToken>(
                sql,
                new { Token = token });
        }

        public async Task RevokeAsync(string token)
        {
            var sql = @"UPDATE RefreshTokens
                    SET IsRevoked = 1
                    WHERE Token = @Token";

            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(sql, new { Token = token });
        }
    }
}
