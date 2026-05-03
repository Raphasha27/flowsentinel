using System.Collections.Concurrent;

namespace FlowSentinel.Core.RateLimiting;

/// <summary>
/// A high-performance local implementation of the Token Bucket algorithm
/// for unit testing and localized rate limiting.
/// </summary>
public class LocalTokenBucketRateLimiter : IRateLimiter
{
    private readonly double _refillRate;
    private readonly long _capacity;
    private readonly ConcurrentDictionary<string, BucketState> _buckets = new();

    private record BucketState(double Tokens, DateTime LastRefill);

    public LocalTokenBucketRateLimiter(double refillRate = 10, long capacity = 100)
    {
        _refillRate = refillRate;
        _capacity = capacity;
    }

    public Task<bool> IsAllowedAsync(string key, int cost = 1)
    {
        var now = DateTime.UtcNow;
        var isAllowed = false;

        _buckets.AddOrUpdate(key, 
            _ => new BucketState(_capacity - cost, now),
            (_, existing) => 
            {
                var delta = (now - existing.LastRefill).TotalSeconds;
                var refilled = Math.Min(_capacity, existing.Tokens + (delta * _refillRate));
                
                if (refilled >= cost)
                {
                    isAllowed = true;
                    return new BucketState(refilled - cost, now);
                }
                
                return existing with { Tokens = refilled, LastRefill = now };
            });

        // The first update handles the case where it's newly added
        if (_buckets.TryGetValue(key, out var state) && state.LastRefill == now)
            isAllowed = true;

        return Task.FromResult(isAllowed);
    }

    public Task<RateLimitMetrics> GetMetricsAsync(string key)
    {
        return Task.FromResult(new RateLimitMetrics(0, TimeSpan.Zero));
    }
}
