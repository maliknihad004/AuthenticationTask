
using AuthTask.Data;
using AuthTask.DTOs;
using AuthTask.Models;
using AuthTask.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthTask.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly IPasswordHasher<AppUser> _hasher;
    private readonly JwtService _jwtService;

    public AuthController(
        AuthDbContext context,
        IPasswordHasher<AppUser> hasher,
        JwtService jwtService)
    {
        _context = context;
        _hasher = hasher;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("All fields are required.");
        }

        string email = request.Email.Trim().ToLowerInvariant();

        bool exists = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (exists)
        {
            return BadRequest("Email already exists.");
        }

        AppUser user = new AppUser
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Role = "User"
        };

        user.PasswordHash = _hasher.HashPassword(
            user,
            request.Password
        );

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pg &&
                  pg.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return BadRequest("Email already exists.");
        }

        return StatusCode(201, new
        {
            user.Id,
            user.FullName,
            user.Email,
            user.Role
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        string? email = request.Email?.Trim().ToLowerInvariant();

        AppUser? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || request.Password == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var result = _hasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid email or password.");
        }

        string token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            token = token,
            tokenType = "Bearer"
        });
    }
}
