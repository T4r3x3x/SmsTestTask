namespace Sms.Shared.Sms;

public sealed record Order(string Id, IReadOnlyCollection<OrderItem> Items);
