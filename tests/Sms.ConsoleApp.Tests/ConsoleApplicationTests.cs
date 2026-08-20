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

    [Fact]
    public async Task RunAsync_StopsWhenInputStreamEnds()
    {
        var client = new SuccessfulClient();
        var output = new TestOutput { Input = null };

        var result = await new ConsoleApplication(
            client,
            new TestRepository(),
            output).RunAsync();

        Assert.Equal(0, result);
        Assert.Equal(1, output.ReadCount);
        Assert.Contains("Входной поток завершён.", output.Logged);
        Assert.DoesNotContain(output.Written, message => message.Contains("Добавьте хотя бы одну позицию"));
        Assert.False(client.OrderSent);
    }

    private sealed class FailingClient : ISmsClient
    {
        public Task<IReadOnlyList<MenuItem>> GetMenuAsync(CancellationToken cancellationToken = default) =>
            throw new SmsApiException("GetMenu", "Меню недоступно");

        public Task SendOrderAsync(Order order, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class SuccessfulClient : ISmsClient
    {
        public bool OrderSent { get; private set; }

        public Task<IReadOnlyList<MenuItem>> GetMenuAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MenuItem>>(
            [
                new MenuItem("1", "A1", "Блюдо", 100, false, "Меню", [])
            ]);

        public Task SendOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            OrderSent = true;
            return Task.CompletedTask;
        }
    }

    private sealed class TestRepository : IMenuRepository
    {
        public Task InitializeAsync(CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task UpsertAsync(
            IReadOnlyCollection<MenuItem> menu,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class TestOutput : IConsoleOutput
    {
        public List<string> Logged { get; } = [];

        public List<string> Written { get; } = [];

        public string? Input { get; init; }

        public int ReadCount { get; private set; }

        public void Log(string message) => Logged.Add(message);

        public void WriteLine(string message) => Written.Add(message);

        public string? ReadLine(string prompt)
        {
            ReadCount++;
            return Input;
        }
    }
}
