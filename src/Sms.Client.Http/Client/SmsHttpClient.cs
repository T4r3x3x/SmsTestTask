using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Sms.Client.Http.Features.GetMenu;
using Sms.Client.Http.Features.SendOrder;

namespace Sms.Client.Http.Client;

public sealed class SmsHttpClient : ISmsHttpClient
{
    private const string GetMenuCommand = "GetMenu";
    private const string SendOrderCommand = "SendOrder";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly Uri _endpoint;
    private readonly AuthenticationHeaderValue _authorization;

    public SmsHttpClient(HttpClient httpClient, SmsHttpClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        if (!Uri.TryCreate(options.Endpoint, UriKind.RelativeOrAbsolute, out var endpoint))
        {
            throw new ArgumentException("A valid HTTP endpoint is required.", nameof(options));
        }

        if (!endpoint.IsAbsoluteUri && httpClient.BaseAddress is null)
        {
            throw new ArgumentException(
                "HttpClient.BaseAddress is required when Endpoint is relative.",
                nameof(httpClient));
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            throw new ArgumentException("A Basic Authentication username is required.", nameof(options));
        }

        _httpClient = httpClient;
        _endpoint = endpoint;
        
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}"));
        
        _authorization = new AuthenticationHeaderValue("Basic", credentials);
    }

    public async Task<IReadOnlyList<MenuItem>> GetMenuAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetMenuRequest(
            GetMenuCommand,
            new GetMenuParameters(WithPrice: true));

        var response = await SendAsync<GetMenuResponse, GetMenuRequest>(request, cancellationToken)
            .ConfigureAwait(false);

        EnsureExpectedCommand(response.Command, GetMenuCommand);
        if (!response.Success)
        {
            throw new SmsApiException(GetMenuCommand, response.ErrorMessage);
        }

        if (response.Data?.MenuItems is null)
        {
            throw new SmsProtocolException("Successful GetMenu response does not contain Data.MenuItems.");
        }

        return response.Data.MenuItems.Select(item => item.ToMenuItem()).ToArray();
    }

    public async Task SendOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        order.Validate();

        var items = order.Items
            .Select(item => new OrderItemDto(
                item.Id,
                item.Quantity.ToString(CultureInfo.InvariantCulture)))
            .ToArray();
        var parameters = new SendOrderParameters(order.Id, items);
        var request = new SendOrderRequest(SendOrderCommand, parameters);

        var response = await SendAsync<SendOrderResponse, SendOrderRequest>(request, cancellationToken)
            .ConfigureAwait(false);

        EnsureExpectedCommand(response.Command, SendOrderCommand);
        if (!response.Success)
        {
            throw new SmsApiException(SendOrderCommand, response.ErrorMessage);
        }
    }

    private async Task<TResponse> SendAsync<TResponse, TRequest>(
        TRequest body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _endpoint);

        request.Content = JsonContent.Create(body, options: SerializerOptions);
        request.Headers.Authorization = _authorization;

        using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        try
        {
            var result = await response.Content
                .ReadFromJsonAsync<TResponse>(SerializerOptions, cancellationToken)
                .ConfigureAwait(false);
            return result ?? throw new SmsProtocolException("SMS server returned an empty response body.");
        }
        catch (JsonException exception)
        {
            throw new SmsProtocolException("SMS server returned invalid JSON.", exception);
        }
        catch (NotSupportedException exception)
        {
            throw new SmsProtocolException("SMS server returned an unsupported response body.", exception);
        }
    }

    private static void EnsureExpectedCommand(string? actualCommand, string expectedCommand)
    {
        if (!string.Equals(actualCommand, expectedCommand, StringComparison.Ordinal))
        {
            throw new SmsProtocolException(
                $"Expected command '{expectedCommand}' in response, but received '{actualCommand ?? "<null>"}'.");
        }
    }
}
