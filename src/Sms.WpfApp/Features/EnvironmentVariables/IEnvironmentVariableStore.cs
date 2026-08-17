namespace Sms.WpfApp.Features.EnvironmentVariables;

public interface IEnvironmentVariableStore
{
    string? Get(string name);

    void Set(string name, string value);
}
