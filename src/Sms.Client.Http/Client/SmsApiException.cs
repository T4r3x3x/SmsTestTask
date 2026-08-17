namespace Sms.Client.Http.Client;

public sealed class SmsApiException : Exception
{
    public SmsApiException(string command, string? errorMessage)
        : base(string.IsNullOrWhiteSpace(errorMessage)
            ? $"SMS server rejected command '{command}'."
            : errorMessage)
    {
        Command = command;
        ErrorMessage = errorMessage ?? string.Empty;
    }

    public string Command { get; }

    public string ErrorMessage { get; }
}
