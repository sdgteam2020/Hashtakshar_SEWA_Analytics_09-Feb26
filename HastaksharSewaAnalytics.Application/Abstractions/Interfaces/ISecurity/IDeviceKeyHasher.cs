namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;

public interface IDeviceKeyHasher
{
    (string hashBase64, string saltBase64) Hash(string deviceKey);
    bool Verify(string deviceKey, string hashBase64, string saltBase64);
}