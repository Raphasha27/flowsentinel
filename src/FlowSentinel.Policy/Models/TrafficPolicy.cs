namespace FlowSentinel.Policy.Models;

public enum PolicyScope
{
    Global,
    Service,
    Endpoint,
    User
}

public enum PolicyAction
{
    Allow,
    Block,
    Throttle,
    Challenge
}

public class TrafficPolicy
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public PolicyScope Scope { get; set; }
    public string Target { get; set; } = string.Empty; // e.g., ServiceName, EndpointPath, or UserTier
    
    // Rate Limit Config
    public int Limit { get; set; }
    public TimeSpan Window { get; set; }
    public int Burst { get; set; }
    
    public PolicyAction Action { get; set; } = PolicyAction.Throttle;
    public bool IsEnabled { get; set; } = true;
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
