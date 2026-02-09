namespace HRManagement.Application.DTOs;

public record LoginDto(string Email, string Password);

public record RegisterDto(
    string Email, 
    string Password, 
    string FirstName, 
    string LastName, 
    int LeaProfileId, 
    string Role // "Admin" or "HRUser"
);

public record AuthResponseDto(
    bool IsSuccess,
    string Message,
    string? Token = null,
    string? RefreshToken = null,
    DateTime? RefreshTokenExpiresAt = null
);

/// <summary>Request body for refresh token endpoint.</summary>
public record RefreshTokenDto(string RefreshToken);