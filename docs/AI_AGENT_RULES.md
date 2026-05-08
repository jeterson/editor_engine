# AI Agent Rules

# Mandatory Rules

1. NEVER introduce technology-specific types into Domain.
2. NEVER reference infrastructure from Domain.
3. NEVER bypass abstractions.
4. NEVER create giant god classes.
5. NEVER mix rendering with business logic.
6. ALWAYS add tests for new behavior.
7. ALWAYS preserve non-destructive editing.
8. ALWAYS prefer extension over modification.
9. NEVER hardcode effect handling.
10. NEVER couple effects to a specific renderer.

---

# Before Writing Code

The AI agent MUST:
- understand the layer boundaries
- validate dependencies
- check architectural rules

---

# Forbidden Behaviors

The AI agent must NEVER:
- move fast by breaking architecture
- add shortcuts violating boundaries
- expose backend-specific types outside infrastructure
- introduce hidden global state

---

# Required Behaviors

The AI agent SHOULD:
- create small cohesive classes
- create focused abstractions
- create unit tests
- document public contracts
