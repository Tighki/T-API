namespace T_API.Shared.Models;

public class CoolingPeriodItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal PriceFrom { get; set; }
    public decimal PriceTo { get; set; }
    public int CoolingDays { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

