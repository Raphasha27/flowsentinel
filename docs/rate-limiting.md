# Rate Limiting Design

## Core Algorithm: Token Bucket

We use a **distributed Token Bucket** algorithm backed by Redis.

### Why Token Bucket?
Unlike "Fixed Window" which causes stampeding at window boundaries, or "Sliding Window Log" which is expensive to store (one entry per request), **Token Bucket** is:
1.  **Memory Efficient**: Only stores `tokens` (float) and `last_updated` (timestamp) for each user.
2.  **Burst Capable**: Allows short bursts of traffic up to the bucket capacity.
3.  **Smooth Refill**: Tokens are added continuously based on the rate.

### Distributed Consistency with Lua
To prevent race conditions where two concurrent requests both read available tokens and try to subtract, we use **Redis Lua Scripts**.

The script guarantees atomicity:
1.  **Read** current tokens & timestamp.
2.  **Calculate** refill based on `now - last_updated`.
3.  **Cap** at `max_capacity`.
4.  **Subtract** cost if sufficient tokens exist.
5.  **Write** new values.

All of this happens in a single Redis round-trip, ensuring that even under high concurrency, the rate limit is strictly enforced.

### Fallback Strategies
*   **Fail-Open**: If Redis is unreachable, the system defaults to allowing traffic to prevent cascading failures (unless configured otherwise for high-security zones).
