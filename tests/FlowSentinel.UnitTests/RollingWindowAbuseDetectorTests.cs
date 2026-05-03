using FlowSentinel.Core.AbuseDetection;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace FlowSentinel.UnitTests;

public class RollingWindowAbuseDetectorTests
{
    [Fact]
    public async Task IsAbusiveAsync_ShouldReturnTrue_WhenThresholdExceeded()
    {
        // Arrange
        var mockRedis = new Mock<IConnectionMultiplexer>();
        var mockDb = new Mock<IDatabase>();
        var mockTransaction = new Mock<ITransaction>();

        mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);
        mockDb.Setup(db => db.CreateTransaction(It.IsAny<object>())).Returns(mockTransaction.Object);
        
        // Mocking SortedSetLengthAsync via transaction to return 1000 (exceeding default 500)
        mockTransaction.Setup(t => t.SortedSetLengthAsync(It.IsAny<RedisKey>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(1000);
        
        mockTransaction.Setup(t => t.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);

        var detector = new RollingWindowAbuseDetector(mockRedis.Object, 60, 500);

        // Act
        var isAbusive = await detector.IsAbusiveAsync("toxic-client");

        // Assert
        Assert.True(isAbusive);
        mockTransaction.Verify(t => t.ExecuteAsync(It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task IsAbusiveAsync_ShouldReturnFalse_WhenUnderThreshold()
    {
        // Arrange
        var mockRedis = new Mock<IConnectionMultiplexer>();
        var mockDb = new Mock<IDatabase>();
        var mockTransaction = new Mock<ITransaction>();

        mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);
        mockDb.Setup(db => db.CreateTransaction(It.IsAny<object>())).Returns(mockTransaction.Object);
        
        mockTransaction.Setup(t => t.SortedSetLengthAsync(It.IsAny<RedisKey>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(100);
        
        mockTransaction.Setup(t => t.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);

        var detector = new RollingWindowAbuseDetector(mockRedis.Object, 60, 500);

        // Act
        var isAbusive = await detector.IsAbusiveAsync("good-client");

        // Assert
        Assert.False(isAbusive);
    }
}
