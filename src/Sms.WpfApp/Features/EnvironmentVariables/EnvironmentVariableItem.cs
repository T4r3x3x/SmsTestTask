using ReactiveUI;

namespace Sms.WpfApp.Features.EnvironmentVariables;

public sealed class EnvironmentVariableItem(
    string name,
    string value,
    string comment,
    bool isSensitive) : ReactiveObject
{
    private string _value = value;

    public string Name { get; } = name;

    public string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public string Comment { get; } = comment;

    public bool IsSensitive { get; } = isSensitive;
}
