using System.Globalization;
using System.Text.RegularExpressions;
using LivenerTechTest.types;
using LogLevel = LivenerTechTest.types.LogLevel;

namespace LivenerTechTest.parsers;

public partial class FileParser(string filePath)
{
    private static readonly Regex LogRegex = LogParseRegex();

    public List<LogEntry> ReadLogs()
    {
        var logs = new List<LogEntry>();
        using (var reader = new StreamReader(filePath))
        {
            while (reader.ReadLine() is { } line)
            {
                var parsedLine = Parse(line);
                logs.Add(parsedLine);
            }
        }

        return logs.OrderBy(l => l.Timestamp).ToList();
    }

    private static LogEntry Parse(string line)
    {
        try
        {
            var match = LogRegex.Match(line);
            if (!match.Success) throw new FormatException($"Invalid log format: {line}");

            var timestampStr = match.Groups[1].Value;
            var eventTypeStr = match.Groups[2].Value;
            var userId = match.Groups[3].Value;
            var streamId = match.Groups[4].Value;
            var message = match.Groups[5].Value.Trim();
            var optionalData = match.Groups[6].Success ? match.Groups[6].Value.Trim() : null;

            return new LogEntry
            {
                Timestamp = DateTime.ParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                EventType = Enum.TryParse<LogLevel>(eventTypeStr, true, out var level) ? level : LogLevel.UNKNOWN,
                UserId = (userId != "-" ? userId : null) ?? string.Empty,
                StreamId = streamId != "-" ? streamId : null,
                Message = message,
                OptionalData = optionalData
            };
        }
        catch
        {
            throw new FailedToParseLineException(line);
        }
    }

    [GeneratedRegex(@"\[(.*?)\]\s+(\S+)\s+(\S+)\s+(\S+)\s+(.*?)(\s+\[.*\])?$", RegexOptions.Compiled)]
    private static partial Regex LogParseRegex();
}