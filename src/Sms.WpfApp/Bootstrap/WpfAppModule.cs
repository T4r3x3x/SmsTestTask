using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using Sms.Shared.Configuration;
using Sms.WpfApp.Configuration;
using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp.Bootstrap;

public sealed class WpfAppModule(string settingsPath) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        ReactiveUiBootstrapper.Initialize();

        builder.RegisterType<JsonSettingsLoader>().As<ISettingsLoader>().SingleInstance();
        builder.Register(context =>
                context.Resolve<ISettingsLoader>().Load<AppSettings>(settingsPath))
            .SingleInstance();

        builder.RegisterType<NLogLoggerFactory>().As<ILoggerFactory>().SingleInstance();
        builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
        builder.RegisterType<EnvironmentVariableStore>()
            .As<IEnvironmentVariableStore>()
            .SingleInstance();
        builder.RegisterType<EnvironmentVariablesViewModel>().SingleInstance();
        builder.RegisterType<MainWindow>().SingleInstance();
    }
}
