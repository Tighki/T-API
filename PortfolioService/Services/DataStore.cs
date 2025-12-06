using T_API.Shared.Models;
using System.Collections.Concurrent;

namespace PortfolioService.Services;

public interface IDataStore
{
    List<GoalItem> GetGoals(string userId);
    GoalItem? GetGoal(int id, string userId);
    GoalItem AddGoal(GoalItem goal);
    GoalItem? UpdateGoal(GoalItem goal);
    bool DeleteGoal(int id, string userId);
}

public class InMemoryDataStore : IDataStore
{
    private readonly ConcurrentDictionary<int, GoalItem> _goals = new();
    private int _idCounter = 1;

    public List<GoalItem> GetGoals(string userId) =>
        _goals.Values.Where(g => g.UserId == userId).ToList();

    public GoalItem? GetGoal(int id, string userId) =>
        _goals.TryGetValue(id, out var goal) && goal.UserId == userId ? goal : null;

    public GoalItem AddGoal(GoalItem goal)
    {
        goal.Id = Interlocked.Increment(ref _idCounter);
        goal.AddedAt = DateTime.UtcNow;
        _goals[goal.Id] = goal;
        return goal;
    }

    public GoalItem? UpdateGoal(GoalItem goal)
    {
        if (_goals.TryGetValue(goal.Id, out var existing) && existing.UserId == goal.UserId)
        {
            _goals[goal.Id] = goal;
            return goal;
        }
        return null;
    }

    public bool DeleteGoal(int id, string userId)
    {
        if (_goals.TryGetValue(id, out var goal) && goal.UserId == userId)
            return _goals.TryRemove(id, out _);
        return false;
    }
}

