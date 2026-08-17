using Google.Protobuf.WellKnownTypes;
using Sms.Client.Grpc.Client;
using Sms.Shared.Sms;
using GetMenuResponse = Sms.Test.GetMenuResponse;
using SendOrderResponse = Sms.Test.SendOrderResponse;

namespace Sms.Client.Grpc.Tests;

public sealed class SmsGrpcClientTests
{
    [Fact]
    public async Task GetMenuAsync_SendsExpectedRequestAndMapsResponse()
    {
        var response = new GetMenuResponse { Success = true };
        response.MenuItems.Add(new Sms.Test.MenuItem
        {
            Id = "5979224",
            Article = "A1004292",
            Name = "Каша гречневая",
            Price = 50,
            IsWeighted = false,
            FullPath = "ПРОИЗВОДСТВО\\Гарниры",
            Barcodes = { "57890975627974236429" }
        });
        var invoker = new TestCallInvoker((_, _) => response);
        var client = new SmsGrpcClient(invoker);

        var menu = await client.GetMenuAsync();

        Assert.Equal("GetMenu", invoker.MethodName);
        Assert.True(Assert.IsType<BoolValue>(invoker.Request).Value);
        var item = Assert.Single(menu);
        Assert.Equal("5979224", item.Id);
        Assert.Equal("A1004292", item.Article);
        Assert.Equal("Каша гречневая", item.Name);
        Assert.Equal(50m, item.Price);
        Assert.False(item.IsWeighted);
        Assert.Equal("ПРОИЗВОДСТВО\\Гарниры", item.FullPath);
        Assert.Equal("57890975627974236429", Assert.Single(item.Barcodes));
    }

    [Fact]
    public async Task GetMenuAsync_ThrowsApiExceptionForBusinessError()
    {
        var invoker = new TestCallInvoker((_, _) =>
            new GetMenuResponse { Success = false, ErrorMessage = "Invalid request" });
        var client = new SmsGrpcClient(invoker);

        var exception = await Assert.ThrowsAsync<SmsApiException>(() => client.GetMenuAsync());

        Assert.Equal("GetMenu", exception.Command);
        Assert.Equal("Invalid request", exception.ErrorMessage);
    }

    [Fact]
    public async Task SendOrderAsync_MapsOrderAndQuantities()
    {
        var invoker = new TestCallInvoker((_, _) => new SendOrderResponse { Success = true });
        var client = new SmsGrpcClient(invoker);
        var order = new Order(
            "62137983-1117-4D10-87C1-EF40A4348250",
            [new OrderItem("5979224", 1m), new OrderItem("9084246", 0.408m)]);

        await client.SendOrderAsync(order);

        Assert.Equal("SendOrder", invoker.MethodName);
        var request = Assert.IsType<Sms.Test.Order>(invoker.Request);
        Assert.Equal(order.Id, request.Id);
        Assert.Equal("5979224", request.OrderItems[0].Id);
        Assert.Equal(1d, request.OrderItems[0].Quantity);
        Assert.Equal("9084246", request.OrderItems[1].Id);
        Assert.Equal(0.408d, request.OrderItems[1].Quantity, 6);
    }

    [Fact]
    public async Task SendOrderAsync_ThrowsApiExceptionForBusinessError()
    {
        var invoker = new TestCallInvoker((_, _) =>
            new SendOrderResponse { Success = false, ErrorMessage = "Order rejected" });
        var client = new SmsGrpcClient(invoker);
        var order = new Order("order-1", [new OrderItem("item-1", 1m)]);

        var exception = await Assert.ThrowsAsync<SmsApiException>(() => client.SendOrderAsync(order));

        Assert.Equal("SendOrder", exception.Command);
        Assert.Equal("Order rejected", exception.ErrorMessage);
    }

    [Fact]
    public async Task SendOrderAsync_RejectsInvalidOrderBeforeGrpcCall()
    {
        var invoker = new TestCallInvoker((_, _) => new SendOrderResponse { Success = true });
        var client = new SmsGrpcClient(invoker);
        var order = new Order("order-1", [new OrderItem("item-1", 0m)]);

        await Assert.ThrowsAsync<ArgumentException>(() => client.SendOrderAsync(order));

        Assert.Equal(0, invoker.CallCount);
    }
}
