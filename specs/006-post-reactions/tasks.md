---

description: "Task list for public visitor reacts to posts"

---

# Tasks: Public Visitor Reacts To Posts

**Input**: Design documents from `/specs/006-post-reactions/`

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
- **Feature docs**: `specs/006-post-reactions/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the minimal test and source structure for the post-reactions slice without expanding scope.

- [X] T001 Create reaction-domain test folder structure in `tests/backend/BlogPlatform.Domain.Tests/Reactions/`
- [X] T002 Create reaction-application test folder structure in `tests/backend/BlogPlatform.Application.Tests/Reactions/`
- [X] T003 Create reaction source folder structure in `src/backend/BlogPlatform.Domain/Reactions/` and `src/backend/BlogPlatform.Application/Posts/`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Identify the minimum shared post and reaction boundaries before story-level TDD work begins.

**CRITICAL**: No reaction behavior is implemented until the reusable Domain/Application boundaries are identified.

- [X] T004 Review public-post eligibility reuse in `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`
- [X] T005 Review post-loading reuse points in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [X] T006 Review reaction boundary decisions in `specs/006-post-reactions/research.md` and `specs/006-post-reactions/contracts/post-reactions-use-case.md`

**Checkpoint**: The project is ready for story-specific TDD work.

---

## Phase 3: User Story 1 - Public Visitor Reacts To A Public Post (Priority: P1) 🎯 MVP

**Goal**: Allow a visitor to submit a like or dislike for a public, available post while rejecting invalid reaction types, invalid actor identity, and posts that are missing, private, or unavailable.

**Independent Test**: The story is complete when Domain and Application tests pass for valid like and dislike reactions, invalid reaction rejection, invalid actor rejection, post-not-available rejection, actor association behavior, repository-backed persistence, and clear success/failure results.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T007 [P] [US1] Add failing Domain tests for valid and invalid reaction types in `tests/backend/BlogPlatform.Domain.Tests/Reactions/PostReactionTests.cs`
- [X] T008 [P] [US1] Add failing Domain tests for valid and invalid reaction actor identity in `tests/backend/BlogPlatform.Domain.Tests/Reactions/PostReactionTests.cs`
- [X] T009 [P] [US1] Add failing Application test for successful like reaction against a public, available post in `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerSuccessTests.cs`
- [X] T010 [P] [US1] Add failing Application test for successful dislike reaction against a public, available post in `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerSuccessTests.cs`
- [X] T011 [P] [US1] Add failing Application tests for invalid reaction type and invalid actor rejection in `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerValidationTests.cs`
- [X] T012 [P] [US1] Add failing Application tests for missing, non-public, and unavailable post rejection in `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerPostAvailabilityTests.cs`
- [X] T013 [P] [US1] Add failing Application assertion coverage for reaction persistence and actor association in `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerSuccessTests.cs`
- [X] T014 [P] [US1] Add failing Application assertion coverage for clear success and failure results in `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerSuccessTests.cs`, `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerValidationTests.cs`, and `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerPostAvailabilityTests.cs`

### Backend Implementation for User Story 1

- [X] T015 [P] [US1] Create reaction type Domain concept in `src/backend/BlogPlatform.Domain/Reactions/ReactionType.cs`
- [X] T016 [P] [US1] Create reaction actor Domain concept in `src/backend/BlogPlatform.Domain/Reactions/ReactionActor.cs`
- [X] T017 [P] [US1] Create post reaction Domain model in `src/backend/BlogPlatform.Domain/Reactions/PostReaction.cs`
- [X] T018 [P] [US1] Create reaction repository abstraction in `src/backend/BlogPlatform.Application/Abstractions/IPostReactionRepository.cs`
- [X] T019 [P] [US1] Create react-to-post command input model in `src/backend/BlogPlatform.Application/Posts/ReactToPostCommand.cs`
- [X] T020 [P] [US1] Create react-to-post result model in `src/backend/BlogPlatform.Application/Posts/ReactToPostResult.cs`
- [X] T021 [US1] Implement public reaction orchestration in `src/backend/BlogPlatform.Application/Posts/ReactToPostHandler.cs`
- [X] T022 [US1] Refactor reaction validation and result handling in `src/backend/BlogPlatform.Domain/Reactions/PostReaction.cs`, `src/backend/BlogPlatform.Application/Posts/ReactToPostHandler.cs`, and `src/backend/BlogPlatform.Application/Posts/ReactToPostResult.cs`
- [X] T023 [US1] Re-run focused TDD test suite for `tests/backend/BlogPlatform.Domain.Tests/Reactions/PostReactionTests.cs`, `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerSuccessTests.cs`, `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerValidationTests.cs`, and `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerPostAvailabilityTests.cs`

**Checkpoint**: User Story 1 is fully functional inside Domain and Application and can be demonstrated without API, Infrastructure, database, authentication changes, or frontend work.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Confirm the slice remains small, reviewable, and aligned with the plan.

- [X] T024 [P] Update feature notes if needed in `specs/006-post-reactions/quickstart.md`
- [X] T025 Run the backend core test sweep in `tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj` and `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`
- [X] T026 Review final reaction core scope in `specs/006-post-reactions/plan.md`

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

- Tests `T007` through `T014` MUST be written and failing before implementation tasks `T015` through `T022`.
- `T015`, `T016`, and `T017` provide the compile-minimum Domain concepts required for the handler workflow.
- `T018`, `T019`, and `T020` provide the compile-minimum Application types and abstraction required for `T021`.
- `T021` implements the minimum behavior needed to pass tests.
- `T022` performs cleanup only after the first green implementation exists.
- `T023` verifies the story independently before final polish.

### Parallel Opportunities

- `T007` and `T008` can run in parallel as separate red-test additions inside `tests/backend/BlogPlatform.Domain.Tests/Reactions/PostReactionTests.cs`.
- `T009` through `T014` can run in parallel as separate red-test additions across the Application reaction test files.
- `T015` through `T020` can run in parallel after the failing tests exist because they target separate production files.
- `T024`, `T025`, and `T026` can run in parallel after implementation is stable.

---

## Parallel Example: User Story 1

```bash
# Write failing Domain/Application tests in parallel:
Task: "Add failing Domain tests for valid and invalid reaction types in tests/backend/BlogPlatform.Domain.Tests/Reactions/PostReactionTests.cs"
Task: "Add failing Domain tests for valid and invalid reaction actor identity in tests/backend/BlogPlatform.Domain.Tests/Reactions/PostReactionTests.cs"
Task: "Add failing Application test for successful like reaction against a public, available post in tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerSuccessTests.cs"
Task: "Add failing Application tests for missing, non-public, and unavailable post rejection in tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerPostAvailabilityTests.cs"

