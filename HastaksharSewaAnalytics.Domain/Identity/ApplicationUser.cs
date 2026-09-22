using Microsoft.AspNetCore.Identity;

namespace HastaksharSewaAnalytics.Domain.Identity;

public sealed class ApplicationUser : IdentityUser<int>
{
    public string? ActiveSessionId { get; set; }
    public DateTime? ActiveSessionIssuedUtc { get; set; }
        
}
