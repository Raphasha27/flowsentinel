using FlowSentinel.Core.RateLimiting;
using Xunit;

namespace FlowSentinel.UnitTests;

public class LocalTokenBucketTests
{
    [Fact]
    public async Task Refill_ShouldWorkOverTime()
    {
        // Arrange: 1 token per second, capacity 1
        var limiter = new LocalTokenBucketRateLimiter(1, 1);
        string key = "test-refill";

        // Act & Assert
        Assert.True(await limiter.IsAllowedAsync(key)); // Consume 1
        Assert.False(await limiter.IsAllowedAsync(key)); // Empty
        
        await Task.Delay(1100); // Wait for refill
        
        Assert.True(await limiter.IsAllowedAsync(key)); // Should be refilled
    }

    [Fact]
    public async Task Burst_ShouldBeLimitedByCapacity()
    {
        // Arrange
        var limiter = new LocalTokenBucketRateLimiter(10, 5);
        string key = "test-burst";

        // Act
        for(int i=0; i<5; i++) 
            Assert.True(await limiter.IsAllowedAsync(key));

        // Assert
        Assert.False(await limiter.IsAllowedAsync(key));
    }
}
