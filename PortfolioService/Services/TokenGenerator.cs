namespace PortfolioService.Services;

public class TokenGenerator
{
    public string GenerateToken(int userId, int goalId) =>
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userId}:{goalId}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"));
}