# Add compile-minimum production types in parallel:
Task: "Create reaction type Domain concept in src/backend/BlogPlatform.Domain/Reactions/ReactionType.cs"
Task: "Create reaction actor Domain concept in src/backend/BlogPlatform.Domain/Reactions/ReactionActor.cs"
Task: "Create post reaction Domain model in src/backend/BlogPlatform.Domain/Reactions/PostReaction.cs"
Task: "Create reaction repository abstraction in src/backend/BlogPlatform.Application/Abstractions/IPostReactionRepository.cs"
Task: "Create react-to-post command input model in src/backend/BlogPlatform.Application/Posts/ReactToPostCommand.cs"
Task: "Create react-to-post result model in src/backend/BlogPlatform.Application/Posts/ReactToPostResult.cs"
```

---

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational Backend.
2. Write failing Domain/Application tests for the single post-reaction story.
3. Add only the minimum Domain concepts, Application types, and repository abstractions needed to compile.
4. Implement the reaction handler to pass tests.
5. Refactor result handling and invariant placement after the first green state is reached.
6. Stop after the core use case is green and independently testable.

### Incremental Delivery

1. First increment: Domain/Application public reaction behavior only.
2. Later increment: API wiring for anonymous and authenticated reaction HTTP flows.
3. Later increment: Infrastructure persistence with raw SQL and Npgsql.
4. Later increment: deduplication, spam controls, analytics, or history only if the feature scope expands.
5. Final increment: frontend integration only if the feature is expanded.

## Notes

- All tasks follow the required checklist format with IDs, labels, and file paths.
- The suggested MVP scope is the single `US1` phase only.
- API, Infrastructure, database, authentication changes, and frontend implementation are intentionally excluded from this task list.
