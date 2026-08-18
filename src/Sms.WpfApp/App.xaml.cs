using System.IO;
using System.Windows;
using Autofac;
using Sms.WpfApp.Composition;

namespace Sms.WpfApp;

public partial class App
{
    private IContainer? _container;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new WpfAppModule(
                Path.Combine(AppContext.BaseDirectory, "appsettings.json")));
            _container = builder.Build();
            _container.Resolve<MainWindow>().Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Ошибка запуска",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _container?.Dispose();
        base.OnExit(e);
    }
}
