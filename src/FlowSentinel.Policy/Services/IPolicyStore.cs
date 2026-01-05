using FlowSentinel.Policy.Models;

namespace FlowSentinel.Policy.Services;

public interface IPolicyStore
{
    Task<IEnumerable<TrafficPolicy>> GetPoliciesAsync();
    Task<TrafficPolicy?> GetPolicyByIdAsync(string id);
    Task UpsertPolicyAsync(TrafficPolicy policy);
    Task DeletePolicyAsync(string id);
}

public class LocalPolicyStore : IPolicyStore
{
    private readonly List<TrafficPolicy> _policies = new();

    public Task<IEnumerable<TrafficPolicy>> GetPoliciesAsync() => Task.FromResult(_policies.AsEnumerable());

    public Task<TrafficPolicy?> GetPolicyByIdAsync(string id) => Task.FromResult(_policies.FirstOrDefault(p => p.Id == id));

    public Task UpsertPolicyAsync(TrafficPolicy policy)
    {
        var existing = _policies.FirstOrDefault(p => p.Id == policy.Id);
        if (existing != null) _policies.Remove(existing);
        _policies.Add(policy);
        return Task.CompletedTask;
    }

    public Task DeletePolicyAsync(string id)
    {
        var existing = _policies.FirstOrDefault(p => p.Id == id);
        if (existing != null) _policies.Remove(existing);
        return Task.CompletedTask;
    }
}
