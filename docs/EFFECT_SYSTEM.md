# Effect System

# Goals

Effects must be:
- extensible
- backend-independent
- composable
- non-destructive

---

# Separation of Concerns

Effects are divided into:

## Effect Definition

Domain object containing:
- parameters
- metadata
- identity

Example:
- BlurEffect
- BrightnessEffect

---

## Effect Processor

Infrastructure implementation responsible for execution.

Example:
- CpuBlurProcessor
- SkiaBlurProcessor
- Win2DBlurProcessor

---

# Open/Closed Principle

New effects must be added WITHOUT modifying:
- render graph
- existing effects
- engine core

---

# Effect Registry

Effects must be discoverable through:
- metadata
- descriptors
- registries

No hardcoded switch statements allowed.
