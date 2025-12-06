namespace T_API.Shared.Models;

public class CoolingPeriodItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal PriceFrom { get; set; }
    public decimal PriceTo { get; set; }
    public int CoolingDays { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
