using Sms.Shared.Logging;

namespace Sms.ConsoleApp.Output;

public sealed class ConsoleOutput(DailyFileLogger logger) : IConsoleOutput
{
    public void Log(string message) => logger.Write(message);

    public void WriteLine(string message)
    {
        Console.WriteLine(message);
        logger.Write(message);
    }

    public string? ReadLine(string prompt)
    {
        Console.WriteLine(prompt);
        logger.Write(prompt);
        var value = Console.ReadLine();
        logger.Write($"INPUT {value}");
        return value;
    }
}