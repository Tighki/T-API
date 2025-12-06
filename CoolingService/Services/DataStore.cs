using T_API.Shared.Models;
using System.Collections.Concurrent;

namespace CoolingService.Services;

public interface IDataStore
{
    List<CoolingPeriodItem> GetCoolingPeriods(string userId);
    CoolingPeriodItem AddCoolingPeriod(CoolingPeriodItem item);
}

public class InMemoryDataStore : IDataStore
{
    private readonly ConcurrentDictionary<int, CoolingPeriodItem> _coolingPeriods = new();
    private int _idCounter = 1;

    public List<CoolingPeriodItem> GetCoolingPeriods(string userId) =>
        _coolingPeriods.Values.Where(cp => cp.UserId == userId).ToList();

    public CoolingPeriodItem AddCoolingPeriod(CoolingPeriodItem item)
    {
        item.Id = Interlocked.Increment(ref _idCounter);
        item.CreatedAt = DateTime.UtcNow;
        _coolingPeriods[item.Id] = item;
        return item;
    }
}

