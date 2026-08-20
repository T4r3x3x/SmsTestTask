using Microsoft.EntityFrameworkCore;
using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Database;

public sealed class MenuRepository(
    MenuDbContext database,
    PostgreSqlDatabaseInitializer databaseInitializer) : IMenuRepository
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await databaseInitializer.EnsureExistsAsync(cancellationToken);
        await database.Database.MigrateAsync(cancellationToken);
    }

    public async Task UpsertAsync(
        IReadOnlyCollection<MenuItem> menu,
        CancellationToken cancellationToken = default)
    {
        var ids = menu.Select(item => item.Id).ToArray();
        var items = database.MenuItems;
        var existing = await items
            .Where(item => ids.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, cancellationToken);

        foreach (var item in menu)
        {
            if (existing.TryGetValue(item.Id, out var entity))
            {
                entity.Update(item);
            }
            else
            {
                items.Add(MenuItemEntity.From(item));
            }
        }

        await database.SaveChangesAsync(cancellationToken);
    }
}
