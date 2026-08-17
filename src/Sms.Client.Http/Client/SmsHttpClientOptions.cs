namespace Sms.Client.Http.Client;

public sealed class SmsHttpClientOptions
{
    public string Endpoint { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
