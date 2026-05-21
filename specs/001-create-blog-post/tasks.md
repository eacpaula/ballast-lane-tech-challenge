---

description: "Task list for authenticated user creates a blog post"
---

# Tasks: Create Blog Post

**Input**: Design documents from `/specs/001-create-blog-post/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Backend tests are mandatory. This feature uses a TDD-first approach with Domain and Application tests written and failing before production code.

**Organization**: Tasks are grouped by user story to preserve an independently testable MVP increment.

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

**Purpose**: Prepare the existing stub projects for the feature slice without expanding scope.

- [X] T001 Remove placeholder test files in `tests/backend/BlogPlatform.Domain.Tests/UnitTest1.cs` and `tests/backend/BlogPlatform.Application.Tests/UnitTest1.cs`
- [X] T002 Remove placeholder production files in `src/backend/BlogPlatform.Domain/Class1.cs` and `src/backend/BlogPlatform.Application/Class1.cs`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Establish the minimum shared contracts required before the user story can be implemented.

**CRITICAL**: No implementation of the create-post use case begins until these baseline contracts are defined.

- [X] T003 Create shared Application contract folder structure in `src/backend/BlogPlatform.Application/Posts/`
- [X] T004 Create shared Domain folder structure in `src/backend/BlogPlatform.Domain/Posts/`
- [X] T005 Create repository abstraction folder structure in `src/backend/BlogPlatform.Application/Abstractions/`

**Checkpoint**: The project is ready for story-specific red-green-refactor work.

---

## Phase 3: User Story 1 - Authenticated User Creates a Blog Post (Priority: P1) 🎯 MVP

**Goal**: Allow an authenticated user to create a blog post with required content and a valid category, while rejecting invalid input before any API or database work is introduced.

**Independent Test**: The story is complete when Domain and Application test projects pass for valid post creation, invalid title/content, and missing authenticated user behavior.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T006 [P] [US1] Add Domain tests for valid post creation and invalid title/content in `tests/backend/BlogPlatform.Domain.Tests/Posts/BlogPostTests.cs`
- [X] T007 [P] [US1] Add failing Application tests for valid post creation in `tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerSuccessTests.cs`
- [X] T008 [P] [US1] Add failing Application tests for invalid title/content in `tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerValidationTests.cs`
- [X] T009 [P] [US1] Add failing Application test for missing authenticated user in `tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerAuthenticationTests.cs`

### Backend Implementation for User Story 1

- [X] T010 [P] [US1] Create minimal post domain model in `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`
- [X] T011 [P] [US1] Create create-post command input model in `src/backend/BlogPlatform.Application/Posts/CreateBlogPostCommand.cs`
- [X] T012 [P] [US1] Create post repository abstraction in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [X] T013 [P] [US1] Create category repository abstraction in `src/backend/BlogPlatform.Application/Abstractions/ICategoryRepository.cs`
- [X] T014 [P] [US1] Create create-post result model in `src/backend/BlogPlatform.Application/Posts/CreateBlogPostResult.cs`
- [X] T015 [US1] Implement create-post use case orchestration in `src/backend/BlogPlatform.Application/Posts/CreateBlogPostHandler.cs`
- [X] T016 [US1] Refactor validation and result handling in `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`, `src/backend/BlogPlatform.Application/Posts/CreateBlogPostHandler.cs`, and `src/backend/BlogPlatform.Application/Posts/CreateBlogPostResult.cs`
- [X] T017 [US1] Re-run focused TDD test suite in `tests/backend/BlogPlatform.Domain.Tests/Posts/BlogPostTests.cs` and `tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandler*.cs`

**Checkpoint**: User Story 1 is fully functional within Domain and Application and can be demonstrated without API, Infrastructure, or frontend work.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Confirm the increment stays small, reviewable, and aligned with the plan.

- [X] T018 [P] Update feature notes if needed in `specs/001-create-blog-post/quickstart.md`
- [X] T019 Run the full backend core test sweep for `tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj` and `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`
- [X] T020 Review final code paths in `src/backend/BlogPlatform.Domain/Posts/` and `src/backend/BlogPlatform.Application/Posts/` to remove unnecessary abstractions and confirm no API/Infrastructure/frontend scope was introduced

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup must complete first to remove placeholder files.
- Foundational Backend depends on Setup.
- User Story 1 depends on Foundational Backend.
- Final Polish depends on User Story 1 completion.

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Phase 2 and is the full MVP scope for this feature.

### Within User Story 1

- Tests `T006` through `T009` MUST be written and failing before implementation tasks `T010` through `T016`.
- Domain model `T010` and command/repository contracts `T011` through `T014` provide the minimum compile surface for `T015`.
- `T016` refines validation/result handling only after the first green implementation exists.
- `T017` verifies the story independently before any further expansion.

### Parallel Opportunities

- `T006`, `T007`, `T008`, and `T009` can run in parallel because they target separate test files.
- `T010`, `T011`, `T012`, `T013`, and `T014` can run in parallel after the failing tests exist because they target separate production files.
- `T018`, `T019`, and `T020` can run in parallel after the implementation is stable.

---

## Parallel Example: User Story 1

```bash
# Write failing tests in parallel:
Task: "Add Domain tests for valid post creation and invalid title/content in tests/backend/BlogPlatform.Domain.Tests/Posts/BlogPostTests.cs"
Task: "Add failing Application tests for valid post creation in tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerSuccessTests.cs"
Task: "Add failing Application tests for invalid title/content in tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerValidationTests.cs"
Task: "Add failing Application test for missing authenticated user in tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerAuthenticationTests.cs"

# Add compile-minimum production types in parallel:
Task: "Create minimal post domain model in src/backend/BlogPlatform.Domain/Posts/BlogPost.cs"
Task: "Create create-post command input model in src/backend/BlogPlatform.Application/Posts/CreateBlogPostCommand.cs"
Task: "Create post repository abstraction in src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs"
Task: "Create category repository abstraction in src/backend/BlogPlatform.Application/Abstractions/ICategoryRepository.cs"
Task: "Create create-post result model in src/backend/BlogPlatform.Application/Posts/CreateBlogPostResult.cs"
```

---

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational Backend.
2. Write failing Domain/Application tests for the single user story.
3. Add only the minimum production types needed to compile and pass those tests.
4. Implement the handler and refactor validation/result handling.
5. Stop after the core use case is green and independently testable.

### Incremental Delivery

1. First increment: Domain/Application create-post behavior only.
2. Later increment: API wiring for unauthenticated rejection and response mapping.
3. Later increment: Infrastructure persistence with raw SQL and Npgsql.
4. Final increment: frontend integration if the feature is expanded.

## Notes

- All tasks follow the required checklist format with IDs, labels, and file paths.
- The suggested MVP scope is the single `US1` phase only.
- API, Infrastructure, database, and frontend work are intentionally excluded from this task list.
