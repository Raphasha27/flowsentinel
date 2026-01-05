using FlowSentinel.Core.RateLimiting;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace FlowSentinel.UnitTests;

public class RedisTokenBucketTests
{
    [Fact]
    public async Task IsAllowedAsync_ShouldCallRedisWithCorrectArguments()
    {
        // Arrange
        var mockRedis = new Mock<IConnectionMultiplexer>();
        var mockDb = new Mock<IDatabase>();
        
        mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);
        
        // Mocking ScriptEvaluateAsync to return [1, 10] (allowed=1, remaining=10)
        mockDb.Setup(db => db.ScriptEvaluateAsync(
            It.IsAny<string>(), 
            It.IsAny<RedisKey[]>(), 
            It.IsAny<RedisValue[]>(), 
            It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisResult.Create(new RedisResult[] { 
                RedisResult.Create(1), 
                RedisResult.Create(10) 
            }));

        var limiter = new RedisTokenBucketRateLimiter(mockRedis.Object, 10, 100);

        // Act
        var allowed = await limiter.IsAllowedAsync("test-client");

        // Assert
        Assert.True(allowed);
        mockDb.Verify(db => db.ScriptEvaluateAsync(
            It.IsAny<string>(),
            It.Is<RedisKey[]>(k => k.Length == 2),
            It.Is<RedisValue[]>(v => v.Length == 4),
            It.IsAny<CommandFlags>()), Times.Once);
    }
}
