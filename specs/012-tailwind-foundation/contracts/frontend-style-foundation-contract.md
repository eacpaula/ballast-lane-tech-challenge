# Frontend Style Foundation Contract

## Purpose

Define the observable contract for the Tailwind foundation slice so later
frontend tasks inherit a stable styling baseline.

## Contract

### 1. Frontend startup contract

- A developer can install frontend dependencies from the existing
  `src/frontend/blog-web` app.
- A developer can run the local development server and see the minimal shell
  without backend/API dependencies.
- A developer can run the production build successfully after the Tailwind
  foundation changes.

### 2. Styling foundation contract

- The app entry continues to load one global stylesheet.
- The global stylesheet contains the shared Tailwind-driven foundation layer.
- Shared design tokens are defined centrally rather than repeated as arbitrary
  one-off values throughout the shell.

### 3. Visual proof contract

- The app renders a minimal shell with:
  - a centered page frame
  - a clear heading hierarchy
  - readable supporting body copy
  - at least one surface/card treatment
  - at least one action/control treatment
  - at least one metadata/tag treatment
- The rendered shell reflects the approved design direction from `DESIGN.md`
  without copying Ballast Lane branding directly.

### 4. Accessibility contract

- The minimal shell uses semantic HTML structure.
- Keyboard users can see a visible focus state on interactive elements.
- Text and surfaces present readable contrast for the first-pass foundation.

### 5. Scope contract

- No backend API calls are introduced.
- No authentication screens, post pages, or admin workflows are introduced.
- No heavy UI framework or broad component library is introduced in this slice.
