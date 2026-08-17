namespace Sms.WpfApp.Features.EnvironmentVariables;

public sealed class EnvironmentVariableStore : IEnvironmentVariableStore
{
    public string? Get(string name) =>
        Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);

    public void Set(string name, string value) =>
        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
}
