using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;
using PortfolioService.Data;

namespace PortfolioService.Services;

public interface IPortfolioRepository
{
    Task<List<GoalItem>> GetGoalsAsync(string userId);
    Task<GoalItem?> GetGoalByIdAsync(string userId, int goalId);
    Task<GoalItem> AddGoalAsync(GoalItem goal);
    Task<GoalItem?> UpdateGoalAsync(GoalItem goal);
    Task<bool> DeleteGoalAsync(string userId, int goalId);
}

public class PortfolioRepository : IPortfolioRepository
{
    private readonly PortfolioDbContext _context;

    public PortfolioRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<List<GoalItem>> GetGoalsAsync(string userId) =>
        await _context.Goals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.AddedAt)
            .ToListAsync();

    public async Task<GoalItem?> GetGoalByIdAsync(string userId, int goalId) =>
        await _context.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

    public async Task<GoalItem> AddGoalAsync(GoalItem goal)
    {
        goal.AddedAt = DateTime.UtcNow;
        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();
        return goal;
    }

    public async Task<GoalItem?> UpdateGoalAsync(GoalItem goal)
    {
        var existing = await _context.Goals.FindAsync(goal.Id);
        if (existing == null || existing.UserId != goal.UserId) return null;

        _context.Entry(existing).CurrentValues.SetValues(goal);
        await _context.SaveChangesAsync();
        return goal;
    }

    public async Task<bool> DeleteGoalAsync(string userId, int goalId)
    {
        var goal = await GetGoalByIdAsync(userId, goalId);
        if (goal == null) return false;

        _context.Goals.Remove(goal);
        await _context.SaveChangesAsync();
        return true;
    }
}
