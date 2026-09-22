using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using Microsoft.AspNetCore.Http;

namespace HastaksharSewaAnalytics.Infrastructure.Security;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _context;

    public CurrentUserService(IHttpContextAccessor context)
    {
        _context = context;
    }

    public string TokenType =>
       _context.HttpContext?
       .User
       .FindFirst("token_type")
       ?.Value ?? "";


    public int? UserId
    {
        get
        {
            var sub = _context.HttpContext?
                .User
                .FindFirst("sub")
                ?.Value;

            return int.TryParse(sub, out var id)
                ? id
                : null;
        }
    }


    public int? ClientId
    {
        get
        {
            var client = _context.HttpContext?
                .User
                .FindFirst("client_id")
                ?.Value;

            return int.TryParse(client, out var id)
                ? id
                : null;
        }
    }
}
