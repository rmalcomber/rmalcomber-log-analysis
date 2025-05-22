using LivenerTechTest.interfaces;
using LivenerTechTest.parsers;
using LivenerTechTest.types;
using LogLevel = LivenerTechTest.types.LogLevel;

namespace LivenerTechTest.providers;

public class FileDataStoreProvider : IDataStore
{
    private readonly List<LogEntry> _allLogs;

    public FileDataStoreProvider(IConfiguration configuration)
    {
        var path = configuration.GetSection("FileDataStore").Value;

        if (path is null || !File.Exists(path))
            throw new FileNotFoundException("File not found for File Datastore", path);


        Console.WriteLine($"Loading file from {path}");

        var fileLoader = new FileParser(path);
        _allLogs = fileLoader.ReadLogs();
    }


    public List<LogEntry> GetAllLogs()
    {
        return _allLogs;
    }

    public LogAggregateDto GetLogAggregate()
    {
        var uniqueUsers = _allLogs.Select(x => x.UserId).Distinct().Count();
        var userActivity = _allLogs.Select(log => new UserActivity
        {
            Event = log.EventType.ToString("G"),
            UserId = log.UserId,
            Timestamp = log.Timestamp.ToLongDateString()
        });

        var logErrors = new LogErrors
        {
            Error = _allLogs.Count(l => l.EventType == LogLevel.ERROR),
            Critical = _allLogs.Count(l => l.EventType == LogLevel.CRITICAL),
            Warning = _allLogs.Count(l => l.EventType == LogLevel.WARNING)
        };

        return new LogAggregateDto
        {
            LogErrors = logErrors,
            UserActivity = userActivity.ToArray(),
            UniqueUsers = uniqueUsers
        };
    }
}