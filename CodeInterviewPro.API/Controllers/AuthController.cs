using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Application.Security;
using CodeInterviewPro.Application.Common.Responses;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly JwtService _jwt;
    private readonly IRefreshTokenRepository _refreshRepo;

    public AuthController(
        IUserRepository repo,
        IPasswordHasher hasher,
        JwtService jwt,
        IRefreshTokenRepository refreshRepo)
    {
        _repo = repo;
        _hasher = hasher;
        _jwt = jwt;
        _refreshRepo = refreshRepo;
    }

    // REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var hash = _hasher.Hash(request.Password);

        var user = new User
        {
            TenantId = request.TenantId,
            Email = request.Email,
            PasswordHash = hash,
            FullName = request.FullName,
            Role = UserRole.Candidate
        };

        await _repo.Create(user);

        return Ok(ApiResponse<string>.SuccessResponse(
            null,
            "User created successfully"));
    }

    // LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _repo.GetByEmail(request.Email);

        if (user == null)
            return Unauthorized(ApiResponse<string>.Failure("Invalid credentials"));

        var valid = _hasher.Verify(request.Password, user.PasswordHash);

        if (!valid)
            return Unauthorized(ApiResponse<string>.Failure("Invalid credentials"));

        var accessToken = _jwt.GenerateToken(
            user.Id,
            user.TenantId,
            user.Role);

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
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

        Response.Cookies.Append(
            "refreshToken",
            refreshTokenValue,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

        return Ok(ApiResponse<string>.SuccessResponse(
            null,
            "Login successful"));
    }

    // LOGOUT
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return Ok(ApiResponse<string>.SuccessResponse(
            null,
            "Logged out successfully"));
    }

    // REFRESH TOKEN
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (refreshToken == null)
            return Unauthorized(ApiResponse<string>.Failure("Refresh token missing"));

        var token = await _refreshRepo.GetByTokenAsync(refreshToken);

        if (token == null || token.IsRevoked || token.ExpiryDate < DateTime.UtcNow)
            return Unauthorized(ApiResponse<string>.Failure("Invalid refresh token"));

        var user = await _repo.GetById(token.UserId);

        if (user == null)
            return Unauthorized(ApiResponse<string>.Failure("User not found"));

        var newAccessToken =
            _jwt.GenerateToken(user.Id, user.TenantId, user.Role);

        Response.Cookies.Append(
            "accessToken",
            newAccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

        return Ok(ApiResponse<string>.SuccessResponse(
            null,
            "Token refreshed"));
    }
}