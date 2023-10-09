using Microsoft.Extensions.Logging;

namespace Test.Portal.Bll.Unit;

public class TestLogger<T> : ILogger<T>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, String> formatter)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
            case LogLevel.Information:
            case LogLevel.Warning:
                LogInformation(logLevel, state.ToString());
                break;
            case LogLevel.Error:
            case LogLevel.Critical:
                LogException(logLevel, exception, state.ToString());
                break;
            case LogLevel.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }

    public void LogException(LogLevel logLevel, Exception? e, String mess)
    {
        var message = e != null ? e.Message : "exception";
        message += String.IsNullOrWhiteSpace(mess) ? "" : mess;
        Console.WriteLine($"{logLevel}|{message}");
    }

    public void LogInformation(LogLevel logLevel, String mess)
    {
        var message = String.IsNullOrWhiteSpace(mess) ? "" : mess;
        Console.WriteLine($"{logLevel}|{message}");
    }


    public Boolean IsEnabled(LogLevel logLevel)
    {
        return true;
    }
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}