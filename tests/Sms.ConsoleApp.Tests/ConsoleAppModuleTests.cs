using Autofac;
using Sms.ConsoleApp.Application;
using Sms.ConsoleApp.Bootstrap;
using Sms.ConsoleApp.Configuration;
using Sms.ConsoleApp.Database;
using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Tests;

public sealed class ConsoleAppModuleTests
{
    [Fact]
    public void ResolveApplication()
    {
        var settings = new ConsoleAppSettings
        {
            ConnectionString = "Host=localhost;Database=sms",
            SmsHttpClient = new SmsHttpSettings
            {
                Endpoint = "http://localhost/sms",
                Username = "user"
            }
        };
        var builder = new ContainerBuilder();
        builder.RegisterModule(new ConsoleAppModule("unused.json"));
        builder.RegisterInstance(settings);
        using var container = builder.Build();

        Assert.NotNull(container.Resolve<ConsoleApplication>());
        Assert.Same(container.Resolve<ISmsClient>(), container.Resolve<ISmsClient>());
        Assert.Same(
            container.Resolve<PostgreSqlDatabaseInitializer>(),
            container.Resolve<PostgreSqlDatabaseInitializer>());
        Assert.Same(container.Resolve<MenuDbContext>(), container.Resolve<MenuDbContext>());
    }
}
