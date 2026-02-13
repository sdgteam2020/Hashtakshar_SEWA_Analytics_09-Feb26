using Microsoft.AspNetCore.Identity;

namespace HastaksharSewaAnalytics.Domain.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? ActiveSessionId { get; set; }
    public DateTime? ActiveSessionIssuedUtc { get; set; }
}
