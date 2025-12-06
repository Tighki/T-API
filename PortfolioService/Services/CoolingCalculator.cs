using T_API.Shared.DTOs;

namespace PortfolioService.Services;

public interface ICoolingCalculator
{
    Task<int> CalculateCoolingDaysAsync(decimal price, int userId);
    int? CalculateSavingsRequiredDays(decimal price, decimal currentSavings, decimal monthlySavings);
    DateTime CalculateCoolingEndDate(int coolingDays);
    bool IsCategoryBlacklisted(string category, List<string> blacklisted);
}

public class CoolingCalculator(ICoolingServiceClient coolingClient) : ICoolingCalculator
{
    public async Task<int> CalculateCoolingDaysAsync(decimal price, int userId)
    {
        var periods = await coolingClient.GetCoolingRangesAsync(userId);
        if (periods == null || periods.Count == 0) return 1;

        var ordered = periods.OrderBy(p => p.PriceFrom).ToList();
        foreach (var p in ordered)
            if (price >= p.PriceFrom && price <= p.PriceTo) return p.CoolingDays;

        var max = ordered.LastOrDefault();
        return max != null && price > max.PriceTo ? max.CoolingDays : 1;
    }

    public int? CalculateSavingsRequiredDays(decimal price, decimal currentSavings, decimal monthlySavings)
    {
        if (monthlySavings <= 0) return null;
        if (currentSavings >= price) return 0;
        var dailySavings = monthlySavings / 30;
        return dailySavings <= 0 ? null : (int)Math.Ceiling((price - currentSavings) / dailySavings);
    }

    public DateTime CalculateCoolingEndDate(int coolingDays) => DateTime.UtcNow.AddDays(coolingDays);

    public bool IsCategoryBlacklisted(string category, List<string> blacklisted) =>
        blacklisted.Any(b => b.Equals(category, StringComparison.OrdinalIgnoreCase));
}
