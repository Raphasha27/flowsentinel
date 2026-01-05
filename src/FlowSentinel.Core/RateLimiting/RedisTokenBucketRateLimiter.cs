using StackExchange.Redis;

namespace FlowSentinel.Core.RateLimiting;

public class RedisTokenBucketRateLimiter : IRateLimiter
{
    private readonly IDatabase _db;
    
    // In a real system, these would come from a Policy Lookup based on the key
    // For this core implementation, we demonstrate the algorithm mechanism.
    private readonly double _refillRatePerSecond; 
    private readonly long _maxCapacity;

    // Lua script for atomic Token Bucket
    private const string RateLimitScript = @"
        local bucketKey = KEYS[1]
        local timestampKey = KEYS[2]
        local capacity = tonumber(ARGV[1])
        local rate = tonumber(ARGV[2])
        local now = tonumber(ARGV[3])
        local cost = tonumber(ARGV[4])

        local lastRefill = tonumber(redis.call('get', timestampKey) or 0)
        local tokens = tonumber(redis.call('get', bucketKey) or capacity)
        
        -- If first time, initialize logic handle
        if lastRefill == 0 then
            lastRefill = now
            tokens = capacity
        end

        local delta = math.max(0, now - lastRefill)
        local refilled = tokens + (delta * rate)
        
        if refilled > capacity then
            refilled = capacity
        end

        local allowed = 0
        local remaining = refilled
        
        if refilled >= cost then
            remaining = refilled - cost
            allowed = 1
            redis.call('set', bucketKey, remaining)
            redis.call('set', timestampKey, now)
            redis.call('expire', bucketKey, 3600)
            redis.call('expire', timestampKey, 3600)
        end

        return { allowed, remaining }
    ";

    public RedisTokenBucketRateLimiter(IConnectionMultiplexer redis, double refillRatePerSecond = 10, long maxCapacity = 100)
    {
        _db = redis.GetDatabase();
        _refillRatePerSecond = refillRatePerSecond;
        _maxCapacity = maxCapacity;
    }

    public async Task<bool> IsAllowedAsync(string key, int cost = 1)
    {
        var result = (RedisResult[])await _db.ScriptEvaluateAsync(
            RateLimitScript,
            new RedisKey[] { $"fs:bucket:{key}", $"fs:ts:{key}" },
            new RedisValue[] { _maxCapacity, _refillRatePerSecond, GetCurrentTimestamp(), cost }
        );

        // result[0] is allowed (1 or 0)
        // result[1] is remaining tokens
        return (int)result[0] == 1;
    }

    public Task<RateLimitMetrics> GetMetricsAsync(string key)
    {
        // To be implemented: peek script
        return Task.FromResult(new RateLimitMetrics(0, TimeSpan.Zero));
    }

    private static double GetCurrentTimestamp()
    {
        // Precise double timestamp
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
    }
}
