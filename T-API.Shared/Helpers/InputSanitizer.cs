using System.Text.RegularExpressions;
using System.Web;

namespace T_API.Shared.Helpers;

public static partial class InputSanitizer
{
    public static string Sanitize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        
        var result = HttpUtility.HtmlEncode(input);
        result = SqlPatternRegex().Replace(result, string.Empty);
        result = ScriptTagRegex().Replace(result, string.Empty);
        return result.Trim();
    }

    [GeneratedRegex(@"('|--|;|/\*|\*/|xp_|sp_|exec|execute|insert|select|delete|update|drop|alter|create|truncate|union)", RegexOptions.IgnoreCase)]
    private static partial Regex SqlPatternRegex();

    [GeneratedRegex(@"<[^>]*script[^>]*>|javascript:|on\w+\s*=", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptTagRegex();
}
