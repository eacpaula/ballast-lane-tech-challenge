---

description: "Task list for public visitor reads posts"

---

# Tasks: Public Visitor Reads Posts

**Input**: Design documents from `/specs/005-read-public-posts/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Backend tests are mandatory. This feature follows a TDD-first workflow with Application unit tests written and failing before production code.

**Organization**: Tasks are grouped by user story so public listing can be delivered as the MVP and public post detail reads can be added as a second independently testable increment.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel when the task touches a different file and does not depend on incomplete work
- **[Story]**: Maps work to the feature user story
- Every task includes the exact file path to change

## Path Conventions

- **Application**: `src/backend/BlogPlatform.Application/`
- **Application tests**: `tests/backend/BlogPlatform.Application.Tests/`
- **Feature docs**: `specs/005-read-public-posts/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the minimal test and source structure for the public-read slice without expanding scope.

- [X] T001 Create public-read application test folder structure in `tests/backend/BlogPlatform.Application.Tests/Posts/`
- [X] T002 Create public-read source folder structure in `src/backend/BlogPlatform.Application/Posts/`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Identify the minimum shared read abstractions and reuse points before story-level TDD work begins.

**CRITICAL**: No public-read behavior is implemented until the existing post abstractions and contract boundaries are reviewed.

- [X] T003 Review shared repository extension points in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [X] T004 Review whether public visibility and availability state can be reused from `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`
- [X] T005 Review public-read contract expectations in `specs/005-read-public-posts/contracts/public-read-posts-use-cases.md`

**Checkpoint**: The project is ready for story-specific TDD work.

---

## Phase 3: User Story 1 - Public Visitor Lists Available Public Posts (Priority: P1) 🎯 MVP

**Goal**: Allow an anonymous visitor to list posts without authentication while returning only posts that are public and available through an Application-layer repository abstraction.

**Independent Test**: The story is complete when Application tests pass for public listing without authentication context, filtering out non-public posts, filtering out unavailable posts, and repository-backed retrieval of public list items.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T006 [P] [US1] Add failing Application test for public listing without authentication context in `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs`
- [X] T007 [P] [US1] Add failing Application test for excluding non-public posts from the listing in `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs`
- [X] T008 [P] [US1] Add failing Application test for excluding unavailable posts from the listing in `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs`
- [X] T009 [P] [US1] Add failing Application assertion coverage for retrieving list results through the repository abstraction in `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs`

### Backend Implementation for User Story 1

- [X] T010 [P] [US1] Create public-post list item result model in `src/backend/BlogPlatform.Application/Posts/PublicPostListItem.cs`
- [X] T011 [P] [US1] Extend the post repository abstraction for public listing reads in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [X] T012 [US1] Implement public post listing orchestration in `src/backend/BlogPlatform.Application/Posts/ListPublicPostsHandler.cs`
- [X] T013 [US1] Refactor public listing filtering and result shaping in `src/backend/BlogPlatform.Application/Posts/ListPublicPostsHandler.cs` and `src/backend/BlogPlatform.Application/Posts/PublicPostListItem.cs`
- [X] T014 [US1] Re-run focused TDD test suite for `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs`

**Checkpoint**: User Story 1 is functional within Application and can be demonstrated without API, Infrastructure, database, or frontend work.

---

## Phase 4: User Story 2 - Public Visitor Reads A Single Public Post (Priority: P2)

**Goal**: Allow an anonymous visitor to read one post by id only when the post exists and is both public and available, returning a clear not-found or not-available outcome otherwise.

**Independent Test**: The story is complete when Application tests pass for successful public detail reads, missing-post rejection, non-public rejection, unavailable-post rejection, and repository-backed retrieval for the public detail use case.

### Tests for User Story 2 (MANDATORY) ⚠️

- [X] T015 [P] [US2] Add failing Application test for reading a public and available post by id without authentication context in `tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs`
- [X] T016 [P] [US2] Add failing Application test for returning a clear not-found or not-available result when the post does not exist in `tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs`
- [X] T017 [P] [US2] Add failing Application test for returning a clear not-found or not-available result when the post is not public in `tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs`
- [X] T018 [P] [US2] Add failing Application test for returning a clear not-found or not-available result when the post is unavailable in `tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs`
- [X] T019 [P] [US2] Add failing Application assertion coverage for retrieving the post detail through the repository abstraction in `tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs`

### Backend Implementation for User Story 2

