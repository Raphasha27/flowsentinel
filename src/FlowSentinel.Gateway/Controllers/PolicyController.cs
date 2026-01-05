using Microsoft.AspNetCore.Mvc;
using FlowSentinel.Policy.Models;
using FlowSentinel.Policy.Services;

namespace FlowSentinel.Gateway.Controllers;

[ApiController]
[Route("api/policies")]
public class PolicyController : ControllerBase
{
    private readonly IPolicyStore _policyStore;

    public PolicyController(IPolicyStore policyStore)
    {
        _policyStore = policyStore;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrafficPolicy>>> GetPolicies()
    {
        return Ok(await _policyStore.GetPoliciesAsync());
    }

    [HttpPost]
    public async Task<IActionResult> CreatePolicy(TrafficPolicy policy)
    {
        policy.Id = Guid.NewGuid().ToString();
        policy.CreatedAt = DateTime.UtcNow;
        policy.Version = 1;
        await _policyStore.UpsertPolicyAsync(policy);
        return CreatedAtAction(nameof(GetPolicies), new { id = policy.Id }, policy);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePolicy(string id, TrafficPolicy policy)
    {
        var existing = await _policyStore.GetPolicyByIdAsync(id);
        if (existing == null) return NotFound();

        policy.Id = id;
        policy.Version = existing.Version + 1;
        await _policyStore.UpsertPolicyAsync(policy);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePolicy(string id)
    {
        await _policyStore.DeletePolicyAsync(id);
        return NoContent();
    }
}
