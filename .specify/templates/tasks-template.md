---

description: "Task list template for feature implementation"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Backend tests are mandatory. For each backend story, create the
relevant unit, API integration, and repository tests before production code.
When frontend work is in scope, include targeted UI and accessibility checks
only after the supporting backend flow and contracts are stable.

**Organization**: Tasks are grouped by user story to enable independent
implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `backend/src/Domain`, `backend/src/Application`,
  `backend/src/Infrastructure`, `backend/src/Api`
- **Backend tests**: `backend/tests/Domain.Tests`,
  `backend/tests/Application.Tests`, `backend/tests/Api.IntegrationTests`,
  `backend/tests/Infrastructure.Tests`
- **Frontend**: `frontend/src/`
- **Frontend theme/config**: `frontend/tailwind.config.*`,
  `frontend/src/styles/`

<!--
  ============================================================================
  Replace the sample tasks below with concrete tasks derived from the feature
  spec, plan, data model, and contracts. The generated task list must preserve
  test-first sequencing and backend-first delivery order.
  ============================================================================
-->

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project setup and baseline tooling needed by all stories

- [ ] T001 Create or update solution and project structure per implementation
      plan
- [ ] T002 Configure backend and frontend dependencies required by the plan
- [ ] T003 [P] Configure formatting, linting, and test commands

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Core backend capabilities that MUST be complete before story-level
implementation

**CRITICAL**: No frontend feature work may begin until this phase is complete

- [ ] T004 Define database schema or migration updates for shared entities
- [ ] T005 [P] Configure JWT authentication and authorization policies
- [ ] T006 [P] Configure API middleware for ProblemDetails-style error handling
- [ ] T007 Create shared Domain/Application abstractions and DTO boundaries
- [ ] T008 [P] Establish repository or gateway contracts and infrastructure
      wiring
- [ ] T009 Configure backend test infrastructure, including PostgreSQL-backed
      repository tests where needed

**Checkpoint**: Backend foundation is ready; user story work can now begin

---

## Phase 3: User Story 1 - [Title] (Priority: P1) 🎯 MVP

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T010 [P] [US1] Add Domain/Application test in
      `backend/tests/[Project].Tests/[file].cs`
- [ ] T011 [P] [US1] Add API integration test in
      `backend/tests/Api.IntegrationTests/[file].cs`
- [ ] T012 [P] [US1] Add repository test in
      `backend/tests/Infrastructure.Tests/[file].cs` if persistence behavior is
      part of the story

### Backend Implementation for User Story 1

- [ ] T013 [P] [US1] Implement Domain/Application logic in
      `backend/src/[layer]/[file].cs`
- [ ] T014 [P] [US1] Implement parameterized SQL repository changes in
      `backend/src/Infrastructure/[file].cs`
- [ ] T015 [US1] Implement API endpoint and DTO updates in
      `backend/src/Api/[file].cs`
- [ ] T016 [US1] Re-run tests and confirm ProblemDetails-style failure behavior

### Frontend for User Story 1

- [ ] T017 [US1] Implement UI integration in `frontend/src/[path]` after T010
      through T016 are passing
- [ ] T018 [US1] Update centralized Tailwind theme tokens or shared component
      primitives in `frontend/tailwind.config.*` or `frontend/src/styles/[file]`
      instead of introducing arbitrary one-off styles
- [ ] T019 [US1] Verify keyboard access, focus states, semantic HTML, and
      readable contrast for the new UI path

**Checkpoint**: User Story 1 is functional and independently testable

---

## Phase 4: User Story 2 - [Title] (Priority: P2)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 2 (MANDATORY) ⚠️

- [ ] T020 [P] [US2] Add Domain/Application test in
      `backend/tests/[Project].Tests/[file].cs`
- [ ] T021 [P] [US2] Add API integration test in
      `backend/tests/Api.IntegrationTests/[file].cs`
- [ ] T022 [P] [US2] Add repository test in
      `backend/tests/Infrastructure.Tests/[file].cs` if applicable

### Backend Implementation for User Story 2

- [ ] T023 [P] [US2] Implement Domain/Application logic in
      `backend/src/[layer]/[file].cs`
- [ ] T024 [P] [US2] Implement parameterized SQL repository changes in
      `backend/src/Infrastructure/[file].cs`
