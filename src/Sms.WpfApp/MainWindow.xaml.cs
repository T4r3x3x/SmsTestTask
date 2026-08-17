using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp;

public partial class MainWindow
{
    public MainWindow() => InitializeComponent();

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

    private void VariablesGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction != DataGridEditAction.Commit ||
            e.Column.Header?.ToString() != "Значение" ||
            e.Row.Item is not EnvironmentVariableItem item ||
            e.EditingElement is not TextBox editor ||
            DataContext is not EnvironmentVariablesViewModel viewModel)
        {
            return;
        }

        if (viewModel.TrySave(item, editor.Text, out var error))
        {
            return;
        }

        e.Cancel = true;
        MessageBox.Show(error, "Не удалось сохранить значение", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
