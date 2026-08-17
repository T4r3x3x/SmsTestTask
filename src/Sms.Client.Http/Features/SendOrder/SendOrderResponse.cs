using System.Text.Json.Serialization;

namespace Sms.Client.Http.Features.SendOrder;

internal sealed record SendOrderResponse
{
    [JsonPropertyName("Command")]
    public string? Command { get; init; }

    [JsonPropertyName("Success")]
    public bool Success { get; init; }

    [JsonPropertyName("ErrorMessage")]
    public string? ErrorMessage { get; init; }
}
