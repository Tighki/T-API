namespace T_API.Shared.DTOs;

public class CoolingPeriodRequest
{
    public decimal PriceFrom { get; set; }
    public decimal PriceTo { get; set; }
    public int CoolingDays { get; set; }
}

public class CoolingPeriodResponse
{
    public int Id { get; set; }
    public decimal PriceFrom { get; set; }
    public decimal PriceTo { get; set; }
    public int CoolingDays { get; set; }
}

