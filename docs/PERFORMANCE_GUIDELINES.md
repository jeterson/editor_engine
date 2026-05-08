# Performance Guidelines

# Performance Philosophy

Performance is a core feature.

Avoid architecture that:
- rerenders everything
- reallocates excessively
- copies memory unnecessarily

---

# Goals

The engine should support:
- partial invalidation
- tile rendering
- render caching
- progressive rendering
- parallel execution

---

# Rules

## Avoid Full Rerenders

Only rerender invalidated graph regions.

---

## Avoid Excessive Allocation

Prefer pooling and reusable buffers.

---

## Avoid UI Thread Blocking

Rendering should be asynchronous whenever possible.

---

## GPU Is Optional

The engine must function correctly:
- with GPU
- without GPU

GPU acceleration is an optimization layer, not a requirement.
