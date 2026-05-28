namespace MockSrv.Domain.Globals;

public static class ErrorCodes
{
    public const string DUPLICATE_REQUEST = "DUPLICATE_REQUEST";

    public const int CODE_DUPLICATE_KEY_SQLLITE = 2067;

    public const int CODE_DUPLICATE_KEY_SQLSERVER = 2627;
}