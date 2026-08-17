using System.Net;

namespace Sms.Client.Http.Tests;

internal sealed class TestHttpMessageHandler(
    Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
    : HttpMessageHandler
{
    public HttpRequestMessage? Request { get; private set; }

    public string? RequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Request = request;
        RequestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);
        return await responder(request, cancellationToken);
    }

    public static HttpResponseMessage Json(string json, HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
}
