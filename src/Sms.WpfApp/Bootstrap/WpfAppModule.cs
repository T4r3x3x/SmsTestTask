using System.IO;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using Sms.WpfApp.Configuration;
using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp.Bootstrap;

public sealed class WpfAppModule(string settingsPath) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        ReactiveUiBootstrapper.Initialize();

        builder.Register(_ =>
            {
                var fullSettingsPath = Path.GetFullPath(settingsPath);
                return new ConfigurationBuilder()
                    .SetBasePath(Path.GetDirectoryName(fullSettingsPath)!)
                    .AddJsonFile(Path.GetFileName(fullSettingsPath), optional: false, reloadOnChange: false)
                    .Build()
                    .Get<AppSettings>()
                    ?? throw new InvalidDataException($"Settings file '{fullSettingsPath}' is empty.");
            })
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
