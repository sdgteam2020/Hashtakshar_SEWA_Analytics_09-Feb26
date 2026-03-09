using System.Text;
using System.Security.Cryptography;

namespace HastaksharSewaAnalytics.Presentation.Helpers;

public static class AESEncrytDecry
{
    public static string GetSalt()
    {
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(saltBytes);
    }

    public static string GetKey()
    {        
        byte[] keyBytes = RandomNumberGenerator.GetBytes(16);
        return Encoding.UTF8.GetString(ToNumeric16Bytes(keyBytes));
    }

    private static byte[] ToNumeric16Bytes(byte[] input)
    {
        byte[] result = new byte[16];
        for (int i = 0; i < 16; i++)
        {
            result[i] = (byte)('0' + (input[i] % 10));
        }
        return result;
    }

    public static string DecryptAES(string cipherText, string key)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
            throw new ArgumentNullException(nameof(cipherText));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        byte[] buffer;
        try
        {
            buffer = Convert.FromBase64String(cipherText);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Cipher text is not valid Base64.", nameof(cipherText), ex);
        }

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        if (keyBytes.Length < 16)
            throw new ArgumentException("Key must be at least 16 characters long.", nameof(key));

        if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
        {
             
            Array.Resize(ref keyBytes, 16);
        }

        byte[] iv = new byte[16];
        Array.Copy(keyBytes, 0, iv, 0, 16);

        try
        {
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;
            aes.IV = iv;

            using ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(result);
        }
        catch (CryptographicException ex)
        {
            throw new CryptographicException(
                "AES decryption failed. Most likely the key/IV does not match the ciphertext, or frontend and backend encryption settings are different.",
                ex);
        }
    }
}