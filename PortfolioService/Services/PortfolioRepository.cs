using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;
using PortfolioService.Data;

namespace PortfolioService.Services;

public interface IPortfolioRepository
{
    Task<List<GoalItem>> GetGoalsAsync(int userId);
    Task<GoalItem?> GetGoalByIdAsync(int userId, int id);
    Task<GoalItem> AddGoalAsync(GoalItem goal);
    Task<GoalItem?> UpdateGoalAsync(GoalItem goal);
    Task<bool> DeleteGoalAsync(int userId, int id);
}

public class PortfolioRepository(PortfolioDbContext context) : IPortfolioRepository
{
    public async Task<List<GoalItem>> GetGoalsAsync(int userId) =>
        await context.Goals.Where(g => g.UserId == userId).OrderByDescending(g => g.AddedAt).ToListAsync();

    public async Task<GoalItem?> GetGoalByIdAsync(int userId, int id) =>
        await context.Goals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

    public async Task<GoalItem> AddGoalAsync(GoalItem goal)
    {
        goal.AddedAt = DateTime.UtcNow;
        context.Goals.Add(goal);
        await context.SaveChangesAsync();
        return goal;
    }

    public async Task<GoalItem?> UpdateGoalAsync(GoalItem goal)
    {
        var existing = await context.Goals.FindAsync(goal.Id);
        if (existing == null || existing.UserId != goal.UserId) return null;
        context.Entry(existing).CurrentValues.SetValues(goal);
        await context.SaveChangesAsync();
        return goal;
    }

    public async Task<bool> DeleteGoalAsync(int userId, int id)
    {
        var goal = await GetGoalByIdAsync(userId, id);
        if (goal == null) return false;
        context.Goals.Remove(goal);
        await context.SaveChangesAsync();
        return true;
    }
}
