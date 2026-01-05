# Check if dotnet is installed
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: .NET SDK is not installed or not in PATH." -ForegroundColor Red
    exit 1
}

# Create Solution File
if (-not (Test-Path "FlowSentinel.sln")) {
    Write-Host "Creating Solution File..."
    dotnet new sln -n FlowSentinel
}

# Add Projects to Solution
Write-Host "Adding projects to solution..."
dotnet sln add src/FlowSentinel.Gateway/FlowSentinel.Gateway.csproj
dotnet sln add src/FlowSentinel.Core/FlowSentinel.Core.csproj
dotnet sln add src/FlowSentinel.Policy/FlowSentinel.Policy.csproj
dotnet sln add src/FlowSentinel.Agent/FlowSentinel.Agent.csproj
dotnet sln add src/FlowSentinel.Worker/FlowSentinel.Worker.csproj
dotnet sln add tests/FlowSentinel.UnitTests/FlowSentinel.UnitTests.csproj
dotnet sln add tests/FlowSentinel.IntegrationTests/FlowSentinel.IntegrationTests.csproj

Write-Host "Solution setup complete!" -ForegroundColor Green
