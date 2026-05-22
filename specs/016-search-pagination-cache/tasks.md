---

description: "Task list for paginated post search with Redis caching"

---

# Tasks: Add Paginated Post Search With Redis Caching

**Input**: Design documents from `/specs/016-search-pagination-cache/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Backend tests are mandatory. For this feature, create Application,
API integration, cache-focused, and PostgreSQL-backed repository tests before
production code. Frontend validation stays pragmatic with lint/build and
full-stack manual checks.

**Organization**: Tasks are grouped by user story to enable independent
implementation and testing of the paginated search and caching flow.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (`US1`)
- Include exact file paths in descriptions

## Path Conventions

- **Backend Application**: `src/backend/BlogPlatform.Application/`
- **Backend API**: `src/backend/BlogPlatform.Api/`
- **Backend Infrastructure**: `src/backend/BlogPlatform.Infrastructure/`
- **Backend tests**: `tests/backend/BlogPlatform.Application.Tests/`,
  `tests/backend/BlogPlatform.Api.Tests/`,
  `tests/backend/BlogPlatform.Infrastructure.Tests/`
- **Frontend app**: `src/frontend/blog-web/src/`
- **Feature docs**: `specs/016-search-pagination-cache/`

## Phase 1: Setup (Shared Runtime and Test Support)

**Purpose**: Prepare reusable Redis/runtime scaffolding and test helpers needed
by the feature.

- [ ] T001 Create shared paginated search test data helpers in `tests/backend/BlogPlatform.Api.Tests/TestSupport/PaginatedPostSearchTestData.cs` and `tests/backend/BlogPlatform.Infrastructure.Tests/TestSupport/PaginatedPostSearchTestData.cs`
- [ ] T002 [P] Create reusable Application pagination and cache stubs in `tests/backend/BlogPlatform.Application.Tests/Posts/PaginatedPostRepositoryStub.cs` and `tests/backend/BlogPlatform.Application.Tests/Posts/PostListCacheStub.cs`
- [ ] T003 [P] Add the Redis NuGet dependency and any related package updates in `src/backend/BlogPlatform.Infrastructure/BlogPlatform.Infrastructure.csproj`
- [ ] T004 [P] Add Redis service defaults for local development in `.env.example`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Establish the shared contracts, configuration, and runtime wiring
that all paginated search work depends on.

**CRITICAL**: Frontend pagination work waits until this phase is complete.

- [ ] T005 Add the Redis service and API dependency wiring in `docker-compose.yml`
- [ ] T006 [P] Add Redis configuration settings in `src/backend/BlogPlatform.Infrastructure/Configuration/RedisCacheSettings.cs`
- [ ] T007 [P] Add Redis configuration binding and connection setup in `src/backend/BlogPlatform.Infrastructure/DependencyInjection.cs`
- [ ] T008 Add paginated list/search contracts to `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs` and add a feature-scoped cache abstraction in `src/backend/BlogPlatform.Application/Abstractions/IPostListCache.cs`
- [ ] T009 [P] Update affected `IPostRepository` test doubles under `tests/backend/BlogPlatform.Application.Tests/Posts/`, `tests/backend/BlogPlatform.Application.Tests/Reactions/`, and `tests/backend/BlogPlatform.Application.Tests/Categories/` to compile with the new pagination contract
- [ ] T010 [P] Add shared paginated result models in `src/backend/BlogPlatform.Application/Posts/PostListPageRequest.cs` and `src/backend/BlogPlatform.Application/Posts/PaginatedPublicPostResult.cs`

**Checkpoint**: Redis runtime wiring and backend contracts are ready for
test-first story work.

---

## Phase 3: User Story 1 - Paginated Post Search With Safe Redis Caching (Priority: P1) 🎯 MVP

**Goal**: Let anonymous and authenticated users browse and search posts through
paginated backend responses, while Redis caches read results for 30 seconds
without leaking private data across viewers.

**Independent Test**: Start the stack, confirm anonymous `GET /api/posts` uses
paginated responses, search with a public term, then authenticate and confirm
an owned private match can appear without exposing another user’s private post;
repeat a request within 30 seconds and again after expiration to validate cache
behavior.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T011 [P] [US1] Add anonymous paginated listing and search Application tests in `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsPaginationAnonymousTests.cs`
- [ ] T012 [P] [US1] Add authenticated owner-inclusive paginated search Application tests in `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsPaginationAuthenticatedTests.cs`
- [ ] T013 [P] [US1] Add Application cache orchestration tests for viewer-specific keys and empty-search behavior in `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsCacheTests.cs`
- [ ] T014 [P] [US1] Add anonymous paginated repository tests in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryPaginationAnonymousTests.cs`
- [ ] T015 [P] [US1] Add authenticated paginated repository tests in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryPaginationAuthenticatedTests.cs`
- [ ] T016 [P] [US1] Add Redis cache provider tests for key isolation and TTL configuration in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostListCacheTests.cs`
- [ ] T017 [P] [US1] Add anonymous paginated API tests in `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostPaginationAnonymousTests.cs`
- [ ] T018 [P] [US1] Add authenticated paginated API tests proving owned private inclusion and foreign private exclusion in `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostPaginationAuthenticatedTests.cs`

