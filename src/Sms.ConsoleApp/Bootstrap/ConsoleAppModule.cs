using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using Sms.Client.Http.Client;
using Sms.ConsoleApp.Application;
using Sms.ConsoleApp.Configuration;
using Sms.ConsoleApp.Database;
using Sms.ConsoleApp.Output;
using Sms.Shared.Configuration;
using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Bootstrap;

public sealed class ConsoleAppModule(string settingsPath) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<JsonSettingsLoader>().As<ISettingsLoader>().SingleInstance();
        builder.Register(context =>
                context.Resolve<ISettingsLoader>().Load<ConsoleAppSettings>(settingsPath))
            .SingleInstance();

        builder.RegisterType<NLogLoggerFactory>().As<ILoggerFactory>().SingleInstance();
        builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
        builder.RegisterType<ConsoleOutput>().As<IConsoleOutput>().SingleInstance();

        builder.Register(context => new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(
                    context.Resolve<ConsoleAppSettings>().SmsHttpClient.TimeoutSeconds)
            })
            .SingleInstance();
        builder.Register(context =>
            {
                var settings = context.Resolve<ConsoleAppSettings>().SmsHttpClient;
                return new SmsHttpClientOptions
                {
                    Endpoint = settings.Endpoint,
                    Username = settings.Username,
                    Password = settings.Password
                };
            })
            .SingleInstance();
        builder.RegisterType<SmsHttpClient>().As<ISmsClient>().SingleInstance();

        builder.Register(context =>
                MenuDbContext.CreateOptions(context.Resolve<ConsoleAppSettings>().ConnectionString))
            .SingleInstance();
        builder.RegisterType<MenuDbContext>().InstancePerLifetimeScope();
        builder.RegisterType<MenuRepository>().As<IMenuRepository>().InstancePerLifetimeScope();

        builder.RegisterType<ConsoleApplication>().InstancePerLifetimeScope();
    }
}
