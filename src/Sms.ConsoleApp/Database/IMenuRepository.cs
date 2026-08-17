using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Database;

public interface IMenuRepository
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task UpsertAsync(
        IReadOnlyCollection<MenuItem> menu,
        CancellationToken cancellationToken = default);
}
