using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sms.WpfApp.Features.EnvironmentVariables;

public sealed class EnvironmentVariableItem(
    string name,
    string value,
    string comment,
    bool isSensitive) : INotifyPropertyChanged
{
    private string _value = value;

    public string Name { get; } = name;

    public string Value
    {
        get => _value;
        private set
        {
            if (_value == value)
            {
                return;
            }

            _value = value;
            OnPropertyChanged();
        }
    }

    public string Comment { get; } = comment;

    public bool IsSensitive { get; } = isSensitive;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void UpdateValue(string value) => Value = value;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
