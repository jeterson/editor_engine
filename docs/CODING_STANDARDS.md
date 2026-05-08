# Coding Standards

# General Rules

- Prefer composition over inheritance
- Prefer immutable value objects
- Avoid static mutable state
- Avoid service locators
- Avoid god classes

---

# Naming

Interfaces:
- IRenderBackend
- IImageSurface

Entities:
- EditorDocument
- Layer

Processors:
- CpuBlurProcessor

---

# Async

Avoid:
- async void

Prefer:
- async Task
- ValueTask when appropriate

---

# Exceptions

Exceptions should be exceptional.

Use Result<T> for expected failures.

---

# Testing

All business logic must be testable without:
- GPU
- file system
- UI
