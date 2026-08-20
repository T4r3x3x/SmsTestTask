using System.Reactive;
using Microsoft.Extensions.Logging;
using Sms.WpfApp.Bootstrap;
using Sms.WpfApp.Configuration;
using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp.Tests;

public sealed class EnvironmentVariablesViewModelTests
{
    [Fact]
    public void Constructor_UsesExistingValueWithoutOverwritingIt()
    {
        var store = new TestStore { Values = { ["SMS_URL"] = "existing" } };

        using var viewModel = CreateViewModel(store, out _);

        Assert.Equal("existing", Assert.Single(viewModel.Variables).Value);
        Assert.Empty(store.Writes);
    }

    [Fact]
    public void Constructor_InitializesMissingValueFromSettings()
    {
        var store = new TestStore();

        using var viewModel = CreateViewModel(store, out var log);

        Assert.Equal("default", Assert.Single(viewModel.Variables).Value);
        Assert.Equal(("SMS_URL", "default"), Assert.Single(store.Writes));
        Assert.Contains("Initialized SMS_URL", Assert.Single(log));
    }

    [Fact]
    public void ValueChange_PersistsValue()
    {
        var store = new TestStore { Values = { ["SMS_URL"] = "old" } };
        using var viewModel = CreateViewModel(store, out var log);
        var item = Assert.Single(viewModel.Variables);

        item.Value = "new";

        Assert.Equal("new", item.Value);
        Assert.Equal(("SMS_URL", "new"), Assert.Single(store.Writes));
        Assert.Contains("old -> new", Assert.Single(log));
    }

    [Fact]
    public void ValueChange_RestoresValueAndShowsErrorWhenStoreFails()
    {
        var store = new TestStore
        {
            Values = { ["SMS_URL"] = "old" },
            Error = new InvalidOperationException("Access denied")
        };
        using var viewModel = CreateViewModel(store, out var log);
        var errors = new List<string>();
        using var handler = viewModel.ShowError.RegisterHandler(context =>
        {
            errors.Add(context.Input);
            context.SetOutput(Unit.Default);
        });
        var item = Assert.Single(viewModel.Variables);

        item.Value = "new";

        Assert.Equal("old", item.Value);
        Assert.Contains("Access denied", Assert.Single(errors));
        Assert.Contains("Failed to change SMS_URL", Assert.Single(log));
    }

    private static EnvironmentVariablesViewModel CreateViewModel(
        TestStore store,
        out List<string> log)
    {
        ReactiveUiBootstrapper.Initialize();
        var logger = new TestLogger();
        var settings = new AppSettings
        {
            EnvironmentVariables = ["SMS_URL"],
            Defaults = new Dictionary<string, string> { ["SMS_URL"] = "default" },
            Comments = new Dictionary<string, string> { ["SMS_URL"] = "Endpoint" }
        };

        var viewModel = new EnvironmentVariablesViewModel(settings, store, logger);
        log = logger.Messages;
        return viewModel;
    }

    private sealed class TestLogger : ILogger<EnvironmentVariablesViewModel>
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            Messages.Add(formatter(state, exception));
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
