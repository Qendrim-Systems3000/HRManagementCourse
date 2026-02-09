using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HRManagement.Application.DTOs;
using HRManagement.Application.Interfaces;
using HRManagement.Infrastructure.Identity;
using HRManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HRManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _db;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext db)
    {
        _userManager = userManager;
        _configuration = configuration;
        _db = db;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return new AuthResponseDto(false, "Invalid credentials");

        var (refreshTokenValue, refreshExpiresAt) = await CreateAndStoreRefreshTokenAsync(user.Id);

        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = BuildClaims(user, userRoles);
        var accessToken = GenerateJwtToken(authClaims);

        return new AuthResponseDto(
            true,
            "Login successful",
            new JwtSecurityTokenHandler().WriteToken(accessToken),
            refreshTokenValue,
            refreshExpiresAt
        );
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return new AuthResponseDto(false, "Refresh token is required.");

        var stored = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow, CancellationToken.None);

        if (stored == null)
            return new AuthResponseDto(false, "Invalid or expired refresh token.");

        var user = stored.User;
        string newRefreshValue;
        DateTime newRefreshExpiresAt;

        // Rotation: revoke current token and issue new one in one transaction (avoid revoking without issuing)
        await using var transaction = await _db.Database.BeginTransactionAsync(CancellationToken.None);
        try
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(CancellationToken.None);

            (newRefreshValue, newRefreshExpiresAt) = await CreateAndStoreRefreshTokenAsync(user.Id);
            await transaction.CommitAsync(CancellationToken.None);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = BuildClaims(user, userRoles);
        var accessToken = GenerateJwtToken(authClaims);

        return new AuthResponseDto(
            true,
            "Token refreshed.",
            new JwtSecurityTokenHandler().WriteToken(accessToken),
            newRefreshValue,
            newRefreshExpiresAt
        );
    }

    public async Task<AuthResponseDto> RevokeRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return new AuthResponseDto(false, "Refresh token is required.");

        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedAt == null, CancellationToken.None);

        if (stored == null)
            return new AuthResponseDto(false, "Invalid or already revoked refresh token.");

        stored.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(CancellationToken.None);
        return new AuthResponseDto(true, "Refresh token revoked.");
    }

    private static List<Claim> BuildClaims(ApplicationUser user, IList<string> userRoles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("LeaProfileId", user.LeaProfileId.ToString())
        };
        foreach (var role in userRoles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        return claims;
    }

    private async Task<(string TokenValue, DateTime ExpiresAt)> CreateAndStoreRefreshTokenAsync(string userId)
    {
        var expiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays());
        var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            Token = tokenValue,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(CancellationToken.None);
        return (tokenValue, expiresAt);
    }

    private JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
        var expires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes());

        return new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: expires,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }

    private int GetAccessTokenExpirationMinutes() =>
        _configuration.GetValue("JWT:AccessTokenExpirationMinutes", 15);

    private int GetRefreshTokenExpirationDays() =>
        _configuration.GetValue("JWT:RefreshTokenExpirationDays", 7);

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var existing = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existing != null)
            return new AuthResponseDto(false, "A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            EmailConfirmed = true,
            LeaProfileId = registerDto.LeaProfileId
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
            return new AuthResponseDto(false, string.Join("; ", result.Errors.Select(e => e.Description)));

        if (!string.IsNullOrWhiteSpace(registerDto.Role))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, registerDto.Role);
            if (!roleResult.Succeeded)
                return new AuthResponseDto(false, "User created but role assignment failed: " + string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }

        return new AuthResponseDto(true, "Registration successful.");
    }
}