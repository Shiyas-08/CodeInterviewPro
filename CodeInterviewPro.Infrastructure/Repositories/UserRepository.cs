
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
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        public UserRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task Create(User user)
        {
            var sql = @"insert into Users (TenantId,Email,PasswordHash,FullName,Role)
                 VALUES 
                 (@TenantId,@Email,@PasswordHash,@FullName,@Role)";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, user);
        }

        public async Task<User?> GetByEmail(string email)
        {
            var sql = "select * from Users where Email=@Email";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                sql, new { Email = email });
        }
        public async Task<User?> GetById(int id)
        {
            var sql = "SELECT * FROM Users WHERE Id=@Id";

            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Id = id });
        }
    }
}
