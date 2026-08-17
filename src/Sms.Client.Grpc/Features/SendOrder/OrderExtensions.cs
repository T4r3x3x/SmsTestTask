using Sms.Shared.Sms;
using GrpcOrder = Sms.Test.Order;
using GrpcOrderItem = Sms.Test.OrderItem;

namespace Sms.Client.Grpc.Features.SendOrder;

internal static class OrderExtensions
{
    internal static GrpcOrder ToGrpcOrder(this Order order)
    {
        var result = new GrpcOrder { Id = order.Id };
        result.OrderItems.AddRange(order.Items.Select(item => new GrpcOrderItem
        {
            Id = item.Id,
            Quantity = (double)item.Quantity
        }));
        return result;
    }
}
