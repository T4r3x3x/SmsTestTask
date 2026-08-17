using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp.Tests;

public sealed class EnvironmentVariablesViewModelTests
{
    [Fact]
    public void Constructor_UsesExistingValueWithoutOverwritingIt()
    {
        var store = new TestStore { Values = { ["SMS_URL"] = "existing" } };

        var viewModel = CreateViewModel(store, out _);

        Assert.Equal("existing", Assert.Single(viewModel.Variables).Value);
        Assert.Empty(store.Writes);
    }

    [Fact]
    public void Constructor_InitializesMissingValueFromSettings()
    {
        var store = new TestStore();

        var viewModel = CreateViewModel(store, out var log);

        Assert.Equal("default", Assert.Single(viewModel.Variables).Value);
        Assert.Equal(("SMS_URL", "default"), Assert.Single(store.Writes));
        Assert.Contains("Initialized SMS_URL", Assert.Single(log));
    }

    [Fact]
    public void TrySave_PersistsValueAndUpdatesItem()
    {
        var store = new TestStore { Values = { ["SMS_URL"] = "old" } };
        var viewModel = CreateViewModel(store, out var log);
        var item = Assert.Single(viewModel.Variables);

        var success = viewModel.TrySave(item, "new", out var error);

        Assert.True(success);
        Assert.Null(error);
        Assert.Equal("new", item.Value);
        Assert.Equal(("SMS_URL", "new"), Assert.Single(store.Writes));
        Assert.Contains("old -> new", Assert.Single(log));
    }

    [Fact]
    public void TrySave_DoesNotUpdateItemWhenStoreFails()
    {
        var store = new TestStore
        {
            Values = { ["SMS_URL"] = "old" },
            Error = new InvalidOperationException("Access denied")
        };
        var viewModel = CreateViewModel(store, out var log);
        var item = Assert.Single(viewModel.Variables);

        var success = viewModel.TrySave(item, "new", out var error);

        Assert.False(success);
        Assert.Equal("Access denied", error);
        Assert.Equal("old", item.Value);
        Assert.Contains("Failed to change SMS_URL", Assert.Single(log));
    }

    private static EnvironmentVariablesViewModel CreateViewModel(
        TestStore store,
        out List<string> log)
    {
        var messages = new List<string>();
        var settings = new AppSettings
        {
            EnvironmentVariables = ["SMS_URL"],
            Defaults = new Dictionary<string, string> { ["SMS_URL"] = "default" },
            Comments = new Dictionary<string, string> { ["SMS_URL"] = "Endpoint" }
        };
        var viewModel = new EnvironmentVariablesViewModel(settings, store, messages.Add);
        log = messages;
        return viewModel;
    }

    private sealed class TestStore : IEnvironmentVariableStore
    {
        public Dictionary<string, string> Values { get; } = [];

        public List<(string Name, string Value)> Writes { get; } = [];

        public Exception? Error { get; init; }

        public string? Get(string name) => Values.GetValueOrDefault(name);

        public void Set(string name, string value)
        {
            if (Error is not null)
            {
                throw Error;
            }

            Writes.Add((name, value));
            Values[name] = value;
        }
    }
}
