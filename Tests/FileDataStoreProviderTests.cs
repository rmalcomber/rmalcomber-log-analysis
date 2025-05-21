using System.Globalization;
using LivenerTechTest.providers;
using Microsoft.Extensions.Configuration;
using Moq;
using LogLevel = LivenerTechTest.types.LogLevel;

namespace Tests;

public class FileDataStoreProviderTests : IDisposable
{
    private readonly Mock<IConfigurationSection> _mockConfigSection;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly string _testFilePath;

    public FileDataStoreProviderTests()
    {
       
        _testFilePath = Path.GetTempFileName();

        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfigSection = new Mock<IConfigurationSection>();

        _mockConfigSection.Setup(x => x.Value).Returns(_testFilePath);
        _mockConfiguration.Setup(x => x.GetSection("FileDataStore")).Returns(_mockConfigSection.Object);
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
    }

    [Fact]
    public void Constructor_FileNotFound_ThrowsFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns(nonExistentPath);
        mockConfig.Setup(x => x.GetSection("FileDataStore")).Returns(mockSection.Object);

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => new FileDataStoreProvider(mockConfig.Object));
    }

    [Fact]
    public void GetAllLogs_ReturnsAllLogsFromFile()
    {
        // Arrange
        var logLines = new[]
        {
            "[2023-05-21 15:30:45] INFO user123 stream456 User logged in",
            "[2023-05-21 14:20:10] ERROR user123 stream456 Connection failed"
        };

        File.WriteAllLines(_testFilePath, logLines);
        var provider = new FileDataStoreProvider(_mockConfiguration.Object);

        // Act
        var result = provider.GetAllLogs();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(LogLevel.INFO, result[1].EventType);
        Assert.Equal(LogLevel.ERROR, result[0].EventType);
    }

    [Fact]
    public void GetLogAggregate_ReturnsCorrectAggregation()
    {
        // Arrange
        var logLines = new[]
        {
            "[2023-05-21 15:30:45] INFO user1 stream1 Info message",
            "[2023-05-21 14:20:10] ERROR user1 stream1 Error message",
            "[2023-05-21 16:45:30] WARNING user2 stream2 Warning message",
            "[2023-05-21 17:10:20] CRITICAL user3 stream3 Critical message",
            "[2023-05-21 18:05:15] ERROR user2 stream2 Another error"
        };

        File.WriteAllLines(_testFilePath, logLines);
        var provider = new FileDataStoreProvider(_mockConfiguration.Object);

        // Act
        var result = provider.GetLogAggregate();

        // Assert
        Assert.Equal(3, result.UniqueUsers);
        Assert.Equal(5, result.UserActivity.Length);

        // Check error counts
        Assert.Equal(2, result.LogErrors.Error);
        Assert.Equal(1, result.LogErrors.Critical);
        Assert.Equal(1, result.LogErrors.Warning);
    }

    [Fact]
    public void GetLogAggregate_EmptyLogs_ReturnsEmptyAggregation()
    {
        // Arrange
        File.WriteAllText(_testFilePath, string.Empty);
        var provider = new FileDataStoreProvider(_mockConfiguration.Object);

        // Act
        var result = provider.GetLogAggregate();

        // Assert
        Assert.Equal(0, result.UniqueUsers);
        Assert.Empty(result.UserActivity);
        Assert.Equal(0, result.LogErrors.Error);
        Assert.Equal(0, result.LogErrors.Critical);
        Assert.Equal(0, result.LogErrors.Warning);
    }

    [Fact]
    public void GetLogAggregate_UserActivityContainsCorrectData()
    {
        // Arrange
        var timestamp = "2023-05-21 15:30:45";
        var logLine = $"[{timestamp}] INFO user123 stream456 User logged in";

        File.WriteAllText(_testFilePath, logLine);
        var provider = new FileDataStoreProvider(_mockConfiguration.Object);

        // Act
        var result = provider.GetLogAggregate();

        // Assert
        Assert.Single(result.UserActivity);
        var activity = result.UserActivity[0];
        Assert.Equal("user123", activity.UserId);
        Assert.Equal("INFO", activity.Event);

        var expectedDate = DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture).ToLongDateString();
        Assert.Equal(expectedDate, activity.Timestamp);
    }
}