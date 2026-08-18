using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp;

public partial class MainWindow : IDisposable
{
    private readonly CompositeDisposable _subscriptions = new();

    public MainWindow(EnvironmentVariablesViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
        
        _subscriptions.Add(
            viewModel.ShowError.RegisterHandler(context =>
            {
                MessageBox.Show(
                    context.Input,
                    "Не удалось сохранить значение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                context.SetOutput(Unit.Default);
            }));
    }

    public void Dispose() => _subscriptions.Dispose();

    private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void MinimizeButton_OnClick(object sender, RoutedEventArgs e) =>
        WindowState = WindowState.Minimized;

    private void CloseButton_OnClick(object sender, RoutedEventArgs e) => Close();
}
