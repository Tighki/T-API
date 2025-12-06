using Microsoft.AspNetCore.Mvc;
using T_API.Shared.DTOs;
using T_API.Shared.Models;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/user")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserRepository repository, ILogger<UserController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Получить настройки пользователя
    /// </summary>
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPreferences([FromHeader(Name = "X-User-Id")] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var preferences = await _repository.GetUserPreferencesAsync(userId);
        if (preferences == null)
            return NotFound(new { error = "User preferences not found" });

        return Ok(MapToResponse(preferences));
    }

    /// <summary>
    /// Обновить настройки пользователя
    /// </summary>
    [HttpPut("preferences")]
    [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePreferences(
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

        var saved = await _repository.SaveUserPreferencesAsync(preferences);
        return Ok(MapToResponse(saved));
    }

    /// <summary>
    /// Проверить наличие пользователя
    /// </summary>
    [HttpGet("exists")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckUserExists([FromHeader(Name = "X-User-Id")] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var preferences = await _repository.GetUserPreferencesAsync(userId);
        return Ok(new { exists = preferences != null, userId });
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
