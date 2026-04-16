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

    public Guid TenantId =>
        Guid.Parse(User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")!.Value);

    public Guid UserId =>
       Guid.Parse(User.FindFirst("uid")!.Value);
    public string Role =>
        User.FindFirst(ClaimTypes.Role)!.Value;
}