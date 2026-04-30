using CodeInterviewPro.Application.Common.Responses;
using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
// Removed invalid InterviewsRepositories reference
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Application.Security;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly JwtService _jwt;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IInterviewInvitationRepository _invitationRepo;
    public AuthController(
        IUserRepository repo,
        IPasswordHasher hasher,
        JwtService jwt,
        IRefreshTokenRepository refreshRepo,
        IInterviewInvitationRepository invitationRepo) 
    {
        _repo = repo;
        _hasher = hasher;
        _jwt = jwt;
        _refreshRepo = refreshRepo;
        _invitationRepo = invitationRepo;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();

        Guid? tenantId = null;
        if (!string.IsNullOrEmpty(request.Token))
        {
            var invitation = await _invitationRepo.GetByTokenAsync(request.Token);
            if (invitation != null)
            {
                if (invitation.CandidateId != null) return BadRequest("Invitation already used");
                tenantId = invitation.TenantId;
            }
            else
            {
                return BadRequest("Invalid invite token");
            }
        }

        var existingUser = await _repo.GetByEmail(email);
        if (existingUser != null) return BadRequest("User already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            FullName = request.FullName,
            Role = UserRole.Candidate,
            IsActive = true
        };

        await _repo.Create(user);

        if (!string.IsNullOrEmpty(request.Token))
        {
            await _invitationRepo.UpdateCandidateAsync(request.Token, user.Id);
        }

        return Ok("User created successfully");
    }

    // ================= LOGIN =================
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var user = await _repo.GetByEmail(email);

        bool isValid = user != null &&
                       _hasher.Verify(request.Password, user.PasswordHash);

        await Task.Delay(300);
        if (!isValid)
            return Unauthorized(ApiResponse<string>.Failure("Invalid email or password"));

        if (user.Role == UserRole.Candidate)
        {
            await _invitationRepo.BindInvitesByEmail(user.Email, user.Id);
        }

        // Candidate flow validation
        if (!string.IsNullOrEmpty(request.Token) && user.Role == UserRole.Candidate)
        {
            var invitation = await _invitationRepo.GetByTokenAsync(request.Token);

            if (invitation == null)
                return Unauthorized(ApiResponse<string>.Failure("Invalid invite link"));

            if (user.TenantId != invitation.TenantId)
                return Unauthorized(ApiResponse<string>.Failure("Unauthorized tenant"));

            if (invitation.CandidateId == null || user.Id != invitation.CandidateId)
                return Unauthorized(ApiResponse<string>.Failure("Unauthorized candidate"));
        }

        var accessToken = _jwt.GenerateToken(
            user.Id,
            user.TenantId,
            user.Role,
            user.FullName);

        var refreshTokenValue = RefreshTokenGenerator.Generate();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshRepo.CreateAsync(refreshToken);

        Response.Cookies.Append(
            "accessToken",
            accessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

        Response.Cookies.Append(
            "refreshToken",
            refreshTokenValue,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

        return Ok(ApiResponse<string>.SuccessResponse(
            null,
            "Login successful"));
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdStr = User.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var userId = Guid.Parse(userIdStr);
        var user = await _repo.GetById(userId);

        if (user == null) return NotFound();

        return Ok(new
        {
            userId = user.Id,
            tenantId = user.TenantId,
            role = (int)user.Role,
            fullName = user.FullName,
            email = user.Email
        });
    }

    // ================= LOGOUT =================
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return Ok(ApiResponse<string>.SuccessResponse(
            null,
            "Logged out successfully"));
    }

    // ================= REFRESH =================
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(ApiResponse<string>.Failure("Refresh token missing"));

        var token = await _refreshRepo.GetByTokenAsync(refreshToken);

        if (token == null || token.IsRevoked || token.ExpiryDate < DateTime.UtcNow)
            return Unauthorized(ApiResponse<string>.Failure("Invalid refresh token"));

        var user = await _repo.GetById(token.UserId);

        if (user == null)
            return Unauthorized(ApiResponse<string>.Failure("User not found"));

        var newAccessToken =
            _jwt.GenerateToken(user.Id, user.TenantId, user.Role,user.FullName);

        Response.Cookies.Append(
            "accessToken",
            newAccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

        return Ok(ApiResponse<string>.SuccessResponse(
            null,
            "Token refreshed"));
    }
}
//using CodeInterviewPro.Application.DTOs;
 //using CodeInterviewPro.Application.Security;
 //using CodeInterviewPro.Application.Common.Responses;
 //using CodeInterviewPro.Domain.Entities;
 //using CodeInterviewPro.Domain.Enums;
 //using CodeInterviewPro.Infrastructure.Services;
 //using Microsoft.AspNetCore.Mvc;
 //using CodeInterviewPro.Application.Interfaces.Services;
 //using CodeInterviewPro.Application.Interfaces.Repositories;

