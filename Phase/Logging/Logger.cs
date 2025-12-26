using System.Threading.Channels;

namespace Phase.Logging;

public static class Logger {
    private static readonly Task LogTask;
    private static readonly Channel<LogEntry> LogChannel;

    static Logger() {
        LogChannel = Channel.CreateUnbounded<LogEntry>();
        LogTask = Task.Run(ProcessLogsAsync);
    }

    public static void Error(string message) {
        WriteToChannel(LogSeverity.Error, message);
    }

    public static void Warning(string message) {
        WriteToChannel(LogSeverity.Warning, message);
    }

    public static void Success(string message) {
        WriteToChannel(LogSeverity.Info, message);
    }

    public static void Debug(string message) {
        WriteToChannel(LogSeverity.Debug, message);
    }

    public static void Trace(string message) {
        WriteToChannel(LogSeverity.Trace, message);
    }

    private static void WriteToChannel(LogSeverity severity, string message) {
        var color = AnsiColorFromSeverity(severity);
        var formattedMessage = $"{color}{message}\e[0m";
        var log = new LogEntry(Environment.CurrentManagedThreadId, severity, formattedMessage);

        LogChannel.Writer.TryWrite(log);
    }

    public static async Task ShutdownAsync() {
        Trace("emptying logger");

        LogChannel.Writer.Complete();
        await LogTask;
    }

    private static async Task ProcessLogsAsync() {
        await foreach (var entry in LogChannel.Reader.ReadAllAsync()) {
            var formatted = $"{entry.ThreadId} .. {entry.Message}";

            Console.WriteLine(formatted);
        }
    }

    private static string AnsiColorFromSeverity(LogSeverity severity) {
        return severity switch {
            LogSeverity.Error => "\e[31m",
            LogSeverity.Info => "\e[32m",
            LogSeverity.Debug => "\e[96m",
            LogSeverity.Trace => "\e[90m",

            _ => "\e[37m",
        };
    }
}