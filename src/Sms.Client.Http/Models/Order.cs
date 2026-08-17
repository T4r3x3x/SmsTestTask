namespace Sms.Client.Http.Models;

public sealed record Order(string Id, IReadOnlyCollection<OrderItem> Items);
