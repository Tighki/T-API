using T_API.Shared.DTOs;

namespace PortfolioService.Services;

public interface ICoolingCalculator
{
    Task<int> CalculateCoolingDaysAsync(decimal price, string userId);
    int? CalculateSavingsRequiredDays(decimal price, decimal currentSavings, decimal monthlySavings);
    DateTime CalculateCoolingEndDate(int coolingDays);
    bool IsCategoryBlacklisted(string category, List<string> blacklistedCategories);
}

public class CoolingCalculator : ICoolingCalculator
{
    private readonly ICoolingServiceClient _coolingServiceClient;

    public CoolingCalculator(ICoolingServiceClient coolingServiceClient)
    {
        _coolingServiceClient = coolingServiceClient;
    }

    public async Task<int> CalculateCoolingDaysAsync(decimal price, string userId)
    {
        var periods = await _coolingServiceClient.GetCoolingRangesAsync(userId);
        if (periods == null || !periods.Any())
            return 1;

        var orderedPeriods = periods.OrderBy(p => p.PriceFrom).ToList();

        foreach (var period in orderedPeriods)
        {
            if (price >= period.PriceFrom && price <= period.PriceTo)
                return period.CoolingDays;
        }

        var maxPeriod = orderedPeriods.LastOrDefault();
        if (maxPeriod != null && price > maxPeriod.PriceTo)
            return maxPeriod.CoolingDays;

        return 1;
    }

    public int? CalculateSavingsRequiredDays(decimal price, decimal currentSavings, decimal monthlySavings)
    {
        if (monthlySavings <= 0) return null;
        if (currentSavings >= price) return 0;

        var deficit = price - currentSavings;
        var dailySavings = monthlySavings / 30;
        
        if (dailySavings <= 0) return null;

        return (int)Math.Ceiling(deficit / dailySavings);
    }

    public DateTime CalculateCoolingEndDate(int coolingDays) =>
        DateTime.UtcNow.AddDays(coolingDays);

    public bool IsCategoryBlacklisted(string category, List<string> blacklistedCategories) =>
        blacklistedCategories.Any(bc => bc.Equals(category, StringComparison.OrdinalIgnoreCase));
}

