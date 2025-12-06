namespace T_API.Shared.Validation;

public static class ValidationRules
{
    public const int MaxNameLength = 200;
    public const int MaxUrlLength = 500;
    public const int MaxCategoryLength = 100;
    public const int MaxTextLength = 1000;
    public const decimal MaxPrice = 999_999_999m;

    public static bool IsValidUserId(int userId) => userId > 0;

    public static bool IsValidName(string? name) =>
        !string.IsNullOrWhiteSpace(name) && name.Length <= MaxNameLength;

    public static bool IsValidUrl(string? url) =>
        string.IsNullOrEmpty(url) || url.Length <= MaxUrlLength;

    public static bool IsValidCategory(string? category) =>
        string.IsNullOrEmpty(category) || category.Length <= MaxCategoryLength;

    public static bool IsValidPrice(decimal price) =>
        price > 0 && price <= MaxPrice;
}
