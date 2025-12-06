namespace T_API.Shared.Models;

public class UserPreferences
{
    public string UserId { get; set; } = string.Empty;
    public List<string> BlacklistedCategories { get; set; } = new();
    public decimal MonthlySavings { get; set; }
    public decimal CurrentSavings { get; set; }
    public bool ConsiderSavings { get; set; }
    public NotificationFrequency NotificationFrequency { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum NotificationFrequency
{
    Daily,
    Weekly,
    Monthly
}

