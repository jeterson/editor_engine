# Architecture

## Goal

Build a high-performance, non-destructive image editing engine with:

- Clean Architecture
- DDD
- technology-independent core
- CPU/GPU hybrid rendering
- Render Graph based execution
- extensible effect system
- pluggable rendering backends
- pluggable codecs
- testability
- high scalability

---

# Architectural Principles

## Core Principles

1. Domain must not know infrastructure.
2. Domain must not know rendering technology.
3. Rendering must be replaceable.
4. Effects must be extensible without modifying existing code.
5. Editing must be non-destructive.
6. Rendering must support partial invalidation.
7. Performance is a first-class requirement.
8. All rendering operations must be backend-agnostic.
9. Memory allocations must be minimized.
10. CPU and GPU execution must coexist.

---

# Layers

## Domain

Contains:
- entities
- value objects
- business rules
- domain events

Must NOT contain:
- rendering logic
- Win2D
- Skia
- ImageSharp
- SoftwareBitmap
- StorageFile
- file system access
- GPU code

---

## Application

Contains:
- use cases
- orchestration
- command execution
- undo/redo coordination

Must NOT contain:
- rendering implementation
- UI logic
- backend-specific logic

---

## RenderGraph

Contains:
- render graph structure
- node dependency resolution
- invalidation system
- graph execution planning

Must NOT contain:
- UI
- domain business rules

---

## Infrastructure

Contains:
- rendering backends
- codecs
- file system
- GPU execution
- hardware integration

Infrastructure implements abstractions from:
- Domain
- Abstractions
- RenderGraph

---

## Presentation

Contains:
- UI
- interaction
- editor tools
- viewport

Must NOT contain:
- business rules
- rendering internals
