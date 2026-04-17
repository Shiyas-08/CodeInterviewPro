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

        public string GenerateToken(Guid userId, Guid? tenantId, UserRole role)
        {
            var claims = new List<Claim>
            {
                new Claim("uid", userId.ToString()),
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
//using CodeInterviewPro.Domain.Enums;
 //using Microsoft.Extensions.Configuration;
 //using Microsoft.IdentityModel.Tokens;
 //using System.IdentityModel.Tokens.Jwt;
 //using System.Security.Claims;
 //using System.Text;

//namespace CodeInterviewPro.Infrastructure.Services
//{
//    public class JwtService
//    {
//        private IConfiguration _config;

//        public JwtService(IConfiguration config)
//        {
//            _config = config;
//        }

//        public string GenerateToken(Guid userId, Guid? tenantId, UserRole role)
//        {
//            var claims = new List<Claim>
//    {
//        new Claim("uid", userId.ToString()),
//        new Claim("rid", ((int)role).ToString()),
//        new Claim(ClaimTypes.Role, role.ToString()) 
//    };

//            // Add tenant claim only if exists
//            if (tenantId.HasValue)
//            {
//                claims.Add(new Claim("tid", tenantId.Value.ToString()));
//            }

//            var key = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

//            var creds = new SigningCredentials(
//                key,
//                SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                claims: claims,
//                expires: DateTime.UtcNow.AddMinutes(30),
//                signingCredentials: creds
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}