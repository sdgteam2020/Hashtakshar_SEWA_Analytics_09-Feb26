namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;

public interface IClientKeyHasher
{
    (string hashBase64, string saltBase64) Hash(string clientKey);
    bool Verify(string clientKey, string hashBase64, string saltBase64);
}