using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using System.Security.Cryptography;
using System.Text;

namespace HastaksharSewaAnalytics.Infrastructure.Security;

public class DeviceKeyHasher : IDeviceKeyHasher
{
    private const int SaltSize = 16;        // 128-bit
    private const int KeySize = 32;         // 256-bit
    private const int Iterations = 120_000; // good baseline

    public (string hashBase64, string saltBase64) Hash(string deviceKey)
    {
        if (string.IsNullOrWhiteSpace(deviceKey))
            throw new ArgumentException("DeviceKey is empty.");

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(deviceKey),
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool Verify(string deviceKey, string hashBase64, string saltBase64)
    {
        if (string.IsNullOrWhiteSpace(deviceKey)) return false;

        byte[] salt = Convert.FromBase64String(saltBase64);
        byte[] expectedHash = Convert.FromBase64String(hashBase64);

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password: Encoding.UTF8.GetBytes(deviceKey),
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
