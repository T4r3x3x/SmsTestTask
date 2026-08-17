using System.IO;
using System.Windows;
using Sms.Shared.Configuration;
using Sms.Shared.Logging;
using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var settings = JsonSettingsLoader.Load<AppSettings>(
                Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
            var logger = new DailyFileLogger(AppContext.BaseDirectory, "test-sms-wpf-app");
            var viewModel = new EnvironmentVariablesViewModel(
                settings,
                new EnvironmentVariableStore(),
                logger.Write);

            new MainWindow { DataContext = viewModel }.Show();
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
}
