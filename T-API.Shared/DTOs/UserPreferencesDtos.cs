using T_API.Shared.Models;

namespace T_API.Shared.DTOs;

public class UserPreferencesRequest
{
    public List<string> BlacklistedCategories { get; set; } = [];
    public decimal MonthlySavings { get; set; }
    public decimal CurrentSavings { get; set; }
    public bool ConsiderSavings { get; set; }
    public NotificationFrequency NotificationFrequency { get; set; }
}

public class UserPreferencesResponse
{
    public int UserId { get; set; }
    public List<string> BlacklistedCategories { get; set; } = [];
    public decimal MonthlySavings { get; set; }
    public decimal CurrentSavings { get; set; }
    public bool ConsiderSavings { get; set; }
    public NotificationFrequency NotificationFrequency { get; set; }
}
