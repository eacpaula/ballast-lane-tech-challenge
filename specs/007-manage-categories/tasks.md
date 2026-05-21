# Tasks: Administrator manages post categories

**Input**: Design documents from `/specs/007-manage-categories/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`

## Phase 1: Setup

**Purpose**: Create the minimal test and source structure for the category-management slice.

- [X] T001 Create category Domain test file scaffold in `tests/backend/BlogPlatform.Domain.Tests/Categories/PostCategoryTests.cs`
- [X] T002 Create category Application test file scaffold in `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs`, `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`, and `tests/backend/BlogPlatform.Application.Tests/Categories/DeactivatePostCategoryHandlerTests.cs`
- [X] T003 Create category source file scaffold in `src/backend/BlogPlatform.Domain/Categories/PostCategory.cs`, `src/backend/BlogPlatform.Application/Posts/CreatePostCategoryCommand.cs`, `src/backend/BlogPlatform.Application/Posts/UpdatePostCategoryCommand.cs`, `src/backend/BlogPlatform.Application/Posts/DeactivatePostCategoryCommand.cs`, `src/backend/BlogPlatform.Application/Posts/PostCategoryManagementResult.cs`, `src/backend/BlogPlatform.Application/Posts/CreatePostCategoryHandler.cs`, `src/backend/BlogPlatform.Application/Posts/UpdatePostCategoryHandler.cs`, and `src/backend/BlogPlatform.Application/Posts/DeactivatePostCategoryHandler.cs`

---

## Phase 2: Foundational Backend

**Purpose**: Establish the shared abstractions and Domain behavior used by all category-management stories.

- [X] T004 Add failing Domain tests for valid, empty, and invalid category titles in `tests/backend/BlogPlatform.Domain.Tests/Categories/PostCategoryTests.cs`
- [X] T005 [P] Add failing Domain tests for update and deactivate state transitions in `tests/backend/BlogPlatform.Domain.Tests/Categories/PostCategoryTests.cs`
- [X] T006 Implement the minimal `PostCategory` Domain model to satisfy title and availability rules in `src/backend/BlogPlatform.Domain/Categories/PostCategory.cs`
- [X] T007 Expand the category repository abstraction with create, load, duplicate-title, update, and deactivate operations in `src/backend/BlogPlatform.Application/Abstractions/ICategoryRepository.cs`

**Checkpoint**: Domain category rules and shared repository abstraction are ready for Application use cases.

---

## Phase 3: User Story 1 - Administrator creates a category (Priority: P1)

**Goal**: Allow an administrator to create a category with a valid, unique title through an Application-layer use case.

**Independent Test**: Category creation passes for an administrator with valid input, rejects non-admin actors, rejects invalid titles, rejects duplicate titles, and persists the new category through the repository abstraction.

- [X] T008 [US1] Add a failing Application test for successful admin category creation in `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs`
- [X] T009 [P] [US1] Add a failing Application test for non-admin category creation rejection in `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs`
- [X] T010 [P] [US1] Add a failing Application test for invalid category title rejection in `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs`
- [X] T011 [P] [US1] Add a failing Application test for duplicate category title rejection in `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs`
- [X] T012 [P] [US1] Add a failing Application test that verifies repository-backed category persistence on success in `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs`
- [X] T013 [US1] Add the compile-minimum create command and result types in `src/backend/BlogPlatform.Application/Posts/CreatePostCategoryCommand.cs` and `src/backend/BlogPlatform.Application/Posts/PostCategoryManagementResult.cs`
- [X] T014 [US1] Implement the minimum create-category handler behavior for admin authorization, title validation, duplicate detection, and repository persistence in `src/backend/BlogPlatform.Application/Posts/CreatePostCategoryHandler.cs`
- [X] T015 [US1] Refactor create-category test setup and result assertions for readability in `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs`

**Checkpoint**: Administrator category creation works end to end within Domain/Application boundaries.

---

## Phase 4: User Story 2 - Administrator updates a category (Priority: P2)

**Goal**: Allow an administrator to update an existing category title while enforcing existence, validity, and uniqueness rules.

**Independent Test**: Category update passes for an administrator with valid input, rejects non-admin actors, rejects missing categories, rejects invalid titles, rejects duplicate replacement titles, and persists the updated category through the repository abstraction.

- [X] T016 [US2] Add a failing Application test for successful admin category update in `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`
- [X] T017 [P] [US2] Add a failing Application test for non-admin category update rejection in `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`
- [X] T018 [P] [US2] Add a failing Application test for missing category rejection during update in `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`
- [X] T019 [P] [US2] Add a failing Application test for invalid updated title rejection in `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`
- [X] T020 [P] [US2] Add a failing Application test for duplicate replacement title rejection in `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`
- [X] T021 [P] [US2] Add a failing Application test that verifies repository-backed category updates on success in `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`
- [X] T022 [US2] Add the compile-minimum update command type in `src/backend/BlogPlatform.Application/Posts/UpdatePostCategoryCommand.cs`
- [X] T023 [US2] Implement the minimum update-category handler behavior for admin authorization, category lookup, title validation, duplicate detection, and repository persistence in `src/backend/BlogPlatform.Application/Posts/UpdatePostCategoryHandler.cs`
- [X] T024 [US2] Refactor update-category test setup and handler result assertions for readability in `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`

