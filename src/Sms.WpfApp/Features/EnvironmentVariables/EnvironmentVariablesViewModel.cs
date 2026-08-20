using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Sms.WpfApp.Configuration;

namespace Sms.WpfApp.Features.EnvironmentVariables;

public sealed class EnvironmentVariablesViewModel : IDisposable
{
    private readonly CompositeDisposable _subscriptions = new();
    private readonly Dictionary<EnvironmentVariableItem, string> _savedValues = [];
    private readonly HashSet<EnvironmentVariableItem> _restoring = [];
    private readonly IEnvironmentVariableStore _store;
    private readonly ILogger<EnvironmentVariablesViewModel> _logger;

    public EnvironmentVariablesViewModel(
        AppSettings settings,
        IEnvironmentVariableStore store,
        ILogger<EnvironmentVariablesViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _store = store;
        _logger = logger;
        Variables = new ObservableCollection<EnvironmentVariableItem>(
            settings.EnvironmentVariables
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(name => CreateItem(name, settings)));

        foreach (var item in Variables)
        {
            _savedValues[item] = item.Value;
            _subscriptions.Add(
                item.WhenAnyValue(value => value.Value)
                    .Skip(1)
                    .DistinctUntilChanged()
                    .Subscribe(value => Save(item, value)));
        }
    }

    public ObservableCollection<EnvironmentVariableItem> Variables { get; }

    public Interaction<string, Unit> ShowError { get; } = new();

    public void Dispose() => _subscriptions.Dispose();

    private void Save(EnvironmentVariableItem item, string value)
    {
        if (_restoring.Remove(item))
        {
            return;
        }

        var previousValue = _savedValues[item];
        try
        {
            _store.Set(item.Name, value);
            if (_store.Get(item.Name) != value)
            {
                throw new InvalidOperationException("The environment variable was not saved.");
            }

            _logger.LogInformation(
                "Changed {VariableName}: {PreviousValue} -> {NewValue}",
                item.Name,
                Display(item, previousValue),
                Display(item, value));
            _savedValues[item] = value;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to change {VariableName}", item.Name);
            _restoring.Add(item);
            item.Value = previousValue;
            ShowError.Handle(exception.Message).Subscribe();
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
            _logger.LogInformation(
                "Initialized {VariableName}: {Value}",
                name,
                Display(isSensitive, value));
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
