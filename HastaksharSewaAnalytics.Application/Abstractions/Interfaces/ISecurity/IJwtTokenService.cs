using System.Security.Claims;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;

public interface IJwtTokenService
{
    string CreateToken(IEnumerable<Claim> claims, DateTime expiresUtc);
    int AccessTokenMinutes { get; }
}