- [X] T020 [P] [US2] Create public-post detail result model in `src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdResult.cs`
- [X] T021 [P] [US2] Extend the post repository abstraction for public detail reads in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [X] T022 [US2] Implement public post detail orchestration in `src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdHandler.cs`
- [X] T023 [US2] Refactor public detail result handling in `src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdHandler.cs` and `src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdResult.cs`
- [X] T024 [US2] Re-run focused TDD test suite for `tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs`

**Checkpoint**: User Stories 1 and 2 both work independently inside Application without API, Infrastructure, database, authentication, or frontend changes.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Confirm the slice remains small, reviewable, and aligned with the plan.

- [X] T025 [P] Update feature notes if needed in `specs/005-read-public-posts/quickstart.md`
- [X] T026 Run the backend Application test project in `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`
- [X] T027 Review final public-read scope in `specs/005-read-public-posts/plan.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup must complete first.
- Foundational Backend depends on Setup.
- User Story 1 depends on Foundational Backend.
- User Story 2 depends on User Story 1 because it reuses the same post repository abstraction and public read conventions.
- Final Polish depends on the selected user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Phase 2 and is the MVP scope for this feature.
- **User Story 2 (P2)**: Starts after User Story 1 and extends the public read slice with post detail behavior.

### Within User Story 1

- Tests `T006` through `T009` MUST be written and failing before implementation tasks `T010` through `T013`.
- `T010` and `T011` provide the compile-minimum Application types required for `T012`.
- `T012` implements the minimum behavior needed to pass tests.
- `T013` performs cleanup only after the first green implementation exists.
- `T014` verifies the story independently before public detail work begins.

### Within User Story 2

- Tests `T015` through `T019` MUST be written and failing before implementation tasks `T020` through `T023`.
- `T020` and `T021` provide the compile-minimum Application types required for `T022`.
- `T022` implements the minimum behavior needed to pass tests.
- `T023` performs cleanup only after the first green implementation exists.
- `T024` verifies the story independently before final polish.

### Parallel Opportunities

- `T006` through `T009` can run in parallel as separate red-test additions inside `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs`.
- `T010` and `T011` can run in parallel after the failing listing tests exist because they target separate production files.
- `T015` through `T019` can run in parallel as separate red-test additions inside `tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs`.
- `T020` and `T021` can run in parallel after the failing detail tests exist because they target separate production files.
- `T025`, `T026`, and `T027` can run in parallel after implementation is stable.

---

## Parallel Example: User Story 1

```bash
# Write failing public-list tests in parallel:
Task: "Add failing Application test for public listing without authentication context in tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs"
Task: "Add failing Application test for excluding non-public posts from the listing in tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs"
Task: "Add failing Application test for excluding unavailable posts from the listing in tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs"

# Add compile-minimum listing types in parallel:
Task: "Create public-post list item result model in src/backend/BlogPlatform.Application/Posts/PublicPostListItem.cs"
Task: "Extend the post repository abstraction for public listing reads in src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs"
```

---

## Parallel Example: User Story 2

```bash
# Write failing public-detail tests in parallel:
Task: "Add failing Application test for reading a public and available post by id without authentication context in tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs"
Task: "Add failing Application test for returning a clear not-found or not-available result when the post does not exist in tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs"
Task: "Add failing Application test for returning a clear not-found or not-available result when the post is not public in tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs"
Task: "Add failing Application test for returning a clear not-found or not-available result when the post is unavailable in tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs"

# Add compile-minimum detail types in parallel:
Task: "Create public-post detail result model in src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdResult.cs"
Task: "Extend the post repository abstraction for public detail reads in src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs"
```

---

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational Backend.
2. Write failing Application tests for public listing.
3. Add only the minimum listing types and repository members needed to compile.
4. Implement public listing to pass tests.
5. Validate User Story 1 independently before adding public detail reads.

### Incremental Delivery

1. First increment: Application public listing behavior only.
2. Second increment: Application public detail behavior only.
3. Later increment: API wiring for anonymous HTTP list and detail endpoints.
4. Later increment: Infrastructure raw-SQL implementations with Npgsql.
5. Final increment: frontend integration only if the feature is expanded.

## Notes

- All tasks follow the required checklist format with IDs, labels, and file paths.
- The suggested MVP scope is `US1` only.
- Domain changes remain optional and should only be introduced if the Application tests prove the current post model cannot express public read state cleanly.
- API, Infrastructure, database, authentication, and frontend implementation are intentionally excluded from this task list.
