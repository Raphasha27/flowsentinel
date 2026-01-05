var builder = WebApplication.CreateBuilder(args);

// 1. Add FlowSentinel Services
// In a real environment, this would point to the shared Redis instance
builder.Services.AddFlowSentinel("sample-api", builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Enable FlowSentinel Middleware
app.UseFlowSentinel("sample-api");

app.MapControllers();

app.Run();
