namespace Sms.ConsoleApp.Output;

public interface IConsoleOutput
{
    void WriteLine(string message);

    string? ReadLine(string prompt);
}