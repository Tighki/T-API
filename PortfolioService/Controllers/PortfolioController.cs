using Microsoft.AspNetCore.Mvc;
using T_API.Shared.DTOs;
using T_API.Shared.Models;
using PortfolioService.Services;

namespace PortfolioService.Controllers;

[ApiController]
[Route("api/v1/portfolio")]
public class PortfolioController : ControllerBase
{
    private readonly IDataStore _dataStore;
    private readonly ICoolingCalculator _coolingCalculator;
    private readonly IUserServiceClient _userServiceClient;
    private readonly TokenGenerator _tokenGenerator;
    private readonly ILogger<PortfolioController> _logger;

    public PortfolioController(
        IDataStore dataStore,
        ICoolingCalculator coolingCalculator,
        IUserServiceClient userServiceClient,
        TokenGenerator tokenGenerator,
        ILogger<PortfolioController> logger)
    {
        _dataStore = dataStore;
        _coolingCalculator = coolingCalculator;
        _userServiceClient = userServiceClient;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Получить все цели пользователя
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<GoalItemResponse>), StatusCodes.Status200OK)]
    public IActionResult GetPortfolio([FromHeader(Name = "X-User-Id")] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var goals = _dataStore.GetGoals(userId);
        var response = goals.Select(g => MapToResponse(g)).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Получить цель по ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GoalItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPortfolioItem(
        [FromHeader(Name = "X-User-Id")] string userId,
        int id)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var goal = _dataStore.GetGoal(id, userId);
        if (goal == null)
            return NotFound(new { error = "Goal not found" });

        return Ok(MapToResponse(goal));
    }

    /// <summary>
    /// Добавить новую цель (покупку)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GoalItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddGoal(
        [FromHeader(Name = "X-User-Id")] string userId,
        [FromBody] AddGoalRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Goal name is required" });

        if (request.Price <= 0)
            return BadRequest(new { error = "Price must be positive" });

        // Получаем настройки пользователя из UserService
        var preferences = await _userServiceClient.GetUserPreferencesAsync(userId);

        // Проверка черного списка категорий
        if (preferences != null && 
            _coolingCalculator.IsCategoryBlacklisted(request.Category, preferences.BlacklistedCategories))
        {
            return BadRequest(new
            {
                error = "Category is blacklisted",
                message = $"Покупки из категории '{request.Category}' запрещены вашими настройками"
            });
        }

        // Рассчитываем период охлаждения из CoolingService
        var coolingDays = await _coolingCalculator.CalculateCoolingDaysAsync(request.Price, userId);
        
        // Учитываем накопления, если включено
        if (preferences?.ConsiderSavings == true)
        {
            var savingsDays = _coolingCalculator.CalculateSavingsRequiredDays(
                request.Price,
                preferences.CurrentSavings,
                preferences.MonthlySavings);

            if (savingsDays.HasValue)
                coolingDays = Math.Max(coolingDays, savingsDays.Value);
        }

        var goal = new GoalItem
        {
            UserId = userId,
            Name = request.Name,
            Url = request.Url,
            Price = request.Price,
            Category = request.Category,
            CoolingUntil = _coolingCalculator.CalculateCoolingEndDate(coolingDays),
            Status = GoalStatus.Cooling
        };

        // Расчет дефицита средств
        if (preferences != null && request.Price > preferences.CurrentSavings)
            goal.PriceGap = request.Price - preferences.CurrentSavings;

        var saved = _dataStore.AddGoal(goal);
        var response = MapToResponse(saved);

        return CreatedAtAction(nameof(GetPortfolioItem), new { id = saved.Id }, response);
    }

    /// <summary>
    /// Обновить цель по ID
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GoalItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateGoal(
        [FromHeader(Name = "X-User-Id")] string userId,
        int id,
        [FromBody] UpdateGoalRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var existing = _dataStore.GetGoal(id, userId);
        if (existing == null)
            return NotFound(new { error = "Goal not found" });

        if (!string.IsNullOrWhiteSpace(request.Name))
            existing.Name = request.Name;

        if (request.Price.HasValue && request.Price.Value > 0)
            existing.Price = request.Price.Value;

        if (!string.IsNullOrWhiteSpace(request.Category))
            existing.Category = request.Category;

        var updated = _dataStore.UpdateGoal(existing);
        return Ok(MapToResponse(updated!));
    }

    /// <summary>
    /// Удалить цель по ID
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteGoal(
        [FromHeader(Name = "X-User-Id")] string userId,
        int id)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { error = "User ID is required in X-User-Id header" });

        var deleted = _dataStore.DeleteGoal(id, userId);
        if (!deleted)
            return NotFound(new { error = "Goal not found" });

        return NoContent();
    }

    private GoalItemResponse MapToResponse(GoalItem goal)
    {
        return new GoalItemResponse
        {
            Id = goal.Id,
            Name = goal.Name,
            Url = goal.Url,
            Price = goal.Price,
            PriceGap = goal.PriceGap,
            Category = goal.Category,
            AddedAt = goal.AddedAt,
            CoolingUntil = goal.CoolingUntil,
            Status = goal.Status,
            BearerToken = _tokenGenerator.GenerateToken(goal.UserId, goal.Id)
        };
    }
}

