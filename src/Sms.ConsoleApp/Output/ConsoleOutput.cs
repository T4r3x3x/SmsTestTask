using Microsoft.Extensions.Logging;

namespace Sms.ConsoleApp.Output;

public sealed class ConsoleOutput(ILogger<ConsoleOutput> logger) : IConsoleOutput
{
    public void Log(string message) => logger.LogInformation("{Message}", message);

    public void WriteLine(string message)
    {
        Console.WriteLine(message);
        logger.LogInformation("{Message}", message);
    }

    public string? ReadLine(string prompt)
    {
        Console.WriteLine(prompt);
        logger.LogInformation("{Prompt}", prompt);
        var value = Console.ReadLine();
        logger.LogInformation("INPUT {Value}", value);
        return value;
    }
}
