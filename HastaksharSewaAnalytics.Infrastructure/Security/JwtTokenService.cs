using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HastaksharSewaAnalytics.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _key;

    public int AccessTokenMinutes { get; }

    public JwtTokenService(IConfiguration config)
    {
        _issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing");
        _audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing");
        var keyStr = config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");

        AccessTokenMinutes = int.TryParse(config["Jwt:AccessTokenMinutes"], out var m) ? m : 30;

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
    }

    public string CreateToken(IEnumerable<Claim> claims, DateTime expiresUtc)
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresUtc,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}