using Sms.ConsoleApp.Application;
using Sms.ConsoleApp.Database;
using Sms.ConsoleApp.Output;
using Sms.Shared.Sms;

namespace Sms.ConsoleApp.Tests;

public sealed class ConsoleApplicationTests
{
    [Fact]
    public async Task RunAsync_LogsSystemMessagesWithoutWritingThemToConsole()
    {
        var output = new TestOutput();
        var result = await new ConsoleApplication(
            new FailingClient(),
            new TestRepository(),
            output).RunAsync();

        Assert.Equal(1, result);
        Assert.Equal(
            ["Инициализация базы данных...", "Получение меню..."],
            output.Logged);
        Assert.Equal(["Меню недоступно"], output.Written);
    }

    private sealed class FailingClient : ISmsClient
    {
        public Task<IReadOnlyList<MenuItem>> GetMenuAsync(CancellationToken cancellationToken = default) =>
            throw new SmsApiException("GetMenu", "Меню недоступно");

        public Task SendOrderAsync(Order order, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class TestRepository : IMenuRepository
    {
        public Task InitializeAsync(CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task UpsertAsync(
            IReadOnlyCollection<MenuItem> menu,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class TestOutput : IConsoleOutput
    {
        public List<string> Logged { get; } = [];

        public List<string> Written { get; } = [];

        public void Log(string message) => Logged.Add(message);

        public void WriteLine(string message) => Written.Add(message);

        public string? ReadLine(string prompt) => throw new NotSupportedException();
    }
}