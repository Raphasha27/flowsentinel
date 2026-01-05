using FlowSentinel.Worker;
using FlowSentinel.Core.AbuseDetection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IAbuseDetector, SpikeDetector>();
builder.Services.AddHostedService<TrafficAnalyticsWorker>();

var host = builder.Build();
host.Run();