- [ ] T025 [US2] Implement API endpoint and DTO updates in
      `backend/src/Api/[file].cs`
- [ ] T026 [US2] Re-run tests and confirm authorization and validation behavior

### Frontend for User Story 2

- [ ] T027 [US2] Implement UI integration in `frontend/src/[path]` after T020
      through T026 are passing
- [ ] T028 [US2] Update centralized Tailwind theme tokens or shared component
      primitives in `frontend/tailwind.config.*` or `frontend/src/styles/[file]`
      instead of introducing arbitrary one-off styles
- [ ] T029 [US2] Verify keyboard access, focus states, semantic HTML, and
      readable contrast for the new UI path

**Checkpoint**: User Stories 1 and 2 both work independently

---

## Phase 5: User Story 3 - [Title] (Priority: P3)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 3 (MANDATORY) ⚠️

- [ ] T030 [P] [US3] Add Domain/Application test in
      `backend/tests/[Project].Tests/[file].cs`
- [ ] T031 [P] [US3] Add API integration test in
      `backend/tests/Api.IntegrationTests/[file].cs`
- [ ] T032 [P] [US3] Add repository test in
      `backend/tests/Infrastructure.Tests/[file].cs` if applicable

### Backend Implementation for User Story 3

- [ ] T033 [P] [US3] Implement Domain/Application logic in
      `backend/src/[layer]/[file].cs`
- [ ] T034 [P] [US3] Implement parameterized SQL repository changes in
      `backend/src/Infrastructure/[file].cs`
- [ ] T035 [US3] Implement API endpoint and DTO updates in
      `backend/src/Api/[file].cs`
- [ ] T036 [US3] Re-run tests and confirm security and error behavior

### Frontend for User Story 3

- [ ] T037 [US3] Implement UI integration in `frontend/src/[path]` after T030
      through T036 are passing
- [ ] T038 [US3] Update centralized Tailwind theme tokens or shared component
      primitives in `frontend/tailwind.config.*` or `frontend/src/styles/[file]`
      instead of introducing arbitrary one-off styles
- [ ] T039 [US3] Verify keyboard access, focus states, semantic HTML, and
      readable contrast for the new UI path

**Checkpoint**: All planned user stories are independently functional

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories without expanding
scope

- [ ] T040 [P] Update documentation in `docs/` or feature quickstart files
- [ ] T041 Re-run the full backend and frontend test suite
- [ ] T042 Review generated code and simplify any unnecessary abstractions
- [ ] T043 Verify OpenAPI output and ProblemDetails-style responses still match
      implementation
- [ ] T044 Confirm no out-of-scope features were introduced

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup starts first
- Foundational Backend depends on Setup and blocks all story work
- For each user story: tests first, backend implementation second, frontend
  integration last
- Final Polish depends on the selected user stories being complete

### Within Each User Story

- Tests MUST be written and fail before backend implementation
- Domain/Application work precedes API wiring when business rules are involved
- Infrastructure SQL changes remain isolated to backend infrastructure code
- Frontend tasks wait for passing backend tests and stable contracts
- Frontend tasks MUST reference `DESIGN.md` through centralized Tailwind theme
  updates and reusable component patterns rather than arbitrary scattered styles

### Parallel Opportunities

- Setup tasks marked `[P]` can run in parallel
- Foundational backend tasks marked `[P]` can run in parallel
- Tests within a story marked `[P]` can run in parallel
- Backend tasks within a story marked `[P]` can run in parallel where file
  ownership does not conflict
- Different stories can proceed in parallel only after foundational backend work
  is complete

---

## Implementation Strategy

### MVP First

1. Complete Setup
2. Complete Foundational Backend
3. Deliver User Story 1 with tests first and backend first
4. Validate User Story 1 independently before expanding scope

### Incremental Delivery

1. Finish shared backend foundations
2. Add the next highest-priority story
3. Prove it with passing tests and stable API behavior
4. Add dependent frontend work with `DESIGN.md` and accessibility checks
5. Repeat without expanding beyond the approved MVP

## Notes

- `[P]` tasks indicate parallelizable work across different files
- Story labels maintain traceability to spec.md
- Every backend story must leave behind useful automated tests
- Avoid vague tasks, cross-story coupling, frontend-first sequencing, and
  scattered one-off UI styling
