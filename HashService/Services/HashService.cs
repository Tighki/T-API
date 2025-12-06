using System.Security.Cryptography;
using System.Text;

namespace HashService.Services;

public interface IHashService
{
    string HashSha256(string text, byte[]? salt = null);
    string HashMd5(string text, byte[]? salt = null);
    byte[] GenerateSalt(int length = 64);
    bool Verify(string text, string hash, byte[]? salt = null);
}

public class HashService : IHashService
{
    public byte[] GenerateSalt(int length = 64)
    {
        var salt = new byte[length];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }

    public string HashSha256(string text, byte[]? salt = null)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        if (salt != null) bytes = [.. salt, .. bytes];
        return Convert.ToBase64String(SHA256.HashData(bytes));
    }

    public string HashMd5(string text, byte[]? salt = null)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        if (salt != null) bytes = [.. salt, .. bytes];
        return Convert.ToBase64String(MD5.HashData(bytes));
    }

    public bool Verify(string text, string hash, byte[]? salt = null) =>
        HashSha256(text, salt) == hash;
}
