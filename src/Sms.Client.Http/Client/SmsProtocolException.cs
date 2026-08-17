namespace Sms.Client.Http.Client;

public sealed class SmsProtocolException : Exception
{
    public SmsProtocolException(string message)
        : base(message)
    {
    }

    public SmsProtocolException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
