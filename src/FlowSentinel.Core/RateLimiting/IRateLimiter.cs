namespace FlowSentinel.Core.RateLimiting;

public interface IRateLimiter
{
    /// <summary>
    /// Checks if a request should be allowed.
    /// </summary>
    /// <param name="key">Unique identifier for the client/bucket (e.g., API Key, IP).</param>
    /// <param name="cost">Number of tokens to consume (default 1).</param>
    /// <returns>True if allowed, False if limited.</returns>
    Task<bool> IsAllowedAsync(string key, int cost = 1);
    
    /// <summary>
    /// Gets current metrics for a specific key.
    /// </summary>
    Task<RateLimitMetrics> GetMetricsAsync(string key);
}

public record RateLimitMetrics(long RemainingTokens, TimeSpan ResetIn);
