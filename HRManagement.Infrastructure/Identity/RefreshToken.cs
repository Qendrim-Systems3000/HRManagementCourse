namespace HRManagement.Infrastructure.Identity;

/// <summary>
/// Stored refresh token for JWT refresh flow. One-time use: revoked when used and a new one is issued (rotation).
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    /// <summary>Unique token value (e.g. Guid). Stored as issued so we can look it up.</summary>
    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>Set when token is used (rotation) or user logs out, so it cannot be reused.</summary>
    public DateTime? RevokedAt { get; set; }
}
