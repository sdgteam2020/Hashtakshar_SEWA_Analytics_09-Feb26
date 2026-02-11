using Microsoft.AspNetCore.Identity;

namespace HastaksharSewaAnalytics.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
