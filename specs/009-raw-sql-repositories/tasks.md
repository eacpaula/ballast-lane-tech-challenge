# Tasks: Implement Raw SQL Repositories for Existing Application Use Cases

**Input**: Design documents from `/specs/009-raw-sql-repositories/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`

**Tests**: PostgreSQL-backed repository integration tests are mandatory for
this Infrastructure slice. Each repository story begins with failing
integration tests before raw SQL repository code is added.

**Organization**: Tasks are grouped by infrastructure user story so each
repository area can be implemented and validated independently against the real
PostgreSQL environment.

## Phase 1: Setup

**Purpose**: Prepare the Infrastructure test project, dependencies, and local
configuration entry points used by all repository stories.

- [ ] T001 Update Infrastructure package dependencies for PostgreSQL and configuration support in `src/backend/BlogPlatform.Infrastructure/BlogPlatform.Infrastructure.csproj`
- [ ] T002 Update Infrastructure test project dependencies and references for PostgreSQL-backed integration tests in `tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj`
- [ ] T003 Create the Infrastructure test support folder structure and placeholder notes in `tests/backend/BlogPlatform.Infrastructure.Tests/TestSupport/.gitkeep` and `tests/backend/BlogPlatform.Infrastructure.Tests/README.md`
- [ ] T004 Document repository test connection variables and local execution prerequisites in `docs/Database-Implementation-Strategy.md`

---

## Phase 2: Foundational Infrastructure

**Purpose**: Add shared test-database setup and connection support that blocks
all repository stories.

- [ ] T005 Create repository test database settings loading in `tests/backend/BlogPlatform.Infrastructure.Tests/TestSupport/PostgreSqlTestSettings.cs`
- [ ] T006 [P] Create the deterministic repository test reset workflow in `tests/backend/BlogPlatform.Infrastructure.Tests/TestSupport/RepositoryIntegrationTestDatabase.cs`
- [ ] T007 [P] Create a shared Infrastructure integration test fixture in `tests/backend/BlogPlatform.Infrastructure.Tests/TestSupport/PostgreSqlRepositoryTestFixture.cs`
- [ ] T008 Create the Infrastructure PostgreSQL connection settings type in `src/backend/BlogPlatform.Infrastructure/Configuration/PostgreSqlConnectionSettings.cs`
- [ ] T009 Create the Infrastructure database connection factory in `src/backend/BlogPlatform.Infrastructure/Data/NpgsqlConnectionFactory.cs`
- [ ] T010 Document the repeatable repository test run and reset workflow in `specs/009-raw-sql-repositories/quickstart.md`

**Checkpoint**: The repository test project can target the local PostgreSQL
environment with a deterministic reset strategy and minimal Infrastructure
connection support.

---

## Phase 3: User Story 1 - Repository test harness persists and retrieves users (Priority: P1) 🎯 MVP

**Goal**: Prove that PostgreSQL-backed user persistence supports registration
and login storage needs through `IUserRepository`.

**Independent Test**: `dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj --filter UserRepository` passes against the local PostgreSQL environment and verifies create plus lookup-by-email behavior.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T011 [US1] Add failing integration tests for creating a user and returning the stored identity in `tests/backend/BlogPlatform.Infrastructure.Tests/Users/UserRepositoryCreateTests.cs`
- [ ] T012 [P] [US1] Add failing integration tests for finding a user by email and retrieving the stored password hash in `tests/backend/BlogPlatform.Infrastructure.Tests/Users/UserRepositoryLookupTests.cs`
- [ ] T013 [P] [US1] Add failing integration tests for surfacing duplicate-email persistence conflicts in `tests/backend/BlogPlatform.Infrastructure.Tests/Users/UserRepositoryDuplicateEmailTests.cs`

### Implementation for User Story 1

- [ ] T014 [US1] Implement the raw SQL user repository in `src/backend/BlogPlatform.Infrastructure/Users/PostgreSqlUserRepository.cs`
- [ ] T015 [US1] Re-run the user repository integration tests and fix SQL mapping issues in `src/backend/BlogPlatform.Infrastructure/Users/PostgreSqlUserRepository.cs`

