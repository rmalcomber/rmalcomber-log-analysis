using LivenerTechTest.types;

namespace LivenerTechTest.interfaces;

public interface IDataStore
{
    List<LogEntry> GetAllLogs();
    LogAggregateDto GetLogAggregate();
}