using FlowSentinel.Policy.Models;

namespace FlowSentinel.Policy.Services;

public interface IPolicyEvaluator
{
    Task<TrafficPolicy?> GetBestMatchingPolicyAsync(string service, string endpoint, string? clientId);
}

public class PolicyEvaluator : IPolicyEvaluator
{
    // In a real implementation, this would fetch from PostgreSQL and cache in-memory
    // For now, we'll provide a mock implementation that demonstrates the 'Hot Reload' concept via a simulated store.
    private readonly List<TrafficPolicy> _policies = new()
    {
        new TrafficPolicy 
        { 
            Id = "global-default",
            Name = "Global Safety Net",
            Scope = PolicyScope.Global,
            Limit = 1000,
            Window = TimeSpan.FromMinutes(1),
            Burst = 100
        },
        new TrafficPolicy
        {
            Id = "service-orders",
            Name = "Orders Service Limit",
            Scope = PolicyScope.Service,
            Target = "orders-api",
            Limit = 100,
            Window = TimeSpan.FromMinutes(1),
            Burst = 10
        }
    };

    public Task<TrafficPolicy?> GetBestMatchingPolicyAsync(string service, string endpoint, string? clientId)
    {
        // 1. Check for specific Endpoint policy (Highest Priority)
        var endpointPolicy = _policies.FirstOrDefault(p => p.IsEnabled && p.Scope == PolicyScope.Endpoint && p.Target == $"{service}:{endpoint}");
        if (endpointPolicy != null) return Task.FromResult<TrafficPolicy?>(endpointPolicy);

        // 2. Check for Service policy
        var servicePolicy = _policies.FirstOrDefault(p => p.IsEnabled && p.Scope == PolicyScope.Service && p.Target == service);
        if (servicePolicy != null) return Task.FromResult<TrafficPolicy?>(servicePolicy);

        // 3. Fallback to Global
        var globalPolicy = _policies.FirstOrDefault(p => p.IsEnabled && p.Scope == PolicyScope.Global);
        return Task.FromResult<TrafficPolicy?>(globalPolicy);
    }
}
