using Microsoft.AspNetCore.Mvc;
using T_API.Shared.DTOs;
using T_API.Shared.Models;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/user")]
public class UserController : ControllerBase
{
    private readonly IDataStore _dataStore;
    private readonly ILogger<UserController> _logger;

    public UserController(IDataStore dataStore, ILogger<UserController> logger)
    {
        _dataStore = dataStore;
        _logger = logger;
    }

    /// <summary>
    /// Получить настройки пользователя
    /// </summary>
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPreferences([FromHeader(Name = "X-User-Id")] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var preferences = _dataStore.GetUserPreferences(userId);
        if (preferences == null)
            return NotFound(new { error = "User preferences not found" });

        var response = new UserPreferencesResponse
        {
            UserId = preferences.UserId,
            BlacklistedCategories = preferences.BlacklistedCategories,
            MonthlySavings = preferences.MonthlySavings,
            CurrentSavings = preferences.CurrentSavings,
            ConsiderSavings = preferences.ConsiderSavings,
            NotificationFrequency = preferences.NotificationFrequency
        };

        return Ok(response);
    }

    /// <summary>
    /// Обновить настройки пользователя
    /// </summary>
    [HttpPut("preferences")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    public IActionResult UpdatePreferences(
        [FromHeader(Name = "X-User-Id")] string userId,
        [FromBody] UserPreferencesRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        if (request.MonthlySavings < 0 || request.CurrentSavings < 0)
            return BadRequest(new { error = "Savings values cannot be negative" });

        var preferences = new UserPreferences
        {
            UserId = userId,
            BlacklistedCategories = request.BlacklistedCategories,
            MonthlySavings = request.MonthlySavings,
            CurrentSavings = request.CurrentSavings,
            ConsiderSavings = request.ConsiderSavings,
            NotificationFrequency = request.NotificationFrequency
        };

        var saved = _dataStore.SaveUserPreferences(preferences);
        var response = new UserPreferencesResponse
        {
            UserId = saved.UserId,
            BlacklistedCategories = saved.BlacklistedCategories,
            MonthlySavings = saved.MonthlySavings,
            CurrentSavings = saved.CurrentSavings,
            ConsiderSavings = saved.ConsiderSavings,
            NotificationFrequency = saved.NotificationFrequency
        };

        return Ok(response);
    }

    /// <summary>
    /// Проверить наличие пользователя
    /// </summary>
    [HttpGet("exists")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult CheckUserExists([FromHeader(Name = "X-User-Id")] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var preferences = _dataStore.GetUserPreferences(userId);
        return Ok(new { exists = preferences != null, userId });
    }
}

