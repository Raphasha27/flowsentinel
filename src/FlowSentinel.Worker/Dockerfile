FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FlowSentinel.Worker/FlowSentinel.Worker.csproj", "src/FlowSentinel.Worker/"]
COPY ["src/FlowSentinel.Core/FlowSentinel.Core.csproj", "src/FlowSentinel.Core/"]
RUN dotnet restore "src/FlowSentinel.Worker/FlowSentinel.Worker.csproj"
COPY . .
WORKDIR "/src/src/FlowSentinel.Worker"
RUN dotnet build "FlowSentinel.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlowSentinel.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlowSentinel.Worker.dll"]
