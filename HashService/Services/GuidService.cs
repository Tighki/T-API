using System.Security.Cryptography;

namespace HashService.Services;

public interface IGuidService
{
    string Generate();
}

public class GuidService : IGuidService
{
    public string Generate()
    {
        var baseGuid = Guid.NewGuid().ToByteArray();
        var timestamp = BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        var hash = SHA256.HashData([.. timestamp, .. baseGuid]);
        return new Guid(hash.Take(16).ToArray()).ToString();
    }
}
