using System.Text.Json.Serialization;

namespace Sms.Client.Http.Features.SendOrder;

internal sealed record SendOrderRequest(
    [property: JsonPropertyName("Command")] string Command,
    [property: JsonPropertyName("CommandParameters")] SendOrderParameters CommandParameters);

internal sealed record SendOrderParameters(
    [property: JsonPropertyName("OrderId")] string OrderId,
    [property: JsonPropertyName("MenuItems")] IReadOnlyCollection<OrderItemDto> MenuItems);

internal sealed record OrderItemDto(
    [property: JsonPropertyName("Id")] string Id,
    [property: JsonPropertyName("Quantity")] string Quantity);
