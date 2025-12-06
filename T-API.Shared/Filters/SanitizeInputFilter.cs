using Microsoft.AspNetCore.Mvc.Filters;
using T_API.Shared.Helpers;

namespace T_API.Shared.Filters;

public class SanitizeInputFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg == null) continue;
            SanitizeObject(arg);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private static void SanitizeObject(object obj)
    {
        var props = obj.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

        foreach (var prop in props)
        {
            var value = prop.GetValue(obj) as string;
            if (!string.IsNullOrEmpty(value))
                prop.SetValue(obj, InputSanitizer.Sanitize(value));
        }
    }
}
