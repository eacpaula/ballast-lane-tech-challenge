# Tasks: Prepare API for Frontend Integration

**Input**: Design documents from `/specs/011-frontend-api-prep/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`

**Tests**: Backend tests are mandatory for the new category-listing slice. Add
Application, repository, and API integration tests before production code.
Docker Compose and Swagger readiness also require manual verification.

**Organization**: Tasks are grouped by user story so local runtime readiness
and category-listing behavior can be implemented and validated independently.

## Phase 1: Setup

**Purpose**: Prepare the API container build inputs and local runtime
configuration defaults shared by the rest of the feature.

- [X] T001 Add API container runtime placeholders and local frontend origin variables in `.env.example`
- [X] T002 Create the API container build definition in `src/backend/BlogPlatform.Api/Dockerfile`
- [X] T003 Update root Docker build exclusions for the new API image context in `.dockerignore`
- [X] T004 Add API runtime configuration placeholders for container execution in `src/backend/BlogPlatform.Api/appsettings.json` and `src/backend/BlogPlatform.Api/appsettings.Development.json`

---

## Phase 2: Foundational Backend

**Purpose**: Add the shared runtime and abstraction changes that block both user
stories.

- [X] T005 Add the API service skeleton with PostgreSQL dependency and exposed API port in `docker-compose.yml`
- [X] T006 Create configuration-driven local CORS settings support in `src/backend/BlogPlatform.Api/Configuration/LocalCorsSettings.cs`
- [X] T007 [P] Extend API configuration binding for container-safe environment overrides in `src/backend/BlogPlatform.Api/Configuration/JwtAuthenticationSettings.cs` and `src/backend/BlogPlatform.Infrastructure/Configuration/PostgreSqlConnectionSettings.cs`
- [X] T008 [P] Extend the category repository abstraction for available-category reads in `src/backend/BlogPlatform.Application/Abstractions/ICategoryRepository.cs`

**Checkpoint**: Docker/runtime configuration and the shared category read
abstraction are ready; story-level work can begin.

---

## Phase 3: User Story 1 - Developer runs the backend API locally for frontend work (Priority: P1) 🎯

**Goal**: Make the API runnable through Docker Compose with PostgreSQL
connectivity, local-development CORS, and Swagger access for manual frontend
integration work.

**Independent Test**: `docker compose up -d postgres api` starts both services,
the API can reach PostgreSQL, Swagger is reachable locally, and a configured
local frontend origin receives the expected CORS response on a public API
route.

### Tests for User Story 1

- [X] T009 [US1] Add a failing API integration test for local frontend CORS behavior on a public endpoint in `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostsCorsTests.cs`

### Implementation for User Story 1

- [X] T010 [P] [US1] Wire local-development CORS and environment-driven API runtime settings in `src/backend/BlogPlatform.Api/Program.cs` and `src/backend/BlogPlatform.Api/Configuration/LocalCorsSettings.cs`
- [X] T011 [P] [US1] Finalize the API Docker Compose service configuration and container environment mapping in `docker-compose.yml` and `src/backend/BlogPlatform.Api/Dockerfile`
- [X] T012 [US1] Validate API container startup and PostgreSQL connectivity through `docker-compose.yml` and `README.md`
- [X] T013 [US1] Validate Swagger availability through the containerized API using `src/backend/BlogPlatform.Api/BlogPlatform.Api.http` and `README.md`
- [X] T014 [US1] Re-run the CORS/API regression test and confirm the documented local runtime flow in `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostsCorsTests.cs` and `specs/011-frontend-api-prep/quickstart.md`

**Checkpoint**: The API is reachable through Docker Compose and can be called
from the local frontend origin with Swagger still available.

---

## Phase 4: User Story 2 - Frontend post forms can load available categories (Priority: P2)

**Goal**: Expose a public read-only category list for post create/edit forms
without weakening administrator-only category management behavior.

**Independent Test**: Application, repository, and API tests pass for
`GET /api/categories/available`, the response contains only available
categories, and existing category write endpoints remain administrator-only.

### Tests for User Story 2 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T015 [P] [US2] Add a failing Application test for listing only available categories in `tests/backend/BlogPlatform.Application.Tests/Categories/ListAvailablePostCategoriesHandlerTests.cs`
- [X] T016 [P] [US2] Add a failing PostgreSQL-backed repository test for available-category filtering in `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/CategoryRepositoryListAvailableTests.cs`
- [X] T017 [P] [US2] Add failing API integration tests for anonymous category listing and unavailable-category exclusion in `tests/backend/BlogPlatform.Api.Tests/Categories/AvailableCategoryListApiTests.cs`

