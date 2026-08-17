namespace Sms.Client.Http.Features.SendOrder;

public sealed record Order(string Id, IReadOnlyCollection<OrderItem> Items);
