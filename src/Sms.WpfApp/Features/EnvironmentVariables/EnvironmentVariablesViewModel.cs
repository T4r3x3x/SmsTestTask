using System.Collections.ObjectModel;

namespace Sms.WpfApp.Features.EnvironmentVariables;

public sealed class EnvironmentVariablesViewModel
{
    private readonly IEnvironmentVariableStore _store;
    private readonly Action<string> _log;

    public EnvironmentVariablesViewModel(
        AppSettings settings,
        IEnvironmentVariableStore store,
        Action<string> log)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _store = store;
        _log = log;
        Variables = new ObservableCollection<EnvironmentVariableItem>(
            settings.EnvironmentVariables
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(name => CreateItem(name, settings)));
    }

    public ObservableCollection<EnvironmentVariableItem> Variables { get; }

    public bool TrySave(EnvironmentVariableItem item, string value, out string? error)
    {
        error = null;
        if (item.Value == value)
        {
            return true;
        }

        try
        {
            _store.Set(item.Name, value);
            if (_store.Get(item.Name) != value)
            {
                throw new InvalidOperationException("The environment variable was not saved.");
            }

            _log($"Changed {item.Name}: {Display(item, item.Value)} -> {Display(item, value)}");
            item.UpdateValue(value);
            return true;
        }
        catch (Exception exception)
        {
            error = exception.Message;
            _log($"Failed to change {item.Name}: {exception.Message}");
            return false;
        }
    }

    private EnvironmentVariableItem CreateItem(string name, AppSettings settings)
    {
        var value = _store.Get(name);
        var isSensitive = settings.SensitiveVariables.Contains(name);
        if (value is null)
        {
            value = settings.Defaults.GetValueOrDefault(name, string.Empty);
            _store.Set(name, value);
            _log($"Initialized {name}: {Display(isSensitive, value)}");
        }

        return new EnvironmentVariableItem(
            name,
            value,
            settings.Comments.GetValueOrDefault(name, string.Empty),
            isSensitive);
    }

    private static string Display(EnvironmentVariableItem item, string value) =>
        Display(item.IsSensitive, value);

    private static string Display(bool isSensitive, string value) =>
        isSensitive ? "***" : value;
}
