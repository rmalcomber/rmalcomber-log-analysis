namespace LivenerTechTest.types;

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogLevel EventType { get; set; }
    public string UserId { get; set; }
    public string? StreamId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? OptionalData { get; set; }
}