### Backend Implementation for User Story 1

- [ ] T019 [US1] Update `src/backend/BlogPlatform.Application/Posts/ListPublicPostsHandler.cs` to accept `page` and `pageSize`, enforce default and bounded pagination, keep visibility rules in Application, and orchestrate cached reads
- [ ] T020 [P] [US1] Add paginated post cache-key composition and normalized query handling in `src/backend/BlogPlatform.Application/Posts/PostListCacheKeyFactory.cs`
- [ ] T021 [P] [US1] Update `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs` to support paginated list/search queries with deterministic ordering and parameterized `LIMIT`/`OFFSET`
- [ ] T022 [P] [US1] Add the supporting total-count queries and paginated projection mapping in `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
- [ ] T023 [P] [US1] Implement the Redis-backed cache provider in `src/backend/BlogPlatform.Infrastructure/Caching/RedisPostListCache.cs`
- [ ] T024 [P] [US1] Add Redis connection creation support in `src/backend/BlogPlatform.Infrastructure/Caching/RedisConnectionFactory.cs`
- [ ] T025 [US1] Update the public posts response DTOs to a paginated envelope in `src/backend/BlogPlatform.Api/Contracts/Posts/PaginatedPublicPostResponse.cs` and `src/backend/BlogPlatform.Api/Contracts/Posts/PublicPostSummaryResponse.cs`
- [ ] T026 [US1] Update `src/backend/BlogPlatform.Api/Controllers/PublicPostsController.cs` to accept `q`, `page`, and `pageSize`, pass optional authenticated context, and return the paginated response model
- [ ] T027 [US1] Update handler registration or Infrastructure service wiring for the new cache implementation in `src/backend/BlogPlatform.Api/Extensions/ServiceCollectionExtensions.cs` and `src/backend/BlogPlatform.Infrastructure/DependencyInjection.cs`
- [ ] T028 [US1] Re-run backend pagination and caching tests in `tests/backend/BlogPlatform.Application.Tests/Posts/`, `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/`, and `tests/backend/BlogPlatform.Api.Tests/Posts/` and confirm invalid-page, empty-results, and private-visibility behaviors

### Frontend for User Story 1

- [ ] T029 [US1] Update the public posts API helper to send `q`, `page`, and `pageSize` and consume the paginated envelope in `src/frontend/blog-web/src/features/posts/public-posts.api.ts`
- [ ] T030 [P] [US1] Add paginated frontend post response types in `src/frontend/blog-web/src/features/posts/post.types.ts`
- [ ] T031 [US1] Replace the single-load public feed with resettable paginated loading in `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`
- [ ] T032 [US1] Add simple infinite-scroll triggering and duplicate-safe page appending in `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`
- [ ] T033 [US1] Preserve the current search input route flow and authenticated token usage for paginated requests in `src/frontend/blog-web/src/app/AppShell.tsx` and `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`
- [ ] T034 [US1] Verify initial-loading, next-page-loading, empty-results, error, and end-of-list states in `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`
- [ ] T035 [US1] Verify keyboard access, focus states, semantic HTML, and responsive infinite-scroll behavior in `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx` and `src/frontend/blog-web/src/app/AppShell.tsx`

**Checkpoint**: Paginated post listing and search with safe Redis caching are
functional and independently testable.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Validate the full stack, document the Redis-backed flow, and keep
the slice within scope.

- [ ] T036 [P] Run backend automated checks in `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`, `tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj`, and `tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj`
- [ ] T037 [P] Run frontend validation with `npm run lint` and `npm run build` from `src/frontend/blog-web/`
- [ ] T038 Validate the full stack with `docker compose up -d postgres redis api frontend` using the steps in `specs/016-search-pagination-cache/quickstart.md`
- [ ] T039 Validate repeated requests within and after the 30-second cache window through the running stack and record notes in `specs/016-search-pagination-cache/quickstart.md`
- [ ] T040 Update Redis, pagination, TTL, and stale-data trade-off documentation in `README.md`, `src/frontend/blog-web/README.md`, and `specs/016-search-pagination-cache/quickstart.md`
- [ ] T041 Verify Swagger/OpenAPI output and direct paginated API behavior for `GET /api/posts?q=...&page=...&pageSize=...` from `src/backend/BlogPlatform.Api/Controllers/PublicPostsController.cs` and the running `/swagger` document
- [ ] T042 Review the pagination, caching, and frontend feed changes in `src/backend/` and `src/frontend/blog-web/src/features/posts/` and remove any unnecessary abstractions or out-of-scope additions

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup starts first
- Foundational Backend depends on Setup and blocks story implementation
- User Story 1 tests come before any backend production code
- Backend Application, repository, cache, and API changes must pass before
  frontend pagination wiring begins
- Final Polish depends on User Story 1 being complete

### Within User Story 1

- Application, repository, cache, and API tests MUST be written and fail
  before implementation begins
- Application pagination and visibility logic precede controller wiring
- Repository SQL remains isolated to `PostgreSqlPostRepository`
- Redis implementation remains isolated to backend Infrastructure code
- Frontend pagination updates wait for stable backend contracts and passing
  backend tests
- Frontend changes must preserve the current `DESIGN.md`-aligned UI rather than
  redesign the feed

### Parallel Opportunities

- `T001` and `T002` can run in parallel
- `T003` and `T004` can run in parallel
- `T006`, `T007`, `T009`, and `T010` can run in parallel after `T005`
- `T011` through `T018` can run in parallel once the foundational phase is complete
- `T020` through `T024` can run in parallel after `T019`
- `T029` and `T030` can run in parallel after `T028`
- `T036` and `T037` can run in parallel after story implementation stabilizes

---

## Implementation Strategy

### MVP First

1. Complete Setup
2. Complete Foundational Backend and Redis wiring
3. Deliver User Story 1 with failing backend tests first
4. Validate paginated backend behavior and viewer-safe caching before touching
   the frontend feed
5. Wire the frontend to the paginated API contract and finish with full-stack
   Redis validation

### Incremental Delivery

1. Prepare shared paginated-search test support and Redis runtime config
2. Lock down pagination, visibility, and cache-key isolation in tests
3. Implement Application orchestration and backend cache boundaries
4. Implement parameterized PostgreSQL pagination and Redis caching
5. Expose the paginated public list endpoint
6. Replace single-load frontend behavior with infinite scrolling
7. Validate the full stack and update docs

## Notes

- `[P]` tasks indicate work that can proceed in parallel across different files
- No cache invalidation machinery, cursor pagination, ranking, or external
  search infrastructure is part of this task list
