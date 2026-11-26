using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Models;
using NexaWork.Application.DTOs;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

namespace NexaWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _currentUserService = currentUserService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Organization)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r!.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(ApiResponse<AuthResponse>.FailureResult("Invalid email or password credentials."));
        }

        if (!user.IsActive)
        {
            return BadRequest(ApiResponse<AuthResponse>.FailureResult("Your user account has been deactivated."));
        }

        user.LastLoginAt = DateTime.UtcNow;

        var roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role!.RolePermissions)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToList();

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(120),
            User = new UserProfileDto
            {
                Id = user.Id,
                OrganizationId = user.OrganizationId,
                OrganizationName = user.Organization?.Name ?? "Nexa Enterprise",
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JobTitle = user.JobTitle,
                AvatarUrl = user.AvatarUrl,
                Role = roles.FirstOrDefault() ?? user.RoleType.ToString(),
                Permissions = permissions
            }
        };

        return Ok(ApiResponse<AuthResponse>.SuccessResult(response, "Login successful"));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var existingToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u!.Organization)
            .Include(rt => rt.User)
            .ThenInclude(u => u!.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r!.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (existingToken == null || !existingToken.IsActive)
        {
            return Unauthorized(ApiResponse<AuthResponse>.FailureResult("Invalid or expired refresh token."));
        }

        var user = existingToken.User!;
        existingToken.IsRevoked = true;

        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id);
        existingToken.ReplacedByToken = newRefreshToken.Token;

        var roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role!.RolePermissions)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToList();

        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user, roles, permissions);

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(120),
            User = new UserProfileDto
            {
                Id = user.Id,
                OrganizationId = user.OrganizationId,
                OrganizationName = user.Organization?.Name ?? "Nexa Enterprise",
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JobTitle = user.JobTitle,
                Role = roles.FirstOrDefault() ?? user.RoleType.ToString(),
                Permissions = permissions
            }
        };

        return Ok(ApiResponse<AuthResponse>.SuccessResult(response, "Token refreshed successfully"));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetCurrentUser()
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponse<UserProfileDto>.FailureResult("User not identified."));
        }

        var user = await _context.Users
            .Include(u => u.Organization)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r!.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId.Value);

        if (user == null)
        {
            return NotFound(ApiResponse<UserProfileDto>.FailureResult("User profile not found."));
        }

        var roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role!.RolePermissions)
            .Select(rp => rp.Permission!.Name)
            .Distinct()
            .ToList();

        var profile = new UserProfileDto
        {
            Id = user.Id,
            OrganizationId = user.OrganizationId,
            OrganizationName = user.Organization?.Name ?? "Nexa Enterprise",
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            JobTitle = user.JobTitle,
            AvatarUrl = user.AvatarUrl,
            Role = roles.FirstOrDefault() ?? user.RoleType.ToString(),
            Permissions = permissions
        };

        return Ok(ApiResponse<UserProfileDto>.SuccessResult(profile));
    }
}
