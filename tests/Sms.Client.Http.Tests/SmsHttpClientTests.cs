using System.Net;
using System.Text.Json;
using Sms.Client.Http.Client;
using Sms.Shared.Sms;

namespace Sms.Client.Http.Tests;

public sealed class SmsHttpClientTests
{
    [Fact]
    public async Task GetMenuAsync_SendsExpectedRequestAndMapsSuccessfulResponse()
    {
        const string responseJson = """
            {
              "Command": "GetMenu",
              "Success": true,
              "ErrorMessage": "",
              "Data": {
                "MenuItems": [
                  {
                    "Id": "5979224",
                    "Article": "A1004292",
                    "Name": "Каша гречневая",
                    "Price": 50,
                    "IsWeighted": false,
                    "FullPath": "ПРОИЗВОДСТВО\\Гарниры",
                    "Barcodes": ["57890975627974236429"]
                  }
                ]
              }
            }
            """;

        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(TestHttpMessageHandler.Json(responseJson)));

        var client = CreateClient(handler);

        var menu = await client.GetMenuAsync();

        var item = Assert.Single(menu);

        Assert.Equal("5979224", item.Id);
        Assert.Equal("A1004292", item.Article);
        Assert.Equal("Каша гречневая", item.Name);
        Assert.Equal(50m, item.Price);
        Assert.False(item.IsWeighted);
        Assert.Equal("ПРОИЗВОДСТВО\\Гарниры", item.FullPath);
        Assert.Equal("57890975627974236429", Assert.Single(item.Barcodes));

        Assert.NotNull(handler.Request);

        Assert.Equal(HttpMethod.Post, handler.Request.Method);
        Assert.Equal(new Uri("https://example.test/api/sms"), handler.Request.RequestUri);
        Assert.Equal("Basic", handler.Request.Headers.Authorization?.Scheme);

        var expectedCredentials = Convert.ToBase64String("demo:secret"u8.ToArray());
        Assert.Equal(expectedCredentials, handler.Request.Headers.Authorization?.Parameter);

        using var body = JsonDocument.Parse(Assert.IsType<string>(handler.RequestBody));
        Assert.Equal("GetMenu", body.RootElement.GetProperty("Command").GetString());
        Assert.True(body.RootElement.GetProperty("CommandParameters").GetProperty("WithPrice").GetBoolean());
    }

    [Fact]
    public async Task GetMenuAsync_ThrowsProtocolExceptionForInvalidJson()
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            Task.FromResult(TestHttpMessageHandler.Json("not-json")));

        var client = CreateClient(handler);

        await Assert.ThrowsAsync<SmsProtocolException>(() => client.GetMenuAsync());
    }

    [Fact]
    public async Task GetMenuAsync_PreservesTransportError()
    {
        var handler = new TestHttpMessageHandler((_, _) => Task.FromResult(
            TestHttpMessageHandler.Json("{}", HttpStatusCode.ServiceUnavailable)));

        var client = CreateClient(handler);

        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetMenuAsync());
    }

    [Fact]
    public async Task GetMenuAsync_ThrowsApiExceptionForBusinessErrorWithHttp200()
    {
        var handler = new TestHttpMessageHandler((_, _) => Task.FromResult(
            TestHttpMessageHandler.Json(
                """
                {
                  "Command": "GetMenu",
                  "Success": false,
                  "ErrorMessage": "Invalid request"
                }
                """)));

        var client = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<SmsApiException>(() => client.GetMenuAsync());

        Assert.Equal("GetMenu", exception.Command);
        Assert.Equal("Invalid request", exception.ErrorMessage);
    }

    [Fact]
    public async Task SendOrderAsync_SerializesQuantitiesAsInvariantStrings()
    {
        var handler = new TestHttpMessageHandler((_, _) => Task.FromResult(
            TestHttpMessageHandler.Json(
                """
                {
                  "Command": "SendOrder",
                  "Success": true,
                  "ErrorMessage": ""
                }
                """)));

        var client = CreateClient(handler);
        var order = new Order(
            "62137983-1117-4D10-87C1-EF40A4348250",
            [new OrderItem("5979224", 1m), new OrderItem("9084246", 0.408m)]);

        await client.SendOrderAsync(order);

        using var body = JsonDocument.Parse(Assert.IsType<string>(handler.RequestBody));

        var root = body.RootElement;
        Assert.Equal("SendOrder", root.GetProperty("Command").GetString());

        var parameters = root.GetProperty("CommandParameters");
        Assert.Equal(order.Id, parameters.GetProperty("OrderId").GetString());

        var items = parameters.GetProperty("MenuItems").EnumerateArray().ToArray();
        Assert.Equal("1", items[0].GetProperty("Quantity").GetString());
        Assert.Equal("0.408", items[1].GetProperty("Quantity").GetString());
    }

    [Fact]
    public async Task SendOrderAsync_ThrowsApiExceptionForBusinessError()
    {
        var handler = new TestHttpMessageHandler((_, _) => Task.FromResult(
            TestHttpMessageHandler.Json(
                """
                {
                  "Command": "SendOrder",
                  "Success": false,
                  "ErrorMessage": "Order rejected"
                }
                """)));

        var client = CreateClient(handler);
        var order = new Order("order-1", [new OrderItem("item-1", 1m)]);

        var exception = await Assert.ThrowsAsync<SmsApiException>(() => client.SendOrderAsync(order));

        Assert.Equal("SendOrder", exception.Command);
        Assert.Equal("Order rejected", exception.ErrorMessage);
    }

    [Fact]
    public async Task SendOrderAsync_RejectsInvalidOrderBeforeSendingRequest()
    {
        var handler = new TestHttpMessageHandler((_, _) =>
            throw new InvalidOperationException("Request must not be sent."));

        var client = CreateClient(handler);
        var order = new Order("order-1", [new OrderItem("item-1", 0m)]);

        await Assert.ThrowsAsync<ArgumentException>(() => client.SendOrderAsync(order));

        Assert.Null(handler.Request);
    }

    private static SmsHttpClient CreateClient(TestHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler);
        var options = new SmsHttpClientOptions
        {
            Endpoint = "https://example.test/api/sms",
            Username = "demo",
            Password = "secret"
        };

        return new SmsHttpClient(httpClient, options);
    }
}