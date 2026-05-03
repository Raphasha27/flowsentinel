# 🌋 FlowSentinel: Chaos Testing & Resiliency Manifesto

FlowSentinel is mission-critical infrastructure. If FlowSentinel stays down, every service depending on it suffers. This manifesto outlines our approach to high availability.

## 🏛️ 1. Fail-Open Architecture
*   **The Golden Rule**: Rate limiting should never be the cause of a system-wide outage.
*   **Implementation**: Use `try-catch-allow` patterns. If the Redis cluster or the FlowSentinel Gateway is unreachable, the system MUST fail-open and allow the request.

## ⏱️ 2. Latency Budgeting (The 50ms Rule)
*   **Constraint**: No traffic decision should take longer than 50ms.
*   **Strategy**: Strict timeouts on all Redis Lua script executions. If a decision isn't reached in time, refer to the Fail-Open rule.

## 🧪 3. Chaos Experiments
We regularly simulate the following failures:
1.  **State Loss**: Sudden termination of the Redis primary.
2.  **Partitioning**: Network latency induction between the Gateway and the Policy Store.
3.  **Thundering Herd**: Simulate 100k requests/sec from a single ClientId to verify Lua script atomicity.

## 📈 4. Observability Under Stress
We measure:
*   **p99 Latency** specifically during a Redis failover.
*   **Decision Accuracy** during clock drift events.

---
*Built for the 99.999%.*
