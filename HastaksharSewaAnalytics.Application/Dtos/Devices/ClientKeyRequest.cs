namespace HastaksharSewaAnalytics.Application.Dtos.Devices;

public sealed record ClientKeyRequest
{
    public string DomainId { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string ClientKey { get; set; } = string.Empty;

}
