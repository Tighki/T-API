namespace T_API.Shared.Models;

public class GoalItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceGap { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CoolingUntil { get; set; }
    public GoalStatus Status { get; set; } = GoalStatus.Pending;
    public DateTime? CompletedAt { get; set; }
}

public enum GoalStatus
{
    Pending,
    Cooling,
    Approved,
    Rejected,
    Purchased
}
