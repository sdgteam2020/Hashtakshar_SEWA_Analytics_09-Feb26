using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class Device : AuditableEntity<int>
{
    private Device() : base(0)
    {
    }

    public string DeviceId { get; set; } = default!;          // Machine name
    public string DeviceKeyHash { get; set; } = default!;     // hashed
    public string DeviceKeySalt { get; set; } = default!;     // base64 salt
    public bool IsActive { get; set; } = true;

    private Device(int id) : base(id) { }

    public static Device Create(
        string deviceId,
        string deviceKeyHash,
        string deviceKeySalt)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("DeviceId is required.", nameof(deviceId));
        if (string.IsNullOrWhiteSpace(deviceKeyHash))
            throw new ArgumentException("DeviceKeyHash is required.", nameof(deviceKeyHash));
        if (string.IsNullOrWhiteSpace(deviceKeySalt))
            throw new ArgumentException("DeviceKeySalt is required.", nameof(deviceKeySalt));
        var device = new Device(0)
        {
            DeviceId = deviceId.Trim(),
            DeviceKeyHash = deviceKeyHash.Trim(),
            DeviceKeySalt = deviceKeySalt.Trim(),
            IsActive = true
        };
        return device;
    }
}