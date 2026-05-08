# Domain Rules

# Purpose

The domain models the editor state and editing behavior.

The domain DOES NOT render pixels.

---

# Domain Responsibilities

The domain contains:

- document structure
- layers
- transformations
- effect stacks
- masks
- metadata
- history
- selection state

---

# Domain Must Not

The domain must NEVER:

- manipulate bitmaps
- access GPU
- allocate rendering surfaces
- read/write files directly
- execute shaders
- decode image formats

---

# Domain Entities

## EditorDocument

Represents the editable document.

Contains:
- layers
- canvas settings
- metadata

---

## Layer

Represents a visual element.

Contains:
- source asset reference
- transforms
- effect stack
- blend mode
- opacity

---

## EffectStack

Contains ordered effects.

Supports:
- add
- remove
- reorder
- enable/disable

---

# Non-Destructive Editing

All editing operations must be declarative.

The document stores:
- operations
- parameters
- references

The document NEVER stores permanently modified output bitmaps.
