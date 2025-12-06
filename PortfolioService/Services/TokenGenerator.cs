namespace PortfolioService.Services;

public class TokenGenerator
{
    public string GenerateToken(string userId, int goalId)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var data = $"{userId}:{goalId}:{timestamp}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(bytes);
    }
}

