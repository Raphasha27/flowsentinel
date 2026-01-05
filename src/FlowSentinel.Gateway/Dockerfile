FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/FlowSentinel.Gateway/FlowSentinel.Gateway.csproj", "src/FlowSentinel.Gateway/"]
COPY ["src/FlowSentinel.Core/FlowSentinel.Core.csproj", "src/FlowSentinel.Core/"]
COPY ["src/FlowSentinel.Policy/FlowSentinel.Policy.csproj", "src/FlowSentinel.Policy/"]
RUN dotnet restore "src/FlowSentinel.Gateway/FlowSentinel.Gateway.csproj"
COPY . .
WORKDIR "/src/src/FlowSentinel.Gateway"
RUN dotnet build "FlowSentinel.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlowSentinel.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlowSentinel.Gateway.dll"]
