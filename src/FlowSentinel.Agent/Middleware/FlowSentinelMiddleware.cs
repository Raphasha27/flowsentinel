using Microsoft.AspNetCore.Http;
using FlowSentinel.Core.RateLimiting;
using FlowSentinel.Policy.Services;
using FlowSentinel.Policy.Models;
using System.Net;

namespace FlowSentinel.Agent.Middleware;

public class FlowSentinelMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiter _rateLimiter;
    private readonly IPolicyEvaluator _policyEvaluator;
    private readonly string _serviceName;

    public FlowSentinelMiddleware(
        RequestDelegate next, 
        IRateLimiter rateLimiter, 
        IPolicyEvaluator policyEvaluator,
        string serviceName)
    {
        _next = next;
        _rateLimiter = rateLimiter;
        _policyEvaluator = policyEvaluator;
        _serviceName = serviceName;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.Request.Path.Value ?? "/";
        var clientId = context.Request.Headers["X-Client-Id"].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString();

        // 1. Resolve Policy
        var policy = await _policyEvaluator.GetBestMatchingPolicyAsync(_serviceName, endpoint, clientId);

        if (policy == null)
        {
            // Fail-open strategy
            await _next(context);
            return;
        }

        // 2. Enforce Rate Limit
        var key = $"{policy.Id}:{clientId}";
        bool isAllowed;
        
        try 
        {
            // Use a 50ms timeout for the rate-limiting check to avoid blocking the request pipeline
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
            isAllowed = await _rateLimiter.IsAllowedAsync(key); 
            // Note: In a production version, IRateLimiter would accept a CancellationToken
        }
        catch (Exception ex)
        {
            // FAIL-OPEN: Log the error and allow traffic to flow
            // In a real app, use ILogger here
            Console.WriteLine($"[FlowSentinel] Rate limiter failed: {ex.Message}. Failing OPEN.");
            await _next(context);
            return;
        }

        if (!isAllowed)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.Headers["Retry-After"] = "60"; // Simplified for now
            await context.Response.WriteAsJsonAsync(new 
            { 
                Message = "Traffic limit exceeded. FlowSentinel restricted your access.",
                Policy = policy.Name,
                Action = policy.Action.ToString()
            });
            return;
        }

        // 3. Process Request
        await _next(context);
    }
}
