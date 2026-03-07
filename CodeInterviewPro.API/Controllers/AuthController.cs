using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces;
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
    private readonly PasswordHasher _hasher;
    private readonly JwtService _jwt;

    public AuthController(
        IUserRepository repo,
        PasswordHasher hasher,
        JwtService jwt)
    {
        _repo = repo;
        _hasher = hasher;
        _jwt = jwt;
    }

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

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _repo.GetByEmail(request.Email);

        if (user == null)
            return Unauthorized();

        var valid = _hasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!valid)
            return Unauthorized();

        var token = _jwt.GenerateToken(user.Id, user.Role);

        return Ok(token);
    }
}