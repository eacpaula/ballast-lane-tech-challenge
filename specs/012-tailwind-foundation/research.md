# Research: Tailwind Frontend Foundation

## Decision 1: Use Tailwind's Vite integration path for installation

- **Decision**: Install TailwindCSS using the Vite-specific integration path
  instead of introducing a separate CSS build pipeline.
- **Rationale**: The frontend already runs on Vite, and the official Tailwind
  documentation provides a direct Vite integration route that keeps the setup
  smaller and easier to explain in an interview.
- **Alternatives considered**:
  - Tailwind CLI pipeline: workable, but adds a separate build concern that the
    current app does not need.
  - Broader PostCSS-centric setup: possible, but unnecessary for a small Vite
    app unless a later feature demands it.

## Decision 2: Keep token mapping centralized in the styling foundation

- **Decision**: Translate `DESIGN.md` into centralized theme tokens for colors,
  fonts, spacing, radius, shadows, and layout constraints.
- **Rationale**: The updated constitution requires styling centralization and
  reusable patterns. A shared token layer gives later components one canonical
  place to inherit the approved design direction.
- **Alternatives considered**:
  - Continue using ad hoc CSS variables in the starter stylesheet: too easy to
    drift away from `DESIGN.md`.
  - Encode visual values directly in each component: violates the design-system
    governance principle and scales poorly.

## Decision 3: Use a clean global stylesheet as the single entry point

- **Decision**: Keep one frontend stylesheet entry point that loads Tailwind and
  defines the shared base layer needed by the app.
- **Rationale**: The current frontend already imports a single global stylesheet
  from `main.tsx`. Preserving that flow keeps the setup understandable while
  allowing token and base-style centralization.
- **Alternatives considered**:
  - Multiple disconnected global stylesheets: harder to reason about and
    easier to fragment.
  - CSS-in-JS for the initial foundation: unnecessary complexity for this
    project size.

## Decision 4: Validate the setup with a static shell, not full screens

- **Decision**: Replace the starter Vite demo with one small validation shell
  that demonstrates typography, spacing, surfaces, and interaction states.
- **Rationale**: The feature scope is infrastructure, not feature delivery. A
  shell proves the styling foundation works without accidentally expanding into
  post listing, authentication, or admin flows.
- **Alternatives considered**:
  - Keep the existing Vite starter UI: it does not reflect `DESIGN.md`.
  - Build a fuller landing page: more visually complete, but outside the
    intentionally narrow scope.

## Decision 5: Accessibility begins in the base layer

- **Decision**: Include readable typography, visible focus styles, semantic
  structure, and contrast-aware defaults in the first global style pass.
- **Rationale**: The constitution now requires accessibility from the start.
  The base layer is the right place to prevent inaccessible defaults from
  propagating into later components.
- **Alternatives considered**:
  - Defer accessibility until component implementation: cheaper short term, but
    it allows poor defaults to spread and increases cleanup cost later.

## Decision 6: Defer component-library breadth and dark mode

- **Decision**: Do not add a broader component library, dark mode system, or
  extra UI framework in this slice.
- **Rationale**: `DESIGN.md` defines a clear light-theme direction and the
  feature spec explicitly excludes heavy frameworks and broader UI scope.
- **Alternatives considered**:
  - Add a prebuilt component framework: faster initial scaffolding, but adds
    weight and weakens the interview value of the custom design direction.
  - Add dark mode now: possible, but not required and likely to distract from
    the primary styling foundation goal.
