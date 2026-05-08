# Memory Management

# Goals

- minimize allocations
- support huge images
- reduce GC pressure
- support pooling
- support unmanaged memory

---

# Rules

## Avoid Large Managed Allocations

Prefer:
- Span<T>
- Memory<T>
- IMemoryOwner<T>

Avoid:
- repeated byte[]
- unnecessary copying

---

# Surface Pooling

Rendering surfaces should be reusable.

Temporary surfaces should come from pools.

---

# Disposal

All native resources MUST be disposable.

GPU resources must never leak.

---

# Large Images

The engine must eventually support:
- tile rendering
- streaming decode
- partial loading