### Implementation for User Story 2

- [X] T018 [P] [US2] Create the available-category Application read model and handler in `src/backend/BlogPlatform.Application/Posts/AvailablePostCategoryListItem.cs` and `src/backend/BlogPlatform.Application/Posts/ListAvailablePostCategoriesHandler.cs`
- [X] T019 [P] [US2] Implement the raw SQL available-category query in `src/backend/BlogPlatform.Infrastructure/Categories/PostgreSqlCategoryRepository.cs`
- [X] T020 [P] [US2] Register the new handler and add the read-only response DTO in `src/backend/BlogPlatform.Api/Extensions/ServiceCollectionExtensions.cs` and `src/backend/BlogPlatform.Api/Contracts/Categories/AvailableCategoryResponse.cs`
- [X] T021 [US2] Implement the public available-categories endpoint in `src/backend/BlogPlatform.Api/Controllers/PublicCategoriesController.cs`
- [X] T022 [US2] Re-run the category listing API tests and the existing category authorization regression tests in `tests/backend/BlogPlatform.Api.Tests/Categories/AvailableCategoryListApiTests.cs` and `tests/backend/BlogPlatform.Api.Tests/Categories/CategoryAuthorizationApiTests.cs`

**Checkpoint**: The frontend can anonymously load available categories, and
category write operations remain administrator-only.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation, demo flow, and full-slice validation
without expanding scope.

- [X] T023 [P] Update backend startup and frontend-consumption documentation in `README.md` and `docs/Database-Implementation-Strategy.md`
- [X] T024 [P] Update local API exploration examples and feature verification notes in `src/backend/BlogPlatform.Api/BlogPlatform.Api.http` and `specs/011-frontend-api-prep/quickstart.md`
- [X] T025 [P] Document that tag listing is deferred for this slice in `specs/011-frontend-api-prep/spec.md` and `README.md`
- [X] T026 Re-run the backend suites in `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`, `tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj`, and `tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj`
- [X] T027 Validate the Docker-based demo flow and Swagger surface through `docker-compose.yml`, `README.md`, and `src/backend/BlogPlatform.Api/BlogPlatform.Api.http`
- [X] T028 Review the API/runtime slice for thin-controller compliance and confirm no out-of-scope frontend or authorization changes were introduced in `src/backend/BlogPlatform.Api`, `src/backend/BlogPlatform.Application`, and `specs/011-frontend-api-prep/tasks.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 starts first.
- Phase 2 depends on Phase 1 and blocks all story work.
- Phase 3 depends on Phase 2.
- Phase 4 depends on Phase 2 and can proceed after the shared runtime and
  repository abstraction changes are in place.
- Final Phase depends on the selected user stories being complete.

### User Story Dependencies

- `US1` establishes the local backend runtime path needed before frontend
  integration work is comfortable.
- `US2` depends on the foundational category read abstraction from Phase 2 but
  does not depend on frontend code.
- `US2` should follow `US1` because the category endpoint is meant to be
  consumed through the newly documented local API runtime.

### Within Each User Story

- For `US1`, add the CORS regression test before runtime/configuration code.
- For `US2`, Application, repository, and API tests MUST fail before the
  handler, repository query, and controller endpoint are implemented.
- Controllers remain thin and delegate to Application.
- SQL filtering remains isolated to Infrastructure.

### Parallel Opportunities

- `T007` and `T008` can run in parallel after `T006`.
- `T010` and `T011` can run in parallel after `T009`.
- `T015`-`T017` can run in parallel once the story scope is fixed.
- `T018`-`T020` can run in parallel after the failing tests are in place.
- `T023`-`T025` can run in parallel after story work is stable.

---

## Implementation Strategy

### MVP First

1. Complete Setup.
2. Complete Foundational Backend.
3. Deliver `US1` so the API can be run and inspected through Docker Compose.
4. Deliver `US2` so the frontend has the minimum read surface it needs.

### Incremental Delivery

1. Establish API container build/runtime configuration and local CORS.
2. Prove local Compose startup and Swagger reachability.
3. Add the available-category Application query, SQL repository support, and
   public endpoint.
4. Confirm category writes remain administrator-only.
5. Finish with documentation and full-slice validation.

## Notes

- `[P]` tasks indicate parallelizable work across different files.
- `US1` and `US2` are intentionally backend-only; no frontend implementation is
  included in this feature.
- No tag-list endpoint is planned in this task list because the current feature
  keeps that scope deferred.
