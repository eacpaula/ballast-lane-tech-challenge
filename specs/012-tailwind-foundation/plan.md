# Implementation Plan: Tailwind Frontend Foundation

**Branch**: `012-tailwind-foundation` | **Date**: 2026-05-21 | **Spec**:
`specs/012-tailwind-foundation/spec.md`

**Input**: Feature specification from
`/specs/012-tailwind-foundation/spec.md`

**Note**: This plan focuses on establishing the frontend styling foundation for
the existing Vite React app. It covers Tailwind installation, theme-token
mapping from `DESIGN.md`, a clean global stylesheet, a minimal validation shell,
and documentation needed for local interview-demo usage.

## Summary

Prepare the existing frontend for future UI work without expanding into full
pages or backend integration.

- Install only the minimum Tailwind dependencies required for the current Vite
  React app.
- Replace the starter visual scaffold with a small validation shell that proves
  shared theme tokens and base styles are active.
- Centralize design tokens for color, typography, spacing, radius, shadows, and
  layout in the frontend styling foundation rather than scattering arbitrary
  values across components.
- Keep the frontend architecture simple: one global stylesheet entry point,
  Tailwind configuration scoped to the existing app, and no heavy component
  framework.

## Implementation Design

**1. Technical approach**:

- Treat this feature as frontend infrastructure, not page delivery.
- Follow the current Tailwind guidance for Vite integration so the setup stays
  small and aligned with the toolchain already in the repo.
- Translate `DESIGN.md` into reusable theme tokens and a few base style rules
  instead of one-off component CSS.
- Use a minimal shell to validate typography, color, spacing, container width,
  surface styling, and accessible focus behavior.

**2. Files and folders affected**:

- `src/frontend/blog-web/package.json`
- `src/frontend/blog-web/package-lock.json`
- `src/frontend/blog-web/vite.config.ts`
- `src/frontend/blog-web/src/index.css`
- `src/frontend/blog-web/src/App.tsx`
- `src/frontend/blog-web/src/App.css` or removal of it if redundant after
  Tailwind setup
- optional new frontend structure such as:
  - `src/frontend/blog-web/src/components/`
  - `src/frontend/blog-web/src/styles/`
- `src/frontend/blog-web/README.md` and/or root `README.md`
- no planned changes to backend source, API contracts, or database files

**3. Dependency installation strategy**:

- Add the Tailwind packages needed for the Vite-based workflow and nothing more.
- Avoid PostCSS-heavy or framework-heavy setup unless the current frontend
  shape proves it necessary.
- Keep frontend dependencies interview-scale: Tailwind itself plus the Vite
  integration path, with no separate UI kit or design-system framework.

**4. Tailwind configuration strategy**:

- Configure Tailwind around the existing `blog-web` app only.
- Ensure Tailwind scans the real source entry points and any new shared
  component/style folders created for this slice.
- Prefer the smallest configuration surface that still exposes centralized theme
  tokens and predictable utility generation.
- Keep the root styling flow easy to explain: app entry imports one stylesheet,
  that stylesheet loads Tailwind and defines shared tokens and base rules.

**5. Design token mapping strategy**:

- Map the approved design direction into named tokens for:
  - background and surface colors
  - headline, body, and code/label typography
  - spacing rhythm and responsive gutters
  - border radius
  - low-contrast outlines
  - ambient shadow depth
  - centered content width
- Translate the design direction faithfully without copying Ballast Lane
  branding directly.
- Only encode the tokens needed to support the first layer of layout and
  primitives; defer broader component-library breadth.

**6. Global styles strategy**:

- Replace the current starter CSS variables and demo styling with a clean global
  stylesheet driven by Tailwind and shared design tokens.
- Keep global styles limited to the concerns that should truly be global:
  typography defaults, background/surface defaults, selection of shared layout
  constraints, and accessible focus treatment.
- Avoid recreating large bespoke CSS modules for the validation shell when
  utilities and a few shared base rules are sufficient.

**7. Minimal validation UI strategy**:

