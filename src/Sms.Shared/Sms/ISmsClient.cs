namespace Sms.Shared.Sms;

public interface ISmsClient
{
    Task<IReadOnlyList<MenuItem>> GetMenuAsync(CancellationToken cancellationToken = default);

    Task SendOrderAsync(Order order, CancellationToken cancellationToken = default);
}
