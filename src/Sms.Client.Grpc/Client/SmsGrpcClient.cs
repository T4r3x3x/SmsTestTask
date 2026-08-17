using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Sms.Client.Grpc.Features.GetMenu;
using Sms.Client.Grpc.Features.SendOrder;
using Sms.Shared.Sms;
using SmsTestService = Sms.Test.SmsTestService;

namespace Sms.Client.Grpc.Client;

public sealed class SmsGrpcClient : ISmsClient
{
    private readonly SmsTestService.SmsTestServiceClient _client;

    public SmsGrpcClient(GrpcChannel channel)
        : this(channel.CreateCallInvoker())
    {
    }

    public SmsGrpcClient(CallInvoker callInvoker) =>
        _client = new SmsTestService.SmsTestServiceClient(callInvoker);

    public async Task<IReadOnlyList<MenuItem>> GetMenuAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .GetMenuAsync(new BoolValue { Value = true }, cancellationToken: cancellationToken)
            .ResponseAsync
            .ConfigureAwait(false);

        if (!response.Success)
        {
            throw new SmsApiException("GetMenu", response.ErrorMessage);
        }

        return response.MenuItems.Select(item => item.ToMenuItem()).ToArray();
    }

    public async Task SendOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        order.Validate();
        
        var response = await _client
            .SendOrderAsync(order.ToGrpcOrder(), cancellationToken: cancellationToken)
            .ResponseAsync
            .ConfigureAwait(false);

        if (!response.Success)
        {
            throw new SmsApiException("SendOrder", response.ErrorMessage);
        }
    }
}
