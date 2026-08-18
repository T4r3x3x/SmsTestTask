namespace Sms.Shared.Logging;

public sealed class DailyFileLogger : IAppLogger
{
    private readonly string _directory;
    private readonly string _filePrefix;
    private readonly object _lock = new();
    private readonly TimeProvider _timeProvider;

    public DailyFileLogger(
        string directory,
        string filePrefix,
        TimeProvider? timeProvider = null)
    {
        Directory.CreateDirectory(directory);
        _directory = directory;
        _filePrefix = filePrefix;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public void Write(string message)
    {
        var now = _timeProvider.GetLocalNow();
        var path = Path.Combine(_directory, $"{_filePrefix}-{now:yyyyMMdd}.log");
        var line = $"{now:yyyy-MM-dd HH:mm:ss.fff zzz} {message}{Environment.NewLine}";

        lock (_lock)
        {
            File.AppendAllText(path, line, System.Text.Encoding.UTF8);
        }
    }
}