**Checkpoint**: User persistence is proven against PostgreSQL and can support
registration and login storage needs without moving business validation into
Infrastructure.

---

## Phase 4: User Story 2 - Repository test harness persists and reads posts (Priority: P2)

**Goal**: Prove that PostgreSQL-backed post persistence supports post create,
edit, remove, and public-read repository behavior through `IPostRepository`.

**Independent Test**: `dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj --filter PostRepository` passes against the local PostgreSQL environment and verifies mutation plus public-read behavior.

### Tests for User Story 2 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T016 [US2] Add failing integration tests for creating a post and finding it by id in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryCreateAndGetTests.cs`
- [ ] T017 [P] [US2] Add failing integration tests for updating an existing post in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryUpdateTests.cs`
- [ ] T018 [P] [US2] Add failing integration tests for removing a post through the current abstraction contract in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryDeleteTests.cs`
- [ ] T019 [P] [US2] Add failing integration tests for listing only public and available posts in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryPublicListTests.cs`
- [ ] T020 [P] [US2] Add failing integration tests for returning public post details only when the post is public and available in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryPublicDetailTests.cs`

### Implementation for User Story 2

- [ ] T021 [US2] Implement the raw SQL post repository in `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
- [ ] T022 [US2] Re-run the post repository integration tests and fix SQL mapping or filtering issues in `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`

**Checkpoint**: Post persistence is proven against PostgreSQL for mutation and
public-read use cases with public and availability filtering kept explicit in
repository queries only.

---

## Phase 5: User Story 3 - Repository test harness persists and manages categories (Priority: P3)

**Goal**: Prove that PostgreSQL-backed category persistence supports the
administrator category-management repository behavior through
`ICategoryRepository`.

**Independent Test**: `dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj --filter CategoryRepository` passes against the local PostgreSQL environment and verifies create, update, deactivate, duplicate-title, and lookup behavior.

### Tests for User Story 3 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T023 [US3] Add failing integration tests for creating a category and finding it by id in `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/CategoryRepositoryCreateAndGetTests.cs`
- [ ] T024 [P] [US3] Add failing integration tests for updating a category in `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/CategoryRepositoryUpdateTests.cs`
- [ ] T025 [P] [US3] Add failing integration tests for deactivating a category and reflecting availability state in `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/CategoryRepositoryDeactivateTests.cs`
- [ ] T026 [P] [US3] Add failing integration tests for duplicate-title checks in `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/CategoryRepositoryTitleExistsTests.cs`

### Implementation for User Story 3

- [ ] T027 [US3] Implement the raw SQL category repository in `src/backend/BlogPlatform.Infrastructure/Categories/PostgreSqlCategoryRepository.cs`
- [ ] T028 [US3] Re-run the category repository integration tests and fix SQL mapping or duplicate-check behavior in `src/backend/BlogPlatform.Infrastructure/Categories/PostgreSqlCategoryRepository.cs`

**Checkpoint**: Category persistence is proven against PostgreSQL for the
current category-management use cases without moving admin authorization or
duplicate-title business decisions into Infrastructure.

---

## Phase 6: User Story 4 - Repository test harness persists post reactions (Priority: P4)

**Goal**: Prove that PostgreSQL-backed reaction persistence supports storing
like and dislike reactions for user or visitor actors through
`IPostReactionRepository`.

**Independent Test**: `dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj --filter PostReactionRepository` passes against the local PostgreSQL environment and verifies reaction persistence for both actor forms.

### Tests for User Story 4 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T029 [US4] Add failing integration tests for storing like and dislike reactions in `tests/backend/BlogPlatform.Infrastructure.Tests/Reactions/PostReactionRepositoryCreateTests.cs`
- [ ] T030 [P] [US4] Add failing integration tests for associating reactions with either a user identifier or visitor identifier in `tests/backend/BlogPlatform.Infrastructure.Tests/Reactions/PostReactionRepositoryActorTests.cs`

### Implementation for User Story 4

- [ ] T031 [US4] Implement the raw SQL post reaction repository in `src/backend/BlogPlatform.Infrastructure/Reactions/PostgreSqlPostReactionRepository.cs`
- [ ] T032 [US4] Re-run the post reaction repository integration tests and fix SQL mapping issues in `src/backend/BlogPlatform.Infrastructure/Reactions/PostgreSqlPostReactionRepository.cs`

**Checkpoint**: Post reaction persistence is proven against PostgreSQL for the
existing like/dislike use case with actor association handled through stored
user or visitor identifiers.

---

## Final Phase: Polish & Cross-Cutting

**Purpose**: Finalize optional composition support, validation, and repository
documentation without expanding scope.

- [ ] T033 [P] Add minimal Infrastructure dependency registration only if it reduces repository composition friction in `src/backend/BlogPlatform.Infrastructure/DependencyInjection.cs`
- [ ] T034 [P] Update the repository implementation workflow and environment notes in `docs/Database-Implementation-Strategy.md` and `specs/009-raw-sql-repositories/quickstart.md`
- [ ] T035 Re-run the full Infrastructure and Application backend test suites through `tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj` and `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`
- [ ] T036 Review the implemented Infrastructure slice and simplify any unnecessary SQL helpers in `src/backend/BlogPlatform.Infrastructure`
- [ ] T037 Confirm that no standalone tag repository, API wiring, or other out-of-scope features were introduced in `specs/009-raw-sql-repositories/tasks.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 starts first.
- Phase 2 depends on Phase 1 and blocks all repository story work.
- Phase 3 depends on Phase 2.
- Phase 4 depends on Phase 2 and can begin after `US1` if shared user test data
  assumptions are needed by post tests.
