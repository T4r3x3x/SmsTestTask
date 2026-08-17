using Microsoft.EntityFrameworkCore.Design;

namespace Sms.ConsoleApp.Database;

public sealed class MenuDbContextFactory : IDesignTimeDbContextFactory<MenuDbContext>
{
    public MenuDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SMS_CONNECTION_STRING") ??
            throw new InvalidOperationException("Set SMS_CONNECTION_STRING to manage migrations.");

        return new MenuDbContext(MenuDbContext.CreateOptions(connectionString));
    }
}