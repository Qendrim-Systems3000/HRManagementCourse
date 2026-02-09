using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRManagement.Application.DTOs;
using HRManagement.Application.Interfaces;
using HRManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HRManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            return new AuthResponseDto(false, "Invalid credentials");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // CRITICAL: This allows ITenantService to filter data
            new Claim("LeaProfileId", user.LeaProfileId.ToString())
        };

        foreach (var role in userRoles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        var token = GenerateJwtToken(authClaims);

        return new AuthResponseDto(true, "Login successful", new JwtSecurityTokenHandler().WriteToken(token));
    }

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

    private JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

        return new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
    }
}