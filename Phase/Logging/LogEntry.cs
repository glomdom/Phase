namespace Phase.Logging;

public readonly struct LogEntry {
    public int ThreadId { get; }
    public LogSeverity Severity { get; }
    public string Message { get; }

    public LogEntry(int threadId, LogSeverity severity, string message) {
        ThreadId = threadId;
        Severity = severity;
        Message = message;
    }
}