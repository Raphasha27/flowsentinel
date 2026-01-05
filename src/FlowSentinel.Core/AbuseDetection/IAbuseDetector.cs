namespace FlowSentinel.Core.AbuseDetection;

public interface IAbuseDetector
{
    Task<bool> IsSpikeDetectedAsync(string key, int currentCount, int historicalAverage);
}

public class SpikeDetector : IAbuseDetector
{
    private const double SpikeThresholdMultiplier = 5.0; // 5x average is a spike

    public Task<bool> IsSpikeDetectedAsync(string key, int currentCount, int historicalAverage)
    {
        // Simple logic: If current traffic is 5x higher than average, flag it.
        // In a real system, this would use more complex Z-Score or ML models.
        if (historicalAverage > 0 && currentCount > (historicalAverage * SpikeThresholdMultiplier))
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}
