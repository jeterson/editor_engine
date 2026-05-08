# Testing Guidelines

# Required Tests

## Domain Tests

Must validate:
- business rules
- invariants
- effect ordering
- undo/redo behavior

---

## Architectural Tests

Must validate:
- forbidden references
- forbidden namespaces
- dependency rules

---

## RenderGraph Tests

Must validate:
- invalidation
- dependency resolution
- cache reuse

---

## Performance Tests

Must validate:
- allocation limits
- cache effectiveness
- render scalability

---

# Forbidden Tests

Do not create tests tightly coupled to:
- UI
- GPU hardware
- operating system behavior
