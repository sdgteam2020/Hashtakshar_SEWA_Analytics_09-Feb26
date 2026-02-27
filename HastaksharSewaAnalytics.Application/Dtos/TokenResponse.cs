namespace HastaksharSewaAnalytics.Application.Dtos;

public sealed class TokenResponse
{
    public string AccessToken { get; set; } = "";
    public int ExpiresInSeconds { get; set; }
    public string TokenType { get; set; } = "Bearer";
}
