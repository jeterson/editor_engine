# Render Graph

# Purpose

The Render Graph is responsible for:

- dependency tracking
- partial invalidation
- execution planning
- cache coordination
- render scheduling

---

# Core Principle

Rendering is graph-based, NOT linear.

The graph allows:
- cache reuse
- partial rendering
- parallel execution
- backend optimization

---

# Node Types

## AssetNode

Loads source assets.

---

## TransformNode

Applies transforms.

---

## EffectNode

Applies effects.

---

## CompositeNode

Combines multiple sources.

---

# Invalidation

When a node changes:
- only dependent subgraphs are invalidated

Full rerendering should be avoided whenever possible.

---

# Cache

Render nodes may cache:
- intermediate surfaces
- thumbnails
- GPU resources

Cache invalidation must be deterministic.
