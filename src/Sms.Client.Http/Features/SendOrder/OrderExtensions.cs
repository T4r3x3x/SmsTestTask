namespace Sms.Client.Http.Features.SendOrder;

internal static class OrderExtensions
{
    internal static void Validate(this Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (string.IsNullOrWhiteSpace(order.Id))
        {
            throw new ArgumentException("Order Id is required.", nameof(order));
        }

        if (order.Items is null || order.Items.Count == 0)
        {
            throw new ArgumentException("Order must contain at least one item.", nameof(order));
        }

        if (order.Items.Any(item =>
                item is null || string.IsNullOrWhiteSpace(item.Id) || item.Quantity <= 0))
        {
            throw new ArgumentException(
                "Every order item must have an Id and a quantity greater than zero.",
                nameof(order));
        }
    }
}
