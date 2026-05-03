# ─────────────────────────────────────────────────────────
# FlowSentinel — Developer Makefile
# Kirov Dynamics Engineering Standard
# ─────────────────────────────────────────────────────────

.PHONY: help build run test lint docker-up docker-down clean

help:
	@echo ""
	@echo "  FlowSentinel — Available Commands"
	@echo "  ─────────────────────────────────────────────"
	@echo "  make build        Build all projects (dotnet build)"
	@echo "  make run          Run the dashboard and gateway locally"
	@echo "  make test         Run all unit and integration tests"
	@echo "  make lint         Run code analysis"
	@echo "  make docker-up    Start full stack via docker-compose"
	@echo "  make docker-down  Tear down docker stack"
	@echo "  make clean        Remove build artifacts"
	@echo ""

# ─── Build ───────────────────────────────────────────────
build:
	@echo "→ Building solutions..."
	dotnet build

# ─── Run ─────────────────────────────────────────────────
run:
	@echo "→ Starting FlowSentinel Gateway..."
	dotnet run --project src/FlowSentinel.Gateway &
	@echo "→ Starting FlowSentinel Dashboard..."
	dotnet run --project src/FlowSentinel.Dashboard &
	@wait

# ─── Testing ─────────────────────────────────────────────
test:
	@echo "→ Running Unit Tests..."
	dotnet test tests/FlowSentinel.UnitTests
	@echo "→ Running Integration Tests..."
	dotnet test tests/FlowSentinel.IntegrationTests

# ─── Linting ─────────────────────────────────────────────
lint:
	@echo "→ Running dotnet format analysis..."
	dotnet format --verify-no-changes

# ─── Docker ──────────────────────────────────────────────
docker-up:
	docker-compose up --build -d
	@echo "✅ FlowSentinel stack running."

docker-down:
	docker-compose down -v
	@echo "🛑 FlowSentinel stack stopped."

# ─── Cleanup ─────────────────────────────────────────────
clean:
	@echo "→ Cleaning bin and obj folders..."
	find . -type d -name "bin" -exec rm -rf {} + 2>/dev/null || true
	find . -type d -name "obj" -exec rm -rf {} + 2>/dev/null || true
	@echo "✅ Cleaned."
