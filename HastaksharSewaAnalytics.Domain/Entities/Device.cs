using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class Device : AuditableEntity<Guid>
{
    private Device() : base(Guid.Empty)
    {
    }

    public string DeviceId { get; set; } = default!;          // Machine name
    public string DeviceKeyHash { get; set; } = default!;     // hashed
    public string DeviceKeySalt { get; set; } = default!;     // base64 salt
    public bool IsActive { get; set; } = true;

    private Device(Guid id) : base(id) { }

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
        var device = new Device(Guid.NewGuid())
        {
            DeviceId = deviceId.Trim(),
            DeviceKeyHash = deviceKeyHash.Trim(),
            DeviceKeySalt = deviceKeySalt.Trim(),
            IsActive = true
        };
        return device;
    }
}