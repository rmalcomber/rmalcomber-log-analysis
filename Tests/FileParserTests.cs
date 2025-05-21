using System.Globalization;
using System.Text;
using LivenerTechTest.parsers;
using LivenerTechTest.types;

namespace Tests;

public class FileParserTests : IDisposable
{
    private readonly string _testFilePath = Path.GetTempFileName();

    public void Dispose()
    {
        
        if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
    }

    [Fact]
    public void ReadLogs_ValidFormat_ReturnsOrderedLogEntries()
    {
        // Arrange
        var logLines = new[]
        {
            "[2023-05-21 15:30:45] INFO user123 stream456 User logged in",
            "[2023-05-21 14:20:10] ERROR user123 stream456 Connection failed",
            "[2023-05-21 15:00:00] DEBUG - stream456 Debug message"
        };

        File.WriteAllLines(_testFilePath, logLines);
        var fileHandler = new FileParser(_testFilePath);

        // Act
        var result = fileHandler.ReadLogs();

        // Assert
        Assert.Equal(3, result.Count);


        Assert.Equal(DateTime.ParseExact("2023-05-21 14:20:10", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            result[0].Timestamp);
        Assert.Equal(DateTime.ParseExact("2023-05-21 15:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            result[1].Timestamp);
        Assert.Equal(DateTime.ParseExact("2023-05-21 15:30:45", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            result[2].Timestamp);

        Assert.Equal(LogLevel.ERROR, result[0].EventType);
        Assert.Equal("user123", result[0].UserId);
        Assert.Equal("stream456", result[0].StreamId);
        Assert.Equal("Connection failed", result[0].Message);
    }

    [Fact]
    public void ReadLogs_WithOptionalData_ParsesCorrectly()
    {
        // Arrange
        var logLine = "[2023-05-21 15:30:45] INFO user123 stream456 User logged in [additional=data, more=info]";
        File.WriteAllText(_testFilePath, logLine);
        var fileHandler = new FileParser(_testFilePath);

        // Act
        var result = fileHandler.ReadLogs();

        // Assert
        Assert.Single(result);
        Assert.Equal(LogLevel.INFO, result[0].EventType);
        Assert.Equal("User logged in", result[0].Message);
        Assert.Equal("[additional=data, more=info]", result[0].OptionalData);
    }

    [Fact]
    public void ReadLogs_WithDashForUserId_SetsEmptyString()
    {
        // Arrange
        var logLine = "[2023-05-21 15:30:45] DEBUG - stream456 Debug message";
        File.WriteAllText(_testFilePath, logLine);
        var fileHandler = new FileParser(_testFilePath);

        // Act
        var result = fileHandler.ReadLogs();

        // Assert
        Assert.Single(result);
        Assert.Equal(LogLevel.DEBUG, result[0].EventType);
        Assert.Equal(string.Empty, result[0].UserId);
        Assert.Equal("stream456", result[0].StreamId);
    }

    [Fact]
    public void ReadLogs_WithDashForStreamId_SetsNull()
    {
        // Arrange
        var logLine = "[2023-05-21 15:30:45] INFO user123 - System message";
        File.WriteAllText(_testFilePath, logLine);
        var fileHandler = new FileParser(_testFilePath);

        // Act
        var result = fileHandler.ReadLogs();

        // Assert
        Assert.Single(result);
        Assert.Equal(LogLevel.INFO, result[0].EventType);
        Assert.Equal("user123", result[0].UserId);
        Assert.Null(result[0].StreamId);
    }

    [Fact]
    public void ReadLogs_WithUnknownLogLevel_SetsUnknown()
    {
        // Arrange
        var logLine = "[2023-05-21 15:30:45] CUSTOM user123 stream456 Custom log level";
        File.WriteAllText(_testFilePath, logLine);
        var fileHandler = new FileParser(_testFilePath);

        // Act
        var result = fileHandler.ReadLogs();

        // Assert
        Assert.Single(result);
        Assert.Equal(LogLevel.UNKNOWN, result[0].EventType);
    }

    [Fact]
    public void ReadLogs_InvalidFormat_ThrowsException()
    {
        // Arrange
        var logLine = "Invalid log format without proper structure";
        File.WriteAllText(_testFilePath, logLine);
        var fileHandler = new FileParser(_testFilePath);

        // Act & Assert
        var exception = Assert.Throws<FailedToParseLineException>(() => fileHandler.ReadLogs());
        Assert.Contains(logLine, exception.Message);
    }

    [Fact]
    public void ReadLogs_EmptyFile_ReturnsEmptyList()
    {
        // Arrange
        File.WriteAllText(_testFilePath, string.Empty);
        var fileHandler = new FileParser(_testFilePath);

        // Act
        var result = fileHandler.ReadLogs();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ReadLogs_FileNotFound_ThrowsFileNotFoundException()
    {
        // Arrange
        var nonExistentFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var fileHandler = new FileParser(nonExistentFilePath);

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => fileHandler.ReadLogs());
    }

    [Fact]
    public void ReadLogs_MultipleLines_ParsesAllLines()
    {
        // Arrange
        var sb = new StringBuilder();
        for (var i = 1; i <= 10; i++) sb.AppendLine($"[2023-05-{i:D2} 10:00:00] INFO user{i} stream{i} Message {i}");
        File.WriteAllText(_testFilePath, sb.ToString());
        var fileHandler = new FileParser(_testFilePath);

        // Act
        var result = fileHandler.ReadLogs();

        // Assert
        Assert.Equal(10, result.Count);

        for (var i = 0; i < 9; i++) Assert.True(result[i].Timestamp < result[i + 1].Timestamp);
    }
}