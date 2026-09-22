using System;
using System.Collections.Generic;
using System.Text;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;

public interface ICurrentUserService
{
    int? UserId { get; }
    int? ClientId { get; }
    string TokenType { get; }
}
