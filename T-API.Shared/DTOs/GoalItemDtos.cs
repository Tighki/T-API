using T_API.Shared.Models;

namespace T_API.Shared.DTOs;

public class AddGoalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class UpdateGoalRequest
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }
}

public class GoalItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceGap { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
    public DateTime? CoolingUntil { get; set; }
    public GoalStatus Status { get; set; }
    public string BearerToken { get; set; } = string.Empty;
}

