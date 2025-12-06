using Microsoft.AspNetCore.Mvc;
using T_API.Shared.DTOs;
using T_API.Shared.Models;
using T_API.Shared.Validation;
using T_API.Shared.Constants;
using PortfolioService.Services;

namespace PortfolioService.Controllers;

[ApiController]
[Route(Routes.Portfolio)]
public class PortfolioController(
    IPortfolioRepository repository,
    ICoolingCalculator coolingCalculator,
    IUserServiceClient userServiceClient,
    TokenGenerator tokenGenerator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPortfolio([FromHeader(Name = Headers.UserId)] int userId)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        return Ok((await repository.GetGoalsAsync(userId)).Select(MapToResponse));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPortfolioItem([FromHeader(Name = Headers.UserId)] int userId, int id)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        var goal = await repository.GetGoalByIdAsync(userId, id);
        return goal == null ? NotFound(new { error = ErrorMessages.NotFound }) : Ok(MapToResponse(goal));
    }

    [HttpPost]
    public async Task<IActionResult> AddGoal([FromHeader(Name = Headers.UserId)] int userId, [FromBody] AddGoalRequest req)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        if (!ValidationRules.IsValidName(req.Name))
            return BadRequest(new { error = ErrorMessages.MaxLength("Name", ValidationRules.MaxNameLength) });
        if (!ValidationRules.IsValidUrl(req.Url))
            return BadRequest(new { error = ErrorMessages.MaxLength("URL", ValidationRules.MaxUrlLength) });
        if (!ValidationRules.IsValidCategory(req.Category))
            return BadRequest(new { error = ErrorMessages.MaxLength("Category", ValidationRules.MaxCategoryLength) });
        if (!ValidationRules.IsValidPrice(req.Price))
            return BadRequest(new { error = ErrorMessages.InvalidPrice });

        var prefs = await userServiceClient.GetUserPreferencesAsync(userId);
        if (prefs != null && coolingCalculator.IsCategoryBlacklisted(req.Category, prefs.BlacklistedCategories))
            return BadRequest(new { error = ErrorMessages.CategoryBlacklisted, message = $"'{req.Category}' запрещена" });

        var coolingDays = await coolingCalculator.CalculateCoolingDaysAsync(req.Price, userId);
        if (prefs?.ConsiderSavings == true)
        {
            var savingsDays = coolingCalculator.CalculateSavingsRequiredDays(req.Price, prefs.CurrentSavings, prefs.MonthlySavings);
            if (savingsDays.HasValue) coolingDays = Math.Max(coolingDays, savingsDays.Value);
        }

        var goal = new GoalItem
        {
            UserId = userId, Name = req.Name, Url = req.Url, Price = req.Price, Category = req.Category,
            CoolingUntil = coolingCalculator.CalculateCoolingEndDate(coolingDays), Status = GoalStatus.Cooling,
            PriceGap = prefs != null && req.Price > prefs.CurrentSavings ? req.Price - prefs.CurrentSavings : null
        };
        var saved = await repository.AddGoalAsync(goal);
        return CreatedAtAction(nameof(GetPortfolioItem), new { id = saved.Id }, MapToResponse(saved));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGoal([FromHeader(Name = Headers.UserId)] int userId, int id, [FromBody] UpdateGoalRequest req)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        var existing = await repository.GetGoalByIdAsync(userId, id);
        if (existing == null) return NotFound(new { error = ErrorMessages.NotFound });

        if (!string.IsNullOrWhiteSpace(req.Name))
        {
            if (!ValidationRules.IsValidName(req.Name))
                return BadRequest(new { error = ErrorMessages.MaxLength("Name", ValidationRules.MaxNameLength) });
            existing.Name = req.Name;
        }
        if (req.Price is > 0)
        {
            if (!ValidationRules.IsValidPrice(req.Price.Value))
                return BadRequest(new { error = ErrorMessages.InvalidPrice });
            existing.Price = req.Price.Value;
        }
        if (!string.IsNullOrWhiteSpace(req.Category))
        {
            if (!ValidationRules.IsValidCategory(req.Category))
                return BadRequest(new { error = ErrorMessages.MaxLength("Category", ValidationRules.MaxCategoryLength) });
            existing.Category = req.Category;
        }

        return Ok(MapToResponse((await repository.UpdateGoalAsync(existing))!));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGoal([FromHeader(Name = Headers.UserId)] int userId, int id)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        return await repository.DeleteGoalAsync(userId, id) ? NoContent() : NotFound(new { error = ErrorMessages.NotFound });
    }

    private GoalItemResponse MapToResponse(GoalItem g) => new()
    {
        Id = g.Id, Name = g.Name, Url = g.Url, Price = g.Price, PriceGap = g.PriceGap,
        Category = g.Category, AddedAt = g.AddedAt, CoolingUntil = g.CoolingUntil,
        Status = g.Status, BearerToken = tokenGenerator.GenerateToken(g.UserId, g.Id)
    };
}
