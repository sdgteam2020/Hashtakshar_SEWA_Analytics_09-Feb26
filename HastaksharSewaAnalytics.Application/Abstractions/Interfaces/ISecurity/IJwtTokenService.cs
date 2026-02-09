using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;

public interface IJwtTokenService
{
    string CreateToken(IEnumerable<Claim> claims, DateTime expiresUtc);
    int AccessTokenMinutes { get; }
}
