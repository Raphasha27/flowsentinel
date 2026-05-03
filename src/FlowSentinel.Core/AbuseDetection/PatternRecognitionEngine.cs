using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FlowSentinel.Core.AbuseDetection;

/// <summary>
/// Elite Pattern Recognition: Uses A* trajectory analysis to differentiate 
/// between legitimate user flows and automated scraping/brute-force patterns.
/// </summary>
public class PatternRecognitionEngine
{
    private readonly EliteAiEngine _aiEngine = new();
    private readonly ConcurrentDictionary<string, List<EliteAiEngine.Position>> _clientTrajectories = new();

    // Architectural mapping of endpoints to a logical 2D navigation space
    private static readonly Dictionary<string, EliteAiEngine.Position> EndpointMap = new()
    {
        { "/home",           new(0, 0) },
        { "/products",       new(0, 1) },
        { "/categories",     new(0, 2) },
        { "/cart",           new(1, 1) },
        { "/checkout",       new(2, 1) },
        { "/login",          new(1, 2) },
        { "/register",       new(2, 2) },
        { "/profile",        new(1, 3) },
        { "/admin",          new(5, 5) }, // Architecturally isolated/protected
        { "/api/v1/debug",   new(10, 10) } // Deep internal endpoint
    };

    /// <summary>
    /// Analyzes the behavioral trajectory of a client. 
    /// Flags clients that "jump" between architecturally distant nodes without transitions.
    /// </summary>
    public bool AnalyzeTrajectory(string clientId, string endpoint)
    {
        if (!EndpointMap.TryGetValue(endpoint, out var currentPos))
            return false; // Ignore untracked endpoints

        var isAbusive = false;

        _clientTrajectories.AddOrUpdate(clientId,
            _ => new List<EliteAiEngine.Position> { currentPos },
            (_, trajectory) =>
            {
                var lastPos = trajectory.Last();
                
                // Calculate "Behavioral Jump Distance"
                var path = _aiEngine.FindPath(lastPos, currentPos, new HashSet<EliteAiEngine.Position>(), 20, 20);
                
                // If the path distance is high (>2 hops) but happens in 1 request, 
                // it suggests non-standard navigation (bot/scanner).
                if (path.Count > 3) 
                {
                    isAbusive = true;
                }

                trajectory.Add(currentPos);
                if (trajectory.Count > 10) trajectory.RemoveAt(0); // Keep window small
                return trajectory;
            });

        return isAbusive;
    }
}
