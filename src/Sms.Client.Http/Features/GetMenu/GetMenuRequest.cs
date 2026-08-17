using System.Text.Json.Serialization;

namespace Sms.Client.Http.Features.GetMenu;

internal sealed record GetMenuRequest(
    [property: JsonPropertyName("Command")] string Command,
    [property: JsonPropertyName("CommandParameters")] GetMenuParameters CommandParameters);

internal sealed record GetMenuParameters(
    [property: JsonPropertyName("WithPrice")] bool WithPrice);
