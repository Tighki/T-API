using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;
using CoolingService.Data;

namespace CoolingService.Services;

public interface ICoolingRepository
{
    Task<List<CoolingPeriodItem>> GetCoolingPeriodsAsync(int userId);
    Task<CoolingPeriodItem> AddCoolingPeriodAsync(CoolingPeriodItem item);
}

public class CoolingRepository(CoolingDbContext context) : ICoolingRepository
{
    public async Task<List<CoolingPeriodItem>> GetCoolingPeriodsAsync(int userId) =>
        await context.CoolingPeriods.Where(cp => cp.UserId == userId).OrderBy(cp => cp.PriceFrom).ToListAsync();

    public async Task<CoolingPeriodItem> AddCoolingPeriodAsync(CoolingPeriodItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        context.CoolingPeriods.Add(item);
        await context.SaveChangesAsync();
        return item;
    }
}