- Replace the Vite starter content with a small static shell that proves the
  setup works.
- Include only the minimum visual elements needed to validate the foundation:
  a centered layout container, headline hierarchy, supporting body text, one
  surface/card, one button-like action, and one metadata/tag treatment.
- Use semantic HTML and visible focus states so accessibility is part of the
  first proof, not deferred work.
- Do not add routing, API data loading, authentication UI, or admin workflows.

**8. Frontend validation/build strategy**:

- Use the existing frontend package scripts as the primary validation path.
- Validate the slice through:
  - dependency install success
  - local dev startup
  - production build success
  - lint success or documented lint follow-up if existing template issues are
    encountered
  - manual verification that the shell reflects the intended tokenized design
    direction on desktop and mobile widths
- Keep validation realistic for interview usage rather than introducing browser
  automation at this stage.

**9. Documentation update strategy**:

- Update frontend-local documentation with the commands needed to install,
  start, and build the app.
- Document where the design tokens live and how `DESIGN.md` drives future UI
  work.
- Keep documentation short and operational rather than turning it into a full
  frontend handbook.

**10. Risks and trade-offs**:

- Tailwind setup adds a small dependency/configuration surface, but it reduces
  future inconsistency compared with continuing from the starter CSS.
- A tokenized foundation introduces some upfront design work, but it prevents
  repeated ad hoc styling later.
- Keeping the validation shell minimal may feel less impressive than a fuller
  page, but it preserves scope discipline and makes the setup easy to review.
- Deferring a larger component library keeps the project maintainable; if the
  next frontend slices reveal repeated patterns, shared primitives can grow from
  this base incrementally.

## Technical Context

**Language/Version**: TypeScript with React 19 on Vite 8

**Primary Dependencies**: React, Vite, TailwindCSS, Tailwind Vite integration,
existing ESLint/TypeScript toolchain

**Storage**: None for this slice

**Testing**: Frontend validation through install, lint/build commands, and
manual verification of the minimal shell; no backend tests affected

**Target Platform**: Browser-based frontend served by the existing Vite app

**Project Type**: Frontend infrastructure slice within a full-stack monorepo

**Performance Goals**: Fast local startup, clean build output, and styling
foundation suitable for interview-scale UI work

**Constraints**: Follow `DESIGN.md`; keep styling centralized; no heavy UI
framework; no backend/API integration; no direct Ballast Lane branding

**Scale/Scope**: Small frontend foundation slice covering configuration, tokens,
global styles, validation shell, and documentation only

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: no backend behavior changes are planned, so backend
  fail-first testing is not implicated by this slice; validation remains focused
  on frontend install/build verification and a minimal shell proof.
- PASS Backend-first: the backend use-case and API foundation already exists,
  and this slice only prepares the frontend styling base that depends on that
  prior stability.
- PASS Architecture: changes remain isolated to the frontend app, with no new
  coupling from frontend into backend or infrastructure code.
- PASS Data access: no persistence work is included.
- PASS Security: no authentication, authorization, or protected workflow logic
  is altered.
- PASS Scope: the plan stops at Tailwind setup, token mapping, a validation
  shell, and documentation, with full pages explicitly deferred.
- PASS API consistency: no API contracts are changed.
- PASS Frontend governance: the slice is explicitly driven by `DESIGN.md`,
  centralized Tailwind tokens, reusable styling patterns, simple architecture,
  and accessibility from the first implementation.

## Project Structure

### Documentation (this feature)

```text
specs/012-tailwind-foundation/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-style-foundation-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/frontend/blog-web/
├── package.json
├── vite.config.ts
├── src/
│   ├── App.tsx
│   ├── App.css
│   ├── index.css
│   ├── components/
│   └── styles/
└── README.md
```

**Structure Decision**: Keep the work inside the existing `src/frontend/blog-web`
app. Centralize Tailwind tokens and base rules in the stylesheet/theme layer,
and only add minimal folders if they make the styling foundation easier to
reuse in later UI slices.

## Complexity Tracking

No constitution violations are expected for this slice.
