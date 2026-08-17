using Sms.Client.Http.Features.GetMenu;
using Sms.Client.Http.Features.SendOrder;

namespace Sms.Client.Http.Client;

public interface ISmsHttpClient
{
    Task<IReadOnlyList<MenuItem>> GetMenuAsync(CancellationToken cancellationToken = default);

    Task SendOrderAsync(Order order, CancellationToken cancellationToken = default);
}