**Checkpoint**: Administrator category updates work end to end within Domain/Application boundaries.

---

## Phase 5: User Story 3 - Administrator deactivates a category (Priority: P3)

**Goal**: Allow an administrator to deactivate an existing category while enforcing authorization and existence checks.

**Independent Test**: Category deactivation passes for an administrator, rejects non-admin actors, rejects missing categories, and performs the operation through the repository abstraction.

- [X] T025 [US3] Add a failing Application test for successful admin category deactivation in `tests/backend/BlogPlatform.Application.Tests/Categories/DeactivatePostCategoryHandlerTests.cs`
- [X] T026 [P] [US3] Add a failing Application test for non-admin category deactivation rejection in `tests/backend/BlogPlatform.Application.Tests/Categories/DeactivatePostCategoryHandlerTests.cs`
- [X] T027 [P] [US3] Add a failing Application test for missing category rejection during deactivation in `tests/backend/BlogPlatform.Application.Tests/Categories/DeactivatePostCategoryHandlerTests.cs`
- [X] T028 [P] [US3] Add a failing Application test that verifies repository-backed category deactivation on success in `tests/backend/BlogPlatform.Application.Tests/Categories/DeactivatePostCategoryHandlerTests.cs`
- [X] T029 [US3] Add the compile-minimum deactivate command type in `src/backend/BlogPlatform.Application/Posts/DeactivatePostCategoryCommand.cs`
- [X] T030 [US3] Implement the minimum deactivate-category handler behavior for admin authorization, category lookup, and repository-backed deactivation in `src/backend/BlogPlatform.Application/Posts/DeactivatePostCategoryHandler.cs`
- [X] T031 [US3] Refactor deactivate-category test setup and handler result assertions for readability in `tests/backend/BlogPlatform.Application.Tests/Categories/DeactivatePostCategoryHandlerTests.cs`

**Checkpoint**: Administrator category deactivation works end to end within Domain/Application boundaries.

---

## Phase 6: Polish & Cross-Cutting

**Purpose**: Finalize documentation and run focused verification across the completed slice.

- [X] T032 Update the feature quickstart to reflect the implemented category-management test and source files in `specs/007-manage-categories/quickstart.md`
- [X] T033 Run the backend Domain and Application test projects for category-management verification in `tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj` and `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`
- [X] T034 Review completed work against the category-management plan constraints and mark task completion in `specs/007-manage-categories/tasks.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1: No dependencies
- Phase 2: Depends on Phase 1
- Phase 3: Depends on Phase 2
- Phase 4: Depends on Phase 2 and can start after Phase 3 compile-minimum category primitives exist
- Phase 5: Depends on Phase 2 and can start after Phase 3 compile-minimum category primitives exist
- Phase 6: Depends on Phases 3, 4, and 5

### User Story Dependencies

- `US1` is the MVP slice and should be implemented first.
- `US2` depends on the shared Domain model and repository abstraction from Phase 2 and benefits from the create-category result pattern from `US1`.
- `US3` depends on the shared Domain model and repository abstraction from Phase 2 and can reuse the admin authorization/result pattern from `US1`.

### Within Each User Story

- Write the failing tests before adding compile-minimum production types.
- Add only the minimum command/result/handler code needed to make the tests compile.
- Implement the handler behavior only after the red tests are in place.
- Refactor test setup and shared assertions only after the story tests are green.

### Parallel Opportunities

- `T005` can run in parallel with `T004` once the Domain test file scaffold exists.
- `T009`-`T012` can run in parallel after `T008` establishes the create-category test fixture.
- `T017`-`T021` can run in parallel after `T016` establishes the update-category test fixture.
- `T026`-`T028` can run in parallel after `T025` establishes the deactivate-category test fixture.
- `T032` and `T034` can run in parallel after implementation stabilizes.

---

## Implementation Strategy

### MVP First

1. Complete Phases 1 and 2 to establish category primitives and repository boundaries.
2. Deliver `US1` as the MVP slice for administrator category creation.
3. Validate `US1` independently before expanding to update and deactivation flows.

### Incremental Delivery

1. Add `US2` to cover category updates with missing-category and duplicate-title protection.
2. Add `US3` to cover category deactivation using the simplest availability-based removal behavior.
3. Finish with documentation and full Domain/Application test verification.

### TDD Discipline

- Every handler behavior task must be preceded by failing tests that describe the expected authorization, validation, and persistence behavior.
- No API, Infrastructure, raw SQL, or frontend tasks are included in this slice.
- Repository abstractions stay in the Application layer; authorization and validation stay outside controllers.
