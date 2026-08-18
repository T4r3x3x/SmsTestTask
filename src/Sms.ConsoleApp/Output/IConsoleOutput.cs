namespace Sms.ConsoleApp.Output;

public interface IConsoleOutput
{
    void Log(string message);

    void WriteLine(string message);

    string? ReadLine(string prompt);
}