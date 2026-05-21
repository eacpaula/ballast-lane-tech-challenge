---

description: "Task list for Tailwind frontend foundation implementation"
---

# Tasks: Tailwind Frontend Foundation

**Input**: Design documents from `/specs/012-tailwind-foundation/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Use frontend validation commands and manual shell verification for
this infrastructure slice. No backend tests or API integration tests are
required.

**Organization**: Tasks are grouped by user story to enable independent
implementation and verification of the frontend foundation slice.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1)
- Include exact file paths in descriptions

## Path Conventions

- **Frontend app**: `src/frontend/blog-web/`
- **Frontend source**: `src/frontend/blog-web/src/`
- **Frontend theme/config**: `src/frontend/blog-web/tailwind.config.*`,
  `src/frontend/blog-web/src/styles/`
- **Feature docs**: `specs/012-tailwind-foundation/`

## Phase 1: Setup

**Purpose**: Confirm the current frontend shape and add the minimum dependencies
needed for Tailwind foundation work.

- [X] T001 Review `DESIGN.md` and record the approved color, typography,
      spacing, radius, shadow, and layout guidance in
      `specs/012-tailwind-foundation/research.md`
- [X] T002 Inspect the existing frontend starter structure in
      `src/frontend/blog-web/package.json`, `src/frontend/blog-web/src/App.tsx`,
      and `src/frontend/blog-web/src/index.css` before replacing the demo
      styling
- [X] T003 Install TailwindCSS and the Vite integration dependency by updating
      `src/frontend/blog-web/package.json` and
      `src/frontend/blog-web/package-lock.json`

---

## Phase 2: Foundational Frontend

**Purpose**: Create the shared Tailwind and stylesheet foundation that all UI
work will inherit.

**CRITICAL**: User-story UI validation work depends on this phase being
complete.

- [X] T004 Configure Tailwind for the Vite app in
      `src/frontend/blog-web/vite.config.ts`
- [X] T005 Create the Tailwind configuration file in
      `src/frontend/blog-web/tailwind.config.ts`
- [X] T006 Configure Tailwind content paths for the existing app and planned
      shared folders in `src/frontend/blog-web/tailwind.config.ts`
- [X] T007 Create the shared frontend styles folder in
      `src/frontend/blog-web/src/styles/`
- [X] T008 Map color, typography, spacing, radius, shadow, and layout tokens
      from `DESIGN.md` into `src/frontend/blog-web/tailwind.config.ts`
- [X] T009 Replace the starter global stylesheet with a clean Tailwind entry
      point and base layer in `src/frontend/blog-web/src/index.css`
- [X] T010 [P] Remove or simplify redundant starter demo styles in
      `src/frontend/blog-web/src/App.css` after the global Tailwind layer is in
      place
- [X] T011 [P] Define reusable layout and surface utility patterns in
      `src/frontend/blog-web/src/styles/` if the tokenized foundation needs a
      small shared helper layer

**Checkpoint**: The Tailwind configuration and global style foundation are ready
for UI proof work.

---

## Phase 3: User Story 1 - Frontend Styling Foundation Works (Priority: P1) 🎯 MVP

**Goal**: Prove that the frontend can render a minimal shell using centralized
Tailwind styling derived from `DESIGN.md`.

**Independent Test**: Start the frontend locally, confirm the starter Vite
screen has been replaced with a minimal shell that shows the approved type
hierarchy, surface styling, action styling, tag/meta styling, responsive
container behavior, and visible focus treatment, then confirm the production
build succeeds.

### Implementation for User Story 1

- [X] T012 [US1] Replace the starter Vite demo markup with a minimal validation
      shell in `src/frontend/blog-web/src/App.tsx`
- [X] T013 [US1] Wire the validation shell to the Tailwind-driven global
      stylesheet through `src/frontend/blog-web/src/main.tsx` and
      `src/frontend/blog-web/src/App.tsx`
- [X] T014 [US1] Apply the approved headline, body, and metadata hierarchy in
      `src/frontend/blog-web/src/App.tsx` using centralized Tailwind tokens
- [X] T015 [US1] Add a centered responsive container and section spacing
      foundation in `src/frontend/blog-web/src/App.tsx` and
      `src/frontend/blog-web/src/index.css`
- [X] T016 [US1] Add one surface/card example and one action/control example in
      `src/frontend/blog-web/src/App.tsx` to prove surface, border, radius, and
      shadow tokens are active
- [X] T017 [US1] Add one tag or metadata treatment in
      `src/frontend/blog-web/src/App.tsx` to prove mono/label styling is
      available for future technical UI
- [X] T018 [US1] Add accessibility-friendly defaults for focus states, readable
      typography, and semantic structure in
      `src/frontend/blog-web/src/index.css` and
      `src/frontend/blog-web/src/App.tsx`
- [X] T019 [US1] Validate the minimal shell at mobile and desktop widths using
      the criteria in `specs/012-tailwind-foundation/quickstart.md`
- [X] T020 [US1] Run the frontend build validation from
      `src/frontend/blog-web/package.json` and confirm the Tailwind foundation
      compiles successfully

**Checkpoint**: The Tailwind foundation is active and independently verifiable.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Finish documentation and verify the slice remains small and
maintainable.

- [X] T021 [P] Update frontend run/build instructions and Tailwind foundation
      notes in `src/frontend/blog-web/README.md`
- [X] T022 [P] Update root-level frontend execution guidance in `README.md` if
      the current instructions do not mention the frontend app clearly
- [X] T023 Re-run the frontend validation flow in
      `specs/012-tailwind-foundation/quickstart.md` and record any follow-up
      notes in `specs/012-tailwind-foundation/quickstart.md`
- [X] T024 Review `src/frontend/blog-web/` for unnecessary starter assets,
      styles, or abstractions and remove anything no longer needed
- [X] T025 Confirm the implementation did not introduce full pages, backend API
      integration, heavy UI frameworks, copied Ballast Lane branding, or other
      out-of-scope work

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup starts first
- Foundational Frontend depends on Setup
- User Story 1 depends on the Tailwind foundation being complete
- Final Polish depends on User Story 1 being complete

### Within User Story 1

- Tailwind configuration and global styles must exist before the shell is
  updated
- The minimal shell should be styled before responsive and accessibility checks
- Build validation should run after the shell and global styles are complete

### Parallel Opportunities

- T010 and T011 can run in parallel after T009
- T021 and T022 can run in parallel after T020

---

## Implementation Strategy

### MVP First

1. Install Tailwind dependencies
2. Configure Tailwind and the global stylesheet foundation
3. Replace the starter demo with the minimal validation shell
4. Verify responsive behavior, accessibility defaults, and build success

### Incremental Delivery

1. Finish setup and shared Tailwind configuration
2. Deliver the smallest validation shell that proves the tokens work
3. Document how to run and verify the frontend
4. Stop before expanding into real pages or backend integration

## Notes

- Tasks are intentionally frontend-only for this slice
- Shared design tokens must remain centralized rather than scattered through
  arbitrary one-off classes or CSS values
- Keep the implementation interview-friendly: clean, small, and easy to explain
