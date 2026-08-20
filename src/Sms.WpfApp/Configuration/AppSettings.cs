namespace Sms.WpfApp.Configuration;

public sealed record AppSettings
{
    public IReadOnlyList<string> EnvironmentVariables { get; init; } = [];

    public IReadOnlyDictionary<string, string> Defaults { get; init; }
        = new Dictionary<string, string>();

    public IReadOnlyDictionary<string, string> Comments { get; init; }
        = new Dictionary<string, string>();
}
