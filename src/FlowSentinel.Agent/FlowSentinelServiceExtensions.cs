using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using FlowSentinel.Core.RateLimiting;
using FlowSentinel.Policy.Services;
using FlowSentinel.Agent.Middleware;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class FlowSentinelServiceExtensions
{
    public static IServiceCollection AddFlowSentinel(this IServiceCollection services, string serviceName, string redisConnection)
    {
        var redis = ConnectionMultiplexer.Connect(redisConnection);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddSingleton<IRateLimiter, RedisTokenBucketRateLimiter>();
        services.AddSingleton<IPolicyEvaluator, PolicyEvaluator>();
        
        return services;
    }

    public static IApplicationBuilder UseFlowSentinel(this IApplicationBuilder app, string serviceName)
    {
        return app.UseMiddleware<FlowSentinelMiddleware>(serviceName);
    }
}
