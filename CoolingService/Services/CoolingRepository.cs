using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;
using CoolingService.Data;

namespace CoolingService.Services;

public interface ICoolingRepository
{
    Task<List<CoolingPeriodItem>> GetCoolingPeriodsAsync(string userId);
    Task<CoolingPeriodItem> AddCoolingPeriodAsync(CoolingPeriodItem item);
}

public class CoolingRepository : ICoolingRepository
{
    private readonly CoolingDbContext _context;

    public CoolingRepository(CoolingDbContext context)
    {
        _context = context;
    }

    public async Task<List<CoolingPeriodItem>> GetCoolingPeriodsAsync(string userId) =>
        await _context.CoolingPeriods
            .Where(cp => cp.UserId == userId)
            .OrderBy(cp => cp.PriceFrom)
            .ToListAsync();

    public async Task<CoolingPeriodItem> AddCoolingPeriodAsync(CoolingPeriodItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        _context.CoolingPeriods.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }
}

