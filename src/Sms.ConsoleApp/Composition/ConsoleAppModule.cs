using Autofac;
using Sms.Client.Http.Client;
using Sms.ConsoleApp.Application;
using Sms.ConsoleApp.Configuration;
using Sms.ConsoleApp.Database;
using Sms.ConsoleApp.Output;
using Sms.Shared.Logging;
using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Composition;

public sealed class ConsoleAppModule(ConsoleAppSettings settings) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => new DailyFileLogger(AppContext.BaseDirectory, "test-sms-console-app"))
            .SingleInstance();
        builder.RegisterType<ConsoleOutput>().As<IConsoleOutput>().SingleInstance();

        builder.Register(_ => new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(settings.SmsHttpClient.TimeoutSeconds)
            })
            .SingleInstance();
        builder.Register(_ => new SmsHttpClientOptions
            {
                Endpoint = settings.SmsHttpClient.Endpoint,
                Username = settings.SmsHttpClient.Username,
                Password = settings.SmsHttpClient.Password
            })
            .SingleInstance();
        builder.RegisterType<SmsHttpClient>().As<ISmsClient>().SingleInstance();

        builder.Register(_ => MenuDbContext.CreateOptions(settings.ConnectionString))
            .SingleInstance();
        builder.RegisterType<MenuDbContext>().InstancePerLifetimeScope();
        builder.RegisterType<MenuRepository>().As<IMenuRepository>().InstancePerLifetimeScope();

        builder.RegisterType<ConsoleApplication>().InstancePerLifetimeScope();
    }
}