# Project Structure

```text
src/
 ├── Engine.Domain
 ├── Engine.Application
 ├── Engine.Abstractions
 ├── Engine.RenderGraph
 ├── Engine.Shared
 ├── Engine.Infrastructure
 ├── Engine.Infrastructure.CPU
 ├── Engine.Infrastructure.Skia
 ├── Engine.Infrastructure.Win2D

tests/
 ├── Engine.Domain.Tests
 ├── Engine.Application.Tests
 ├── Engine.RenderGraph.Tests
 ├── Engine.Architecture.Tests
```

---

# Rules

## Domain

Contains only business rules.

---

## Infrastructure

Contains external integrations.

---

## RenderGraph

Contains rendering orchestration.

---

## Tests

Each layer should have isolated tests.
