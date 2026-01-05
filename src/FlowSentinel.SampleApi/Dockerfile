FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FlowSentinel.SampleApi/FlowSentinel.SampleApi.csproj", "src/FlowSentinel.SampleApi/"]
COPY ["src/FlowSentinel.Agent/FlowSentinel.Agent.csproj", "src/FlowSentinel.Agent/"]
COPY ["src/FlowSentinel.Core/FlowSentinel.Core.csproj", "src/FlowSentinel.Core/"]
COPY ["src/FlowSentinel.Policy/FlowSentinel.Policy.csproj", "src/FlowSentinel.Policy/"]
RUN dotnet restore "src/FlowSentinel.SampleApi/FlowSentinel.SampleApi.csproj"
COPY . .
WORKDIR "/src/src/FlowSentinel.SampleApi"
RUN dotnet build "FlowSentinel.SampleApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlowSentinel.SampleApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlowSentinel.SampleApi.dll"]
