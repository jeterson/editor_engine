# Task Execution Guide

# Important

Tasks must remain:
- small
- isolated
- testable
- architecture-safe

---

# Task Rules

Each task must:
- affect a single concern
- preserve architecture
- include tests
- avoid unrelated refactors

---

# Recommended Task Size

Good:
- create Layer entity
- implement RenderNode base
- add EffectRegistry

Bad:
- implement full rendering engine
- create complete editor

---

# Required Output

Each task should produce:
- implementation
- tests
- documentation updates if needed
