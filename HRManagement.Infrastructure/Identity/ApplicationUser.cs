using Microsoft.AspNetCore.Identity;

namespace HRManagement.Infrastructure.Identity;

/// <summary>
/// Identity user with district (tenant) support. Lives in Infrastructure so Domain has no framework dependencies.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public int LeaProfileId { get; set; }
}