- Phase 5 depends on Phase 2 and can proceed after `US1` because category tests
  rely on the shared repository test harness but not on post implementation.
- Phase 6 depends on Phases 2 and 4 because reaction tests require stable post
  persistence assumptions for referenced posts.
- Final Phase depends on the selected user stories being complete.

### User Story Dependencies

- `US1` is the MVP slice and should be completed first.
- `US2` depends on the shared repository test harness and benefits from the
  stored-user test support added for `US1`.
- `US3` depends only on the shared repository test harness and can run after
  Phase 2 if capacity allows.
- `US4` depends on the shared repository test harness and on stable post
  persistence coverage because reactions reference posts.

### Within Each User Story

- Repository integration tests MUST be written and fail before repository
  implementation.
- Repository classes SHOULD be added only after the relevant test files exist.
- Re-run and stabilize the story-level test filter before moving to the next
  repository area.
- Business validation must remain in Application even when a repository test
  exposes a persistence constraint or conflict.

### Parallel Opportunities

- `T006` and `T007` can run in parallel after `T005`.
- `T011`-`T013` can run in parallel once the shared test fixture exists.
- `T017`-`T020` can run in parallel after `T016`.
- `T024`-`T026` can run in parallel after `T023`.
- `T029` and `T030` can run in parallel.
- `T033` and `T034` can run in parallel after repository implementations are
  stable.

---

## Implementation Strategy

### MVP First

1. Complete Setup.
2. Complete Foundational Infrastructure.
3. Deliver `US1` with real PostgreSQL integration tests first.
4. Validate `US1` independently before expanding into posts, categories, and
   reactions.

### Incremental Delivery

1. Establish the shared PostgreSQL-backed test harness and reset workflow.
2. Implement the user repository and prove it with passing integration tests.
3. Add post persistence coverage and implementation next because it supports
   the broadest existing use-case surface.
4. Add category persistence after the shared Infrastructure pattern is stable.
5. Add reaction persistence last because it depends on stable post references.
6. Finish with documentation, optional DI, and full backend regression
   validation.

## Notes

- `[P]` tasks indicate parallelizable work across different files.
- `US1` through `US4` are derived from the repository areas in scope because
  this Infrastructure spec does not define product-facing user stories.
- No standalone tag repository task is included because there is no current
  Application abstraction requiring it.
