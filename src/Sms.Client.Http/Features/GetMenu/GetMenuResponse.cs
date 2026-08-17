using System.Text.Json.Serialization;

namespace Sms.Client.Http.Features.GetMenu;

internal sealed record GetMenuResponse
{
    [JsonPropertyName("Command")]
    public string? Command { get; init; }

    [JsonPropertyName("Success")]
    public bool Success { get; init; }

    [JsonPropertyName("ErrorMessage")]
    public string? ErrorMessage { get; init; }

    [JsonPropertyName("Data")]
    public GetMenuData? Data { get; init; }
}

internal sealed record GetMenuData
{
    [JsonPropertyName("MenuItems")]
    public List<MenuItemDto>? MenuItems { get; init; }
}

internal sealed record MenuItemDto
{
    [JsonPropertyName("Id")]
    public string? Id { get; init; }

    [JsonPropertyName("Article")]
    public string? Article { get; init; }

    [JsonPropertyName("Name")]
    public string? Name { get; init; }

    [JsonPropertyName("Price")]
    public decimal Price { get; init; }

    [JsonPropertyName("IsWeighted")]
    public bool IsWeighted { get; init; }

    [JsonPropertyName("FullPath")]
    public string? FullPath { get; init; }

    [JsonPropertyName("Barcodes")]
    public List<string>? Barcodes { get; init; }
}
