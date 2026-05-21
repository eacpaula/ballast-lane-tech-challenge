---

description: "Task list for authenticated user removes only their own blog post"

---

# Tasks: Remove Owned Blog Post

**Input**: Design documents from `/specs/003-remove-owned-post/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Backend tests are mandatory. This feature follows a TDD-first workflow with Application unit tests written and failing before production code.

**Organization**: Tasks are grouped by user story to preserve a single, independently testable MVP increment.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel when the task touches a different file and does not depend on incomplete work
- **[Story]**: Maps work to the feature user story
- Every task includes the exact file path to change

## Path Conventions

- **Application**: `src/backend/BlogPlatform.Application/`
- **Application tests**: `tests/backend/BlogPlatform.Application.Tests/`
- **Feature docs**: `specs/003-remove-owned-post/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the minimal test and source structure for the remove-post slice without expanding scope.

- [ ] T001 Create remove-post test folder structure in `tests/backend/BlogPlatform.Application.Tests/Posts/`
- [ ] T002 Create remove-post source folder structure in `src/backend/BlogPlatform.Application/Posts/`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Establish the minimum existing types that the remove-post use case will reuse.

**CRITICAL**: No remove-post behavior is implemented until the reusable Application abstractions are identified.

- [ ] T003 Review read/delete reuse points in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [ ] T004 Review loaded post ownership data reuse in `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`

**Checkpoint**: The project is ready for story-specific TDD work.

---

## Phase 3: User Story 1 - Authenticated User Removes Only Their Own Blog Post (Priority: P1) 🎯 MVP

**Goal**: Allow an authenticated user to remove an existing post only when they own it, while rejecting missing authentication, missing posts, and non-owner attempts.

**Independent Test**: The story is complete when Application tests pass for successful owned-post removal, missing-post failure, non-owner failure, missing authenticated user failure, repository-backed deletion, and clear success/failure results.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T005 [P] [US1] Add failing Application test for successful owned-post removal in `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerSuccessTests.cs`
- [ ] T006 [P] [US1] Add failing Application test for missing post rejection in `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerMissingPostTests.cs`
- [ ] T007 [P] [US1] Add failing Application test for non-owner rejection in `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerOwnershipTests.cs`
- [ ] T008 [P] [US1] Add failing Application test for missing authenticated user in `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerAuthenticationTests.cs`
- [ ] T009 [P] [US1] Add failing Application assertion coverage for deletion occurring only after ownership validation in `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerOwnershipTests.cs`
- [ ] T010 [P] [US1] Add failing Application assertion coverage for clear success and failure results in `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerSuccessTests.cs`, `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerMissingPostTests.cs`, and `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerAuthenticationTests.cs`

### Backend Implementation for User Story 1

- [ ] T011 [P] [US1] Create remove-post command input model in `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostCommand.cs`
- [ ] T012 [P] [US1] Create remove-post result model in `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostResult.cs`
- [ ] T013 [P] [US1] Extend repository abstraction for load/delete behavior in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [ ] T014 [US1] Implement owned-post removal orchestration in `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostHandler.cs`
- [ ] T015 [US1] Refactor handler result shaping and deletion flow in `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostHandler.cs` and `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostResult.cs`
- [ ] T016 [US1] Re-run focused TDD test suite for `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandler*.cs`

**Checkpoint**: User Story 1 is fully functional within Application and can be demonstrated without API, Infrastructure, frontend, or database work.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Confirm the slice remains small, reviewable, and aligned with the plan.

- [ ] T017 [P] Update feature notes if needed in `specs/003-remove-owned-post/quickstart.md`
- [ ] T018 Run the backend Application test project in `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`
- [ ] T019 Review final remove-post code paths in `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostHandler.cs` and `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup must complete first.
- Foundational Backend depends on Setup.
- User Story 1 depends on Foundational Backend.
- Final Polish depends on User Story 1 completion.

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Phase 2 and is the full MVP scope for this feature.

### Within User Story 1

- Tests `T005` through `T010` MUST be written and failing before implementation tasks `T011` through `T015`.
- `T011`, `T012`, and `T013` provide the compile-minimum Application types and abstraction changes required for `T014`.
- `T014` implements the minimum behavior needed to pass tests.
- `T015` performs cleanup only after the first green implementation exists.
- `T016` verifies the story independently before any later expansion.

### Parallel Opportunities

- `T005` through `T008` can run in parallel because they target separate test files.
- `T011`, `T012`, and `T013` can run in parallel after the failing tests exist because they target separate production files.
- `T017`, `T018`, and `T019` can run in parallel after implementation is stable.

---

## Parallel Example: User Story 1

```bash
# Write failing tests in parallel:
Task: "Add failing Application test for successful owned-post removal in tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerSuccessTests.cs"
Task: "Add failing Application test for missing post rejection in tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerMissingPostTests.cs"
Task: "Add failing Application test for non-owner rejection in tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerOwnershipTests.cs"
Task: "Add failing Application test for missing authenticated user in tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerAuthenticationTests.cs"

# Add compile-minimum production types in parallel:
Task: "Create remove-post command input model in src/backend/BlogPlatform.Application/Posts/RemoveBlogPostCommand.cs"
Task: "Create remove-post result model in src/backend/BlogPlatform.Application/Posts/RemoveBlogPostResult.cs"
Task: "Extend repository abstraction for load/delete behavior in src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs"
```

---

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational Backend.
2. Write failing Application tests for the single remove-owned-post story.
3. Add only the minimum production types and repository members needed to compile.
4. Implement the handler to pass tests.
5. Refactor result handling once the first green state is reached.
6. Stop after the core use case is green and independently testable.

### Incremental Delivery

1. First increment: Application remove-owned-post behavior only.
2. Later increment: API wiring for HTTP rejection and response mapping.
3. Later increment: Infrastructure persistence with raw SQL and Npgsql, defaulting to hard delete unless requirements change.
4. Final increment: frontend integration only if the feature is expanded.

## Notes

- All tasks follow the required checklist format with IDs, labels, and file paths.
- The suggested MVP scope is the single `US1` phase only.
- Domain, API, Infrastructure, database, and frontend implementation are intentionally excluded unless later work proves they are needed.
