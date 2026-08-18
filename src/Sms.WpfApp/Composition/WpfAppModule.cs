using Autofac;
using Sms.Shared.Configuration;
using Sms.Shared.Logging;
using Sms.WpfApp.Configuration;
using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp.Composition;

public sealed class WpfAppModule(string settingsPath) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        ReactiveUiBootstrapper.Initialize();

        builder.RegisterType<JsonSettingsLoader>().As<ISettingsLoader>().SingleInstance();
        builder.Register(context =>
                context.Resolve<ISettingsLoader>().Load<AppSettings>(settingsPath))
            .SingleInstance();

        builder.Register(_ => new DailyFileLogger(AppContext.BaseDirectory, "test-sms-wpf-app"))
            .As<IAppLogger>()
            .SingleInstance();
        builder.RegisterType<EnvironmentVariableStore>()
            .As<IEnvironmentVariableStore>()
            .SingleInstance();
        builder.RegisterType<EnvironmentVariablesViewModel>().SingleInstance();
        builder.RegisterType<MainWindow>().SingleInstance();
    }
}
