using System.Text.RegularExpressions;

namespace MockSrv.Common.Extensions;

public static partial class LoggerExtensions
{
    [GeneratedRegex(@"\p{C}+")]
    private static partial Regex NonPrintableCharsRegex();

    public static string? Sanitized(this string? message)
    {
        if (string.IsNullOrWhiteSpace(message)) return string.Empty;

        return NonPrintableCharsRegex().Replace(message, "");
    }

    public static object?[] Sanitized(this object?[] args)
    {
        if (args is null || args.Length == 0) return [];

        return [.. args.Select(arg => (object?)arg.ToString())];
    }
}
