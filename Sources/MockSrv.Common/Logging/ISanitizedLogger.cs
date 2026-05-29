namespace MockSrv.Common.Logging;

public interface ISanitizedLogger<T>
{
    void Info(string? message, params object?[] args);

    void Warning(string? message, params object?[] args);

    void Warning(Exception exception, string? message, params object?[] args);

    void Error(string? message, params object?[] args);

    void Error(Exception exception, string? message, params object?[] args);
}
