using Microsoft.AspNetCore.Mvc;
using T_API.Shared.DTOs;
using T_API.Shared.Models;
using T_API.Shared.Validation;
using T_API.Shared.Constants;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route(Routes.User)]
public class UserController(IUserRepository repository) : ControllerBase
{
    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences([FromHeader(Name = Headers.UserId)] int userId)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        var prefs = await repository.GetUserPreferencesAsync(userId);
        return prefs == null ? NotFound(new { error = ErrorMessages.NotFound }) : Ok(MapToResponse(prefs));
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromHeader(Name = Headers.UserId)] int userId, [FromBody] UserPreferencesRequest req)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        if (req.MonthlySavings < 0 || req.CurrentSavings < 0)
            return BadRequest(new { error = ErrorMessages.SavingsNegative });
        if (req.BlacklistedCategories.Any(c => c.Length > ValidationRules.MaxCategoryLength))
            return BadRequest(new { error = ErrorMessages.MaxLength("Category", ValidationRules.MaxCategoryLength) });

        var prefs = new UserPreferences
        {
            UserId = userId,
            BlacklistedCategories = req.BlacklistedCategories,
            MonthlySavings = req.MonthlySavings,
            CurrentSavings = req.CurrentSavings,
            ConsiderSavings = req.ConsiderSavings,
            NotificationFrequency = req.NotificationFrequency
        };
        return Ok(MapToResponse(await repository.SaveUserPreferencesAsync(prefs)));
    }

    [HttpGet("exists")]
    public async Task<IActionResult> CheckUserExists([FromHeader(Name = Headers.UserId)] int userId)
    {
        if (!ValidationRules.IsValidUserId(userId))
            return BadRequest(new { error = ErrorMessages.InvalidUserId });
        var prefs = await repository.GetUserPreferencesAsync(userId);
        return Ok(new { exists = prefs != null, userId });
    }

    private static UserPreferencesResponse MapToResponse(UserPreferences p) => new()
    {
        UserId = p.UserId,
        BlacklistedCategories = p.BlacklistedCategories,
        MonthlySavings = p.MonthlySavings,
        CurrentSavings = p.CurrentSavings,
        ConsiderSavings = p.ConsiderSavings,
        NotificationFrequency = p.NotificationFrequency
    };
}
