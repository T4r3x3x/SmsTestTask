namespace Sms.Client.Http.Models;

public sealed record MenuItem(
    string Id,
    string Article,
    string Name,
    decimal Price,
    bool IsWeighted,
    string FullPath,
    IReadOnlyList<string> Barcodes);
