# Chaos Testing & Resiliency

FlowSentinel is designed with the **Infrastructure-First** principle: *The traffic controller must never be the cause of a total system outage.*

## 🛡️ Fail-Open Design

In a distributed system, dependencies will fail. If FlowSentinel's connection to Redis is severed, we exhibit **Fail-Open** behavior by default.

### 1. Circuit Breaker on the Agent
If the FlowSentinel Agent cannot communicate with the Core/Redis:
- **Action**: It catches the exception.
- **Result**: It logs the failure and immediately calls `_next(context)`.
- **Reasoning**: It is better to allow occasional over-limit traffic than to block 100% of legitimate traffic because the rate-limiter is down.

### 2. Policy Engine Resilience
If the Policy Engine cannot be reached:
- **Action**: The Agent falls back to a locally cached "Emergency Policy" or simply allows the traffic.

## 🧪 Chaos Scenarios

### Scenario A: Redis Crash
**Simulation**: `docker-compose stop redis`
**Expected Outcome**: 
- Gateway logs errors.
- Microservices using the Agent continue to serve requests.
- Rate limits are temporarily NOT enforced.
- System recovers automatically when Redis is restarted.

### Scenario B: High Latency
**Simulation**: Introduce 500ms network delay to Redis.
**Expected Outcome**:
- The Agent uses a `CancellationToken` with a strict timeout (e.g., 50ms).
- If the limit check exceeds 50ms, it "Fails-Open" to preserve the target service's latency SLA.

## 📊 Recovery (Fail-Back)
Once dependencies stabilize, FlowSentinel's stateful counters resume. Brief spikes during the "window of failure" are tolerated as the price of availability.
