namespace Sms.Client.Http.Features.GetMenu;

public sealed record MenuItem(
    string Id,
    string Article,
    string Name,
    decimal Price,
    bool IsWeighted,
    string FullPath,
    IReadOnlyList<string> Barcodes);
