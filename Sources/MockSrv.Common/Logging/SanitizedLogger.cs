using Microsoft.Extensions.Logging;
using MockSrv.Common.Extensions;

namespace MockSrv.Common.Logging;

public sealed class SanitizedLogger<T>(ILogger<T> logger) : ISanitizedLogger<T>
{
    public void Info(string? message, params object?[] args)
    {
        logger.LogInformation(message.Sanitized(), args.Sanitized());
    }

    public void Warning(string? message, params object?[] args)
    {
        logger.LogWarning(message.Sanitized(), args.Sanitized());
    }

    public void Warning(Exception exception, string? message, params object?[] args)
    {
        logger.LogWarning(exception, message.Sanitized(), args.Sanitized());
    }

    public void Error(string? message, params object?[] args)
    {
        logger.LogError(message.Sanitized(), args.Sanitized());
    }

    public void Error(Exception exception, string? message, params object?[] args)
    {
        logger.LogError(exception, message.Sanitized(), args.Sanitized());
    }
}
