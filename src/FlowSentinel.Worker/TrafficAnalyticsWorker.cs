using FlowSentinel.Core.AbuseDetection;

namespace FlowSentinel.Worker;

public class TrafficAnalyticsWorker : BackgroundService
{
    private readonly ILogger<TrafficAnalyticsWorker> _logger;
    private readonly IAbuseDetector _abuseDetector;

    public TrafficAnalyticsWorker(ILogger<TrafficAnalyticsWorker> logger, IAbuseDetector abuseDetector)
    {
        _logger = logger;
        _abuseDetector = abuseDetector;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Traffic Analytics Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Simulate reading from a message bus or stream
            _logger.LogDebug("Processing traffic event stream...");

            // Logic here would:
            // 1. Fetch recent traffic counts
            // 2. Run abuse detection
            // 3. Trigger automatic blocks if spike detected
            
            await Task.Delay(5000, stoppingToken);
        }
    }
}
