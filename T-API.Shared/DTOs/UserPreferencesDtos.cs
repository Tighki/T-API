using T_API.Shared.Models;

namespace T_API.Shared.DTOs;

public class UserPreferencesRequest
{
    public List<string> BlacklistedCategories { get; set; } = new();
    public decimal MonthlySavings { get; set; }
    public decimal CurrentSavings { get; set; }
    public bool ConsiderSavings { get; set; }
    public NotificationFrequency NotificationFrequency { get; set; }
}

public class UserPreferencesResponse
{
    public string UserId { get; set; } = string.Empty;
    public List<string> BlacklistedCategories { get; set; } = new();
    public decimal MonthlySavings { get; set; }
    public decimal CurrentSavings { get; set; }
    public bool ConsiderSavings { get; set; }
    public NotificationFrequency NotificationFrequency { get; set; }
}

