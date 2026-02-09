namespace HastaksharSewaAnalytics.Application.Dtos.Devices;

public sealed record DeviceRequest
{
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceKey { get; set; } = string.Empty;

}
