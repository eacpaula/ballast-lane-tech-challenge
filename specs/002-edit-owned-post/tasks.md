---

description: "Task list for authenticated user edits only their own blog post"
---

# Tasks: Edit Owned Blog Post

**Input**: Design documents from `/specs/002-edit-owned-post/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Backend tests are mandatory. This feature follows a TDD-first workflow with Domain and Application unit tests written and failing before production code.

**Organization**: Tasks are grouped by user story to preserve a single, independently testable MVP increment.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel when the task touches a different file and does not depend on incomplete work
- **[Story]**: Maps work to the feature user story
- Every task includes the exact file path to change

## Path Conventions

- **Domain**: `src/backend/BlogPlatform.Domain/`
- **Application**: `src/backend/BlogPlatform.Application/`
- **Domain tests**: `tests/backend/BlogPlatform.Domain.Tests/`
- **Application tests**: `tests/backend/BlogPlatform.Application.Tests/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the minimal test and source structure for the edit-post slice without expanding scope.

- [X] T001 Create edit-post test folder structure in `tests/backend/BlogPlatform.Domain.Tests/Posts/` and `tests/backend/BlogPlatform.Application.Tests/Posts/`
- [X] T002 Create edit-post source folder structure in `src/backend/BlogPlatform.Application/Posts/`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Establish the minimum compile surface shared by the edit-post use case.

**CRITICAL**: No edit-post behavior is implemented until the baseline folders and target files are identified.

- [X] T003 Review and prepare reuse of `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs` for edit behavior
- [X] T004 Review and prepare reuse of `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs` for read/update behavior

**Checkpoint**: The project is ready for story-specific TDD work.

---

## Phase 3: User Story 1 - Authenticated User Edits Only Their Own Blog Post (Priority: P1) 🎯 MVP

**Goal**: Allow an authenticated user to update an existing post only when they own it, while rejecting missing post, non-owner, and invalid content cases.

**Independent Test**: The story is complete when Domain and Application tests pass for successful owned-post updates, missing-post failure, non-owner failure, missing authenticated user failure, and invalid title/content failure.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T005 [P] [US1] Add Domain tests for valid post updates and invalid title/content in `tests/backend/BlogPlatform.Domain.Tests/Posts/BlogPostEditTests.cs`
- [X] T006 [P] [US1] Add failing Application test for successful owned-post update in `tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerSuccessTests.cs`
- [X] T007 [P] [US1] Add failing Application test for missing post in `tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerMissingPostTests.cs`
- [X] T008 [P] [US1] Add failing Application test for non-owner edit rejection in `tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerOwnershipTests.cs`
- [X] T009 [P] [US1] Add failing Application test for missing authenticated user in `tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerAuthenticationTests.cs`
- [X] T010 [P] [US1] Add failing Application tests for invalid title/content in `tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerValidationTests.cs`

### Backend Implementation for User Story 1

- [X] T011 [P] [US1] Add minimal edit behavior to `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`
- [X] T012 [P] [US1] Create edit-post command input model in `src/backend/BlogPlatform.Application/Posts/EditBlogPostCommand.cs`
- [X] T013 [P] [US1] Create edit-post result model in `src/backend/BlogPlatform.Application/Posts/EditBlogPostResult.cs`
- [X] T014 [P] [US1] Extend repository abstraction for load/update behavior in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [X] T015 [US1] Implement owned-post edit orchestration in `src/backend/BlogPlatform.Application/Posts/EditBlogPostHandler.cs`
- [X] T016 [US1] Refactor ownership, validation, and result handling in `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`, `src/backend/BlogPlatform.Application/Posts/EditBlogPostHandler.cs`, and `src/backend/BlogPlatform.Application/Posts/EditBlogPostResult.cs`
- [X] T017 [US1] Re-run focused TDD test suite in `tests/backend/BlogPlatform.Domain.Tests/Posts/BlogPostEditTests.cs` and `tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandler*.cs`

**Checkpoint**: User Story 1 is fully functional within Domain and Application and can be demonstrated without API, Infrastructure, frontend, or database work.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Confirm the slice remains small, reviewable, and aligned with the plan.

- [X] T018 [P] Update feature notes if needed in `specs/002-edit-owned-post/quickstart.md`
- [X] T019 Run the full backend core test sweep for `tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj` and `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`
- [X] T020 Review final code paths in `src/backend/BlogPlatform.Domain/Posts/`, `src/backend/BlogPlatform.Application/Posts/`, and `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs` to confirm no API/Infrastructure/frontend scope was introduced

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

- Tests `T005` through `T010` MUST be written and failing before implementation tasks `T011` through `T016`.
- `T011` and `T014` define the minimal existing-type changes needed for the edit flow.
- `T012` and `T013` provide the compile-minimum Application types for `T015`.
- `T015` implements the minimum behavior needed to pass tests.
- `T016` performs cleanup only after the first green implementation exists.
- `T017` verifies the story independently before any later expansion.

### Parallel Opportunities

- `T005` through `T010` can run in parallel because they target separate test files.
- `T011`, `T012`, `T013`, and `T014` can run in parallel after the failing tests exist because they target separate production files.
- `T018`, `T019`, and `T020` can run in parallel after implementation is stable.

---

## Parallel Example: User Story 1

```bash
# Write failing tests in parallel:
Task: "Add Domain tests for valid post updates and invalid title/content in tests/backend/BlogPlatform.Domain.Tests/Posts/BlogPostEditTests.cs"
Task: "Add failing Application test for successful owned-post update in tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerSuccessTests.cs"
Task: "Add failing Application test for missing post in tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerMissingPostTests.cs"
Task: "Add failing Application test for non-owner edit rejection in tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerOwnershipTests.cs"
Task: "Add failing Application test for missing authenticated user in tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerAuthenticationTests.cs"
Task: "Add failing Application tests for invalid title/content in tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerValidationTests.cs"

# Add compile-minimum production types in parallel:
Task: "Add minimal edit behavior to src/backend/BlogPlatform.Domain/Posts/BlogPost.cs"
Task: "Create edit-post command input model in src/backend/BlogPlatform.Application/Posts/EditBlogPostCommand.cs"
Task: "Create edit-post result model in src/backend/BlogPlatform.Application/Posts/EditBlogPostResult.cs"
Task: "Extend repository abstraction for load/update behavior in src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs"
```

---

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational Backend.
2. Write failing Domain/Application tests for the single edit-owned-post story.
3. Add only the minimum production types and repository members needed to compile.
4. Implement the handler and domain update behavior to pass tests.
5. Refactor validation/result handling once the first green state is reached.
6. Stop after the core use case is green and independently testable.

### Incremental Delivery

1. First increment: Domain/Application edit-owned-post behavior only.
2. Later increment: API wiring for HTTP rejection and response mapping.
3. Later increment: Infrastructure persistence with raw SQL and Npgsql.
4. Final increment: frontend integration only if the feature is expanded.

## Notes

- All tasks follow the required checklist format with IDs, labels, and file paths.
- The suggested MVP scope is the single `US1` phase only.
- API, Infrastructure, database, and frontend work are intentionally excluded from this task list.
