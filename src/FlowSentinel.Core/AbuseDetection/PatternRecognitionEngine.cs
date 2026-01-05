namespace FlowSentinel.Core.AbuseDetection;

public record TrafficPattern(string ClientId, string[] Endpoints, DateTime FirstSeen);

public class PatternRecognitionEngine
{
    private readonly Dictionary<string, TrafficPattern> _activePatterns = new();
    private readonly string[] _sensitiveEndpoints = { "/login", "/register", "/reset-password" };

    public bool Analysis(string clientId, string endpoint)
    {
        if (!_sensitiveEndpoints.Contains(endpoint)) return false;

        if (!_activePatterns.TryGetValue(clientId, out var pattern))
        {
            _activePatterns[clientId] = new TrafficPattern(clientId, new[] { endpoint }, DateTime.UtcNow);
            return false;
        }

        // If the client has hit 3 sensitive endpoints in less than 2 seconds, flag it.
        var updatedEndpoints = pattern.Endpoints.Append(endpoint).ToArray();
        _activePatterns[clientId] = pattern with { Endpoints = updatedEndpoints };

        if (updatedEndpoints.Length >= 3 && (DateTime.UtcNow - pattern.FirstSeen).TotalSeconds < 2)
        {
            return true; // Suspicious pattern detected
        }

        return false;
    }
}
