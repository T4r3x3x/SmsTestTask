namespace Sms.ConsoleApp.Configuration;

public sealed record ConsoleAppSettings
{
    public string ConnectionString { get; init; } = string.Empty;

    public SmsHttpSettings SmsHttpClient { get; init; } = new();
}

public sealed record SmsHttpSettings
{
    public string Endpoint { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 30;
}
