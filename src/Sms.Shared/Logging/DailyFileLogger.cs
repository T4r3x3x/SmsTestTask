namespace Sms.Shared.Logging;

public sealed class DailyFileLogger(
    string directory,
    string filePrefix,
    TimeProvider? timeProvider = null)
{
    private readonly object _lock = new();
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    public void Write(string message)
    {
        var now = _timeProvider.GetLocalNow();
        var path = Path.Combine(directory, $"{filePrefix}-{now:yyyyMMdd}.log");
        var line = $"{now:yyyy-MM-dd HH:mm:ss.fff zzz} {message}{Environment.NewLine}";

        lock (_lock)
        {
            Directory.CreateDirectory(directory);
            File.AppendAllText(path, line, System.Text.Encoding.UTF8);
        }
    }
}
