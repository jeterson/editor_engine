# Dependency Rules

# Allowed Dependencies

## Domain

Can reference:
- Engine.Shared
- standard library only

Cannot reference:
- Infrastructure
- UI
- Render backend
- WinRT
- Win2D
- Skia
- ImageSharp

---

## Application

Can reference:
- Domain
- Abstractions
- Shared

Cannot reference:
- Infrastructure implementation
- UI frameworks

---

## RenderGraph

Can reference:
- Abstractions
- Shared

Cannot reference:
- UI
- Win2D
- Skia
- Presentation

---

## Infrastructure

Can reference:
- all abstraction layers

Implements:
- rendering backends
- codecs
- hardware acceleration

---

# Forbidden Types Outside Infrastructure

The following types MUST NEVER appear outside Infrastructure:

- SoftwareBitmap
- StorageFile
- CanvasBitmap
- SKBitmap
- SKImage
- MagickImage
- ImageSharp.Image
- Direct3D objects
- GPU handles

---

# Enforcement

Architectural tests MUST validate:
- forbidden references
- forbidden namespaces
- forbidden dependencies

Roslyn analyzers should be added later.
