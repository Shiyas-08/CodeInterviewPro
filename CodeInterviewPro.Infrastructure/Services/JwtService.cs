using CodeInterviewPro.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class JwtService
    {
        private readonly string _jwtKey;

        public JwtService()
        {
            _jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrEmpty(_jwtKey))
            {
                throw new Exception("JWT key not found in environment variables");
            }
        }

        public string GenerateToken(Guid userId, Guid? tenantId, UserRole role,string FullName)
        {
            var claims = new List<Claim>
            {
                new Claim("uid", userId.ToString()),
                new Claim("name", FullName),
                new Claim("rid", ((int)role).ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            if (tenantId.HasValue)
            {
                claims.Add(new Claim("tid", tenantId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtKey));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}