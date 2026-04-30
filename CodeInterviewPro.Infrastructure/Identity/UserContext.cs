using CodeInterviewPro.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CodeInterviewPro.Infrastructure.Identity;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new Exception("User not authenticated");

    public Guid TenantId
    {
        get
        {
            var claim = User.FindFirst("tid") 
                        ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid");
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }

    public Guid UserId
    {
        get
        {
            var claim = User.FindFirst("uid") 
                        ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }

    public string Role =>
        User.FindFirst("rid")?.Value 
        ?? User.FindFirst(ClaimTypes.Role)?.Value 
        ?? string.Empty;
}
