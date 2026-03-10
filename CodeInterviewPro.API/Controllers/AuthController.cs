using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces;
using CodeInterviewPro.Application.Security;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Domain.Enums;
using CodeInterviewPro.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
        JwtService jwt,IRefreshTokenRepository refreshRepo)
    {
        _repo = repo;
        _hasher = hasher;
        _jwt = jwt;
        _refreshRepo = refreshRepo;
    }
    //register
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

        return Ok("User created");
    }
    //login

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _repo.GetByEmail(request.Email);

        if (user == null)
            return Unauthorized();

        var valid = _hasher.Verify(request.Password, user.PasswordHash);

        if (!valid)
            return Unauthorized();

        var accessToken = _jwt.GenerateToken(
            user.Id,
            user.TenantId,
            user.Role);

        var refreshTokenValue =
            RefreshTokenGenerator.Generate();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshRepo.CreateAsync(refreshToken);

        // AccessToken Cookie
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

        // RefreshToken Cookie
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

        return Ok("Login successful");
    }
    //logout 
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return Ok("Logged out");
    }
    [HttpPost("refresh")]
public async Task<IActionResult> Refresh()
{
    var refreshToken = Request.Cookies["refreshToken"];

    if (refreshToken == null)
        return Unauthorized();

    var token = await _refreshRepo.GetByTokenAsync(refreshToken);

    if (token == null)
        return Unauthorized();

    if (token.IsRevoked)
        return Unauthorized();

    if (token.ExpiryDate < DateTime.UtcNow)
        return Unauthorized();

    var user = await _repo.GetById(token.UserId);

    if (user == null)
        return Unauthorized();

    var newAccessToken =
        _jwt.GenerateToken(user.Id,user.TenantId, user.Role);

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

    return Ok();
}
}