using StackExchange.Redis;

namespace FlowSentinel.Core.AbuseDetection;

public class RollingWindowAbuseDetector
{
    private readonly IDatabase _db;
    private readonly int _windowSeconds;
    private readonly int _threshold;

    public RollingWindowAbuseDetector(IConnectionMultiplexer redis, int windowSeconds = 60, int threshold = 500)
    {
        _db = redis.GetDatabase();
        _windowSeconds = windowSeconds;
        _threshold = threshold;
    }

    /// <summary>
    /// Records a request and checks if it violates a density threshold.
    /// This uses a Redis Sorted Set (ZSET) to store timestamps.
    /// </summary>
    public async Task<bool> IsAbusiveAsync(string clientId)
    {
        var key = $"fs:abuse:{clientId}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowStart = now - _windowSeconds;

        var transaction = _db.CreateTransaction();

        // 1. Add current request timestamp
        _ = transaction.SortedSetAddAsync(key, now, now);

        // 2. Remove old entries outside the window
        _ = transaction.SortedSetRemoveRangeByScoreAsync(key, 0, windowStart);

        // 3. Count remaining entries in the window
        var countTask = transaction.SortedSetLengthAsync(key);

        await transaction.ExecuteAsync();

        var currentRequestCount = await countTask;

        // If count exceeds threshold, it's abusive
        return currentRequestCount > _threshold;
    }
}
