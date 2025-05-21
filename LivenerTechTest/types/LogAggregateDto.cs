using System.Text.Json.Serialization;

namespace LivenerTechTest.types;

public class LogAggregateDto
{
    [JsonPropertyName("uniqueUsers")] public int UniqueUsers { get; set; }
    [JsonPropertyName("userActivity")] public UserActivity[] UserActivity { get; set; }
    [JsonPropertyName("errors")] public LogErrors LogErrors { get; set; }
}

public class UserActivity
{
    [JsonPropertyName("userId")] public string UserId { get; set; }
    [JsonPropertyName("event")] public string Event { get; set; }
    [JsonPropertyName("timestamp")] public string Timestamp { get; set; }
}

public class LogErrors
{
    [JsonPropertyName("ERROR")] public int Error { get; set; }
    [JsonPropertyName("CRITICAL")] public int Critical { get; set; }
    [JsonPropertyName("WARNING")] public int Warning { get; set; }
}