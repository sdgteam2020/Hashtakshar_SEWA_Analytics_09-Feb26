using Microsoft.AspNetCore.Identity;

namespace HastaksharSewaAnalytics.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    // Add extra columns if needed
    public string? FullName { get; set; }
}
