---

description: "Task list for paginating categories and supporting category descriptions"

---

# Tasks: Paginate Categories and Support Category Descriptions

**Input**: Design documents from `/specs/017-category-pagination-description/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Backend tests are mandatory. For this feature, create or update
Application, API integration, and PostgreSQL-backed repository tests before
production code. Frontend validation stays pragmatic with lint/build and
full-stack manual verification.

**Organization**: Tasks are grouped by user story to enable independent
implementation and testing of the category pagination and description flow.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (`US1`)
- Include exact file paths in descriptions

## Path Conventions

- **Backend Application**: `src/backend/BlogPlatform.Application/`
- **Backend Domain**: `src/backend/BlogPlatform.Domain/`
- **Backend API**: `src/backend/BlogPlatform.Api/`
- **Backend Infrastructure**: `src/backend/BlogPlatform.Infrastructure/`
- **Backend tests**: `tests/backend/BlogPlatform.Application.Tests/`,
  `tests/backend/BlogPlatform.Api.Tests/`,
  `tests/backend/BlogPlatform.Infrastructure.Tests/`,
  `tests/backend/BlogPlatform.Domain.Tests/`
- **Database scripts**: `database/scripts/` and `database/seeds/`
- **Frontend app**: `src/frontend/blog-web/src/`
- **Feature docs**: `specs/017-category-pagination-description/`

## Phase 1: Setup (Shared Category Flow Review and Test Support)

**Purpose**: Verify the current category data baseline and prepare reusable test
support for the contract change.

- [X] T001 Verify the existing category description column and sample seed descriptions in `database/scripts/004-create-post-categories.sql` and `database/seeds/004-seed-post-categories.sql`, updating them only if the column or example data is missing
- [X] T002 Create shared category pagination test data helpers in `tests/backend/BlogPlatform.Api.Tests/TestSupport/CategoryPaginationTestData.cs` and `tests/backend/BlogPlatform.Infrastructure.Tests/TestSupport/CategoryPaginationTestData.cs`
- [X] T003 [P] Create reusable Application category repository stubs for paginated reads and description-aware writes in `tests/backend/BlogPlatform.Application.Tests/Categories/CategoryRepositoryStub.cs`
- [X] T004 [P] Add frontend category pagination response scaffolding in `src/frontend/blog-web/src/features/categories/category.types.ts`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Establish the shared models and contracts required by all category
pagination and description work.

**CRITICAL**: Frontend category updates wait until this phase is complete.

- [X] T005 Add category page request and paginated result models in `src/backend/BlogPlatform.Application/Posts/CategoryPageRequest.cs` and `src/backend/BlogPlatform.Application/Posts/PaginatedCategoryResult.cs`
- [X] T006 Update category read/list item models to carry optional descriptions in `src/backend/BlogPlatform.Application/Posts/ManagedPostCategoryListItem.cs` and `src/backend/BlogPlatform.Application/Posts/AvailablePostCategoryListItem.cs`
- [X] T007 Update the category repository contract for paginated list reads and description-aware mapping in `src/backend/BlogPlatform.Application/Abstractions/ICategoryRepository.cs`
- [X] T008 [P] Update `PostCategory` to support an optional description and description-preserving rehydration in `src/backend/BlogPlatform.Domain/Categories/PostCategory.cs`
- [X] T009 [P] Update category-related Application test doubles to compile against the new repository and category models in `tests/backend/BlogPlatform.Application.Tests/Categories/` and `tests/backend/BlogPlatform.Application.Tests/Posts/`

**Checkpoint**: Shared category pagination and description contracts are ready
for test-first story work.

---

## Phase 3: User Story 1 - Paginated Category Management With Optional Descriptions (Priority: P1) 🎯 MVP

**Goal**: Let the existing category list endpoints return paginated data while
administrator users create and update categories with an optional description,
without changing existing authorization boundaries.

**Independent Test**: Start the stack, call `GET /api/categories/available` and
`GET /api/categories` with `page` and `pageSize`, confirm the paginated
metadata is correct, then log in as the seeded administrator and create, edit,
and clear a category description while non-admin users remain blocked from
category writes.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T010 [P] [US1] Add Domain tests for optional description normalization and clearing in `tests/backend/BlogPlatform.Domain.Tests/Categories/PostCategoryTests.cs`
- [X] T011 [P] [US1] Add Application tests for creating and updating categories with optional descriptions in `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs` and `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`
- [X] T012 [P] [US1] Add Application tests for paginated available category listing in `tests/backend/BlogPlatform.Application.Tests/Categories/ListAvailablePostCategoriesHandlerTests.cs`
- [X] T013 [P] [US1] Add Application tests for paginated administrator category listing and invalid page inputs in `tests/backend/BlogPlatform.Application.Tests/Categories/ListAllCategoriesHandlerTests.cs`
- [X] T014 [P] [US1] Add repository tests for available-category pagination and description projection in `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/CategoryRepositoryListAvailableTests.cs`
- [X] T015 [P] [US1] Add repository tests for administrator-category pagination, total counts, and unavailable rows in `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/CategoryRepositoryAdminListTests.cs`
- [X] T016 [P] [US1] Add API tests for paginated `GET /api/categories/available` responses in `tests/backend/BlogPlatform.Api.Tests/Categories/AvailableCategoryListApiTests.cs`
- [X] T017 [P] [US1] Add API tests for paginated `GET /api/categories` responses and unchanged admin-only protection in `tests/backend/BlogPlatform.Api.Tests/Categories/AdminCategoryListApiTests.cs` and `tests/backend/BlogPlatform.Api.Tests/Categories/CategoryAuthorizationApiTests.cs`
- [X] T018 [P] [US1] Add API tests for create/update description round-trips in `tests/backend/BlogPlatform.Api.Tests/Categories/CreateCategoryApiTests.cs` and `tests/backend/BlogPlatform.Api.Tests/Categories/UpdateCategoryApiTests.cs`

### Backend Implementation for User Story 1

- [X] T019 [US1] Extend category create/update commands and result models with optional descriptions in `src/backend/BlogPlatform.Application/Posts/CreatePostCategoryCommand.cs`, `src/backend/BlogPlatform.Application/Posts/UpdatePostCategoryCommand.cs`, and `src/backend/BlogPlatform.Application/Posts/PostCategoryManagementResult.cs`
- [X] T020 [P] [US1] Update category create and update handlers to normalize optional descriptions, keep required-title validation, and preserve duplicate-title checks in `src/backend/BlogPlatform.Application/Posts/CreatePostCategoryHandler.cs` and `src/backend/BlogPlatform.Application/Posts/UpdatePostCategoryHandler.cs`
- [X] T021 [P] [US1] Update category list handlers to accept `page` and `pageSize`, enforce existing pagination conventions, and return paginated metadata in `src/backend/BlogPlatform.Application/Posts/ListAllPostCategoriesHandler.cs` and `src/backend/BlogPlatform.Application/Posts/ListAvailablePostCategoriesHandler.cs`
- [X] T022 [P] [US1] Update the raw SQL category repository to persist and read descriptions in create, update, get-by-id, and deactivate paths in `src/backend/BlogPlatform.Infrastructure/Categories/PostgreSqlCategoryRepository.cs`
- [X] T023 [P] [US1] Add parameterized paginated list and total-count SQL for admin and available categories in `src/backend/BlogPlatform.Infrastructure/Categories/PostgreSqlCategoryRepository.cs`
- [X] T024 [US1] Update category API request and response DTOs with optional description and paginated envelope types in `src/backend/BlogPlatform.Api/Contracts/Categories/CreateCategoryRequest.cs`, `src/backend/BlogPlatform.Api/Contracts/Categories/UpdateCategoryRequest.cs`, `src/backend/BlogPlatform.Api/Contracts/Categories/CategoryResponse.cs`, `src/backend/BlogPlatform.Api/Contracts/Categories/AdminCategoryListItemResponse.cs`, `src/backend/BlogPlatform.Api/Contracts/Categories/AvailableCategoryResponse.cs`, and `src/backend/BlogPlatform.Api/Contracts/Categories/PaginatedCategoryResponse.cs`
- [X] T025 [US1] Update the administrator category controller for paginated reads and description-aware writes in `src/backend/BlogPlatform.Api/Controllers/CategoriesController.cs`
- [X] T026 [US1] Update the public available-category controller to accept `page` and `pageSize` and return the paginated response model in `src/backend/BlogPlatform.Api/Controllers/PublicCategoriesController.cs`
- [X] T027 [US1] Re-run backend category tests in `tests/backend/BlogPlatform.Domain.Tests/Categories/`, `tests/backend/BlogPlatform.Application.Tests/Categories/`, `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/`, and `tests/backend/BlogPlatform.Api.Tests/Categories/` and confirm empty-page, description, and authorization behavior

### Frontend for User Story 1

- [X] T028 [US1] Update the category API client to consume paginated responses and submit optional descriptions in `src/frontend/blog-web/src/features/categories/categories.api.ts`
- [X] T029 [P] [US1] Extend frontend category types for paginated lists and optional descriptions in `src/frontend/blog-web/src/features/categories/category.types.ts`
- [X] T030 [US1] Enable the existing description field and submit title plus description from the category form in `src/frontend/blog-web/src/features/categories/CategoryForm.tsx`
- [X] T031 [US1] Update the admin category management page to render paginated category data, show descriptions where appropriate, and keep create/update/deactivate flows working in `src/frontend/blog-web/src/features/categories/AdminCategoriesPage.tsx` and `src/frontend/blog-web/src/features/categories/CategoryListItem.tsx`
- [X] T032 [US1] Update post-form category loading to consume the paginated available-category contract with a practical page size in `src/frontend/blog-web/src/features/posts/public-posts.api.ts`, `src/frontend/blog-web/src/features/posts/CreatePostPage.tsx`, and `src/frontend/blog-web/src/features/posts/EditPostPage.tsx`
- [X] T033 [US1] Verify keyboard access, semantic form behavior, responsive layout, and existing `DESIGN.md` alignment for the category management changes in `src/frontend/blog-web/src/features/categories/AdminCategoriesPage.tsx` and `src/frontend/blog-web/src/features/categories/CategoryForm.tsx`

**Checkpoint**: Category pagination and optional description support are
functional and independently testable.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Validate the full stack, document the contract change, and keep
the slice within scope.

- [X] T034 [P] Run backend automated checks in `tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj`, `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`, `tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj`, and `tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj`
- [X] T035 [P] Run frontend validation with `npm run lint` and `npm run build` from `src/frontend/blog-web/`
- [X] T036 Validate the full stack with `docker compose up -d postgres api frontend` using the steps in `specs/017-category-pagination-description/quickstart.md`
- [X] T037 Validate Swagger/OpenAPI output and direct paginated category behavior for `GET /api/categories?page=1&pageSize=10` and `GET /api/categories/available?page=1&pageSize=10` from the running `/swagger` document and `src/backend/BlogPlatform.Api/Controllers/`
- [X] T038 Validate administrator description create/update/clear behavior and non-admin write rejection through the running stack using `specs/017-category-pagination-description/quickstart.md`
- [X] T039 Validate the frontend category management flow and post create/edit category dropdown behavior in `src/frontend/blog-web/src/features/categories/` and `src/frontend/blog-web/src/features/posts/` and check the browser console for warnings
- [X] T040 Update category pagination and description documentation in `README.md`, `src/frontend/blog-web/README.md`, and `specs/017-category-pagination-description/quickstart.md`
- [X] T041 Review the category backend and frontend changes in `src/backend/` and `src/frontend/blog-web/src/features/categories/` and remove any unnecessary abstractions or out-of-scope additions

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup starts first
- Foundational Backend depends on Setup and blocks all story work
- User Story 1 tests come before any production code
- Backend contract, repository, and API changes must pass before frontend
  category consumers are updated
- Final Polish depends on User Story 1 being complete

### Within User Story 1

- Domain, Application, repository, and API tests MUST be written and fail
  before implementation begins
- Domain and Application validation work precedes controller wiring
- Raw SQL pagination and description mapping remain isolated to
  `PostgreSqlCategoryRepository`
- Public and admin category list routes must move to the paginated contract
  before frontend category consumers are updated
- Frontend changes must preserve the current `DESIGN.md`-aligned UI rather than
  redesign the category workflow

### Parallel Opportunities

- `T003` and `T004` can run in parallel
- `T008` and `T009` can run in parallel after `T007`
- `T010` through `T018` can run in parallel once the foundational phase is complete
- `T020` through `T023` can run in parallel after `T019`
- `T028` and `T029` can run in parallel after `T027`
- `T034` and `T035` can run in parallel after implementation stabilizes

---

## Implementation Strategy

### MVP First

1. Complete Setup
2. Complete Foundational Backend contracts
3. Deliver User Story 1 with failing backend tests first
4. Validate paginated backend behavior and description persistence before
   touching the frontend
5. Update the current frontend category flows and finish with full-stack
   verification

### Incremental Delivery

1. Verify the schema baseline and prepare shared category test support
2. Lock down description and pagination behavior in tests
3. Implement Domain/Application command and query updates
4. Implement parameterized PostgreSQL pagination and description mapping
5. Expose the revised category API contracts
6. Adapt the existing admin category screen and post-form category loading
7. Validate Swagger, Docker Compose flow, and documentation

## Notes

- `[P]` tasks indicate work that can proceed in parallel across different files
- No Redis caching, category search, hierarchy, slug generation, or major UI
  redesign is part of this task list