//[ApiController]
//[Route("api/auth")]
//public class AuthController : ControllerBase
//{
//    private readonly IUserRepository _repo;
//    private readonly IPasswordHasher _hasher;
//    private readonly JwtService _jwt;
//    private readonly IRefreshTokenRepository _refreshRepo;

//    public AuthController(
//        IUserRepository repo,
//        IPasswordHasher hasher,
//        JwtService jwt,
//        IRefreshTokenRepository refreshRepo)
//    {
//        _repo = repo;
//        _hasher = hasher;
//        _jwt = jwt;
//        _refreshRepo = refreshRepo;
//    }

//// register 
//    [HttpPost("register")]
//    public async Task<IActionResult> Register(RegisterRequest request)
//    {
//        var email = request.Email.Trim().ToLower();

//        var hash = _hasher.Hash(request.Password);

//        var user = new User
//        {
//            TenantId = request.TenantId,
//            Email = email,
//            PasswordHash = hash,
//            FullName = request.FullName,
//            Role = UserRole.Candidate
//        };

//        await _repo.Create(user);

//        return Ok(ApiResponse<string>.SuccessResponse(
//            null,
//            "User created successfully"));
//    }

//    // login
//    [HttpPost("login")]
//    public async Task<IActionResult> Login(LoginRequest request)
//    {
//        var email = request.Email.Trim().ToLower();

//        var user = await _repo.GetByEmail(email);

//        // Constant-time validation logic
//        bool isValid = user != null &&
//                       _hasher.Verify(request.Password, user.PasswordHash);

//        // Small delay to prevent brute-force timing attacks
//        await Task.Delay(300);

//        if (!isValid)
//            return Unauthorized(ApiResponse<string>.Failure("Invalid email or password"));

//        var accessToken = _jwt.GenerateToken(
//            user.Id,
//            user.TenantId,
//            user.Role);

//        var refreshTokenValue = RefreshTokenGenerator.Generate();

//        var refreshToken = new RefreshToken
//        {
//            UserId = user.Id,
//            Token = refreshTokenValue,
//            ExpiryDate = DateTime.UtcNow.AddDays(7),
//            CreatedAt = DateTime.UtcNow
//        };

//        await _refreshRepo.CreateAsync(refreshToken);

//        Response.Cookies.Append(
//            "accessToken",
//            accessToken,
//            new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                SameSite = SameSiteMode.Strict,
//                Expires = DateTime.UtcNow.AddMinutes(30)
//            });

//        Response.Cookies.Append(
//            "refreshToken",
//            refreshTokenValue,
//            new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                SameSite = SameSiteMode.Strict,
//                Expires = DateTime.UtcNow.AddDays(7)
//            });

//        return Ok(ApiResponse<string>.SuccessResponse(
//            null,
//            "Login successful"));
//    }

//    // logout
//    [HttpPost("logout")]
//    public IActionResult Logout()
//    {
//        Response.Cookies.Delete("accessToken");
//        Response.Cookies.Delete("refreshToken");

//        return Ok(ApiResponse<string>.SuccessResponse(
//            null,
//            "Logged out successfully"));
//    }

//    // refresh token
//    [HttpPost("refresh")]
//    public async Task<IActionResult> Refresh()
//    {
//        var refreshToken = Request.Cookies["refreshToken"];

//        if (string.IsNullOrEmpty(refreshToken))
//            return Unauthorized(ApiResponse<string>.Failure("Refresh token missing"));

//        var token = await _refreshRepo.GetByTokenAsync(refreshToken);

//        if (token == null || token.IsRevoked || token.ExpiryDate < DateTime.UtcNow)
//            return Unauthorized(ApiResponse<string>.Failure("Invalid refresh token"));

//        var user = await _repo.GetById(token.UserId);

//        if (user == null)
//            return Unauthorized(ApiResponse<string>.Failure("User not found"));

//        var newAccessToken =
//            _jwt.GenerateToken(user.Id, user.TenantId, user.Role);

//        Response.Cookies.Append(
//            "accessToken",
//            newAccessToken,
//            new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                SameSite = SameSiteMode.Strict,   
//                Expires = DateTime.UtcNow.AddMinutes(30)
//            });

//        return Ok(ApiResponse<string>.SuccessResponse(
//            null,
//            "Token refreshed"));
//    }
//}