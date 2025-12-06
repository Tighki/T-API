namespace T_API.Shared.Constants;

public static class Headers
{
    public const string UserId = "X-User-Id";
}

public static class ErrorMessages
{
    public const string InvalidUserId = "Invalid X-User-Id";
    public const string NotFound = "Not found";
    public const string InvalidPrice = "Invalid price";
    public const string InvalidPriceRange = "Invalid price range";
    public const string SavingsNegative = "Savings cannot be negative";
    public const string CategoryBlacklisted = "Category blacklisted";
    public const string NameRequired = "Name required";
    public const string CoolingDaysRange = "Cooling days must be 1-365";
    
    public static string MaxLength(string field, int max) => $"{field} too long (max {max} chars)";
}

public static class Routes
{
    public const string ApiV1 = "api/v1";
    public const string User = $"{ApiV1}/user";
    public const string Cooling = $"{ApiV1}/cooling";
    public const string Portfolio = $"{ApiV1}/portfolio";
    public const string Hash = $"{ApiV1}/hash";
    public const string Guid = $"{ApiV1}/guid";
}

