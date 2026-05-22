---

description: "Task list for search posts through backend API"

---

# Tasks: Search Posts Through Backend API

**Input**: Design documents from `/specs/015-search-posts/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Backend tests are mandatory. For this feature, create the relevant
Application, API integration, and PostgreSQL-backed repository tests before
production code. Frontend verification stays pragmatic with lint/build and
manual full-stack validation.

**Organization**: Tasks are grouped by user story to enable independent
implementation and testing of the backend-powered search flow.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (`US1`)
- Include exact file paths in descriptions

## Path Conventions

- **Backend Application**: `src/backend/BlogPlatform.Application/`
- **Backend API**: `src/backend/BlogPlatform.Api/`
- **Backend Infrastructure**: `src/backend/BlogPlatform.Infrastructure/`
- **Backend tests**: `tests/backend/BlogPlatform.Application.Tests/`,
  `tests/backend/BlogPlatform.Api.Tests/`,
  `tests/backend/BlogPlatform.Infrastructure.Tests/`
- **Frontend app**: `src/frontend/blog-web/src/`
- **Feature docs**: `specs/015-search-posts/`

## Phase 1: Setup (Shared Test Support)

**Purpose**: Prepare small reusable helpers so the search tests can be written
cleanly without editing permanent seed data.

- [ ] T001 Create shared API and repository search test data helpers in `tests/backend/BlogPlatform.Api.Tests/TestSupport/SearchPostTestData.cs` and `tests/backend/BlogPlatform.Infrastructure.Tests/TestSupport/SearchPostTestData.cs`
- [ ] T002 [P] Create a reusable application post-repository search stub in `tests/backend/BlogPlatform.Application.Tests/Posts/SearchPostRepositoryStub.cs`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Establish the minimal shared contract changes required before the
search story can be completed.

**CRITICAL**: Frontend search wiring waits until the backend contracts and tests
are stable.

- [ ] T003 Add the search-oriented post repository contract in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [ ] T004 [P] Update affected `IPostRepository` test doubles under `tests/backend/BlogPlatform.Application.Tests/Posts/`, `tests/backend/BlogPlatform.Application.Tests/Reactions/`, and `tests/backend/BlogPlatform.Application.Tests/Categories/` to compile with the new search contract

**Checkpoint**: The repository abstraction is ready for test-first search work.

---

## Phase 3: User Story 1 - Backend-Powered Post Search (Priority: P1) 🎯 MVP

**Goal**: Replace client-side-only filtering with backend-powered search that
preserves anonymous visibility, authenticated owner visibility, and the existing
public listing experience.

**Independent Test**: Start the stack, search for a public seeded term as an
anonymous visitor, clear the search back to the default list, then authenticate
and confirm a matching owned private post can appear without exposing another
user’s private post.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T005 [P] [US1] Add anonymous search Application tests in `tests/backend/BlogPlatform.Application.Tests/Posts/SearchPostsAnonymousTests.cs`
- [ ] T006 [P] [US1] Add authenticated owner-inclusive search Application tests in `tests/backend/BlogPlatform.Application.Tests/Posts/SearchPostsAuthenticatedTests.cs`
- [ ] T007 [P] [US1] Add anonymous repository search tests in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositorySearchAnonymousTests.cs`
- [ ] T008 [P] [US1] Add authenticated repository search tests in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositorySearchAuthenticatedTests.cs`
- [ ] T009 [P] [US1] Add anonymous API search tests in `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostSearchAnonymousTests.cs`
- [ ] T010 [P] [US1] Add authenticated API search tests in `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostSearchAuthenticatedTests.cs`

### Backend Implementation for User Story 1

- [ ] T011 [US1] Update `src/backend/BlogPlatform.Application/Posts/ListPublicPostsHandler.cs` to accept a search term plus optional authenticated user context and enforce search visibility rules
- [ ] T012 [P] [US1] Update `src/backend/BlogPlatform.Application/Posts/PublicPostListItem.cs` and related Application search projections if needed to preserve the existing list response shape
- [ ] T013 [US1] Implement parameterized anonymous and authenticated search SQL in `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
- [ ] T014 [US1] Extend the Infrastructure search query in `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs` to cover title and description at minimum, plus content and category title, and tag matching only if it stays a small join-based extension
- [ ] T015 [US1] Update the public listing endpoint to accept `q` and optional authenticated context in `src/backend/BlogPlatform.Api/Controllers/PublicPostsController.cs`
- [ ] T016 [US1] Update handler wiring and OpenAPI-visible endpoint behavior in `src/backend/BlogPlatform.Api/Extensions/ServiceCollectionExtensions.cs` and `src/backend/BlogPlatform.Api/Contracts/Posts/PublicPostSummaryResponse.cs` only if the search-enabled list flow requires contract adjustments
- [ ] T017 [US1] Re-run backend search tests and confirm empty-search, no-results, and private-visibility behaviors in `tests/backend/BlogPlatform.Application.Tests/Posts/`, `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/`, and `tests/backend/BlogPlatform.Api.Tests/Posts/`

### Frontend for User Story 1

- [ ] T018 [US1] Update the public posts API helper to send `q` and an optional bearer token in `src/frontend/blog-web/src/features/posts/public-posts.api.ts`
- [ ] T019 [US1] Replace client-side filtering with backend requests in `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`
- [ ] T020 [US1] Preserve the existing search-box route flow and authenticated token usage in `src/frontend/blog-web/src/app/AppShell.tsx` and `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`
- [ ] T021 [US1] Verify loading, no-results, and error rendering for backend search in `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`
- [ ] T022 [US1] Verify keyboard access, focus states, semantic HTML, and readable contrast for the search-driven public listing in `src/frontend/blog-web/src/app/AppShell.tsx` and `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`

**Checkpoint**: Backend-powered post search is functional and independently
testable.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Validate the full stack and record the final run flow without
expanding scope.

- [ ] T023 [P] Run the backend automated checks for search in `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`, `tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj`, and `tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj`
- [ ] T024 [P] Run frontend validation with `npm run lint` and `npm run build` from `src/frontend/blog-web/`
- [ ] T025 Validate anonymous and authenticated search through `docker compose up -d postgres api frontend` using the steps in `specs/015-search-posts/quickstart.md`
- [ ] T026 Update the search run-flow documentation in `specs/015-search-posts/quickstart.md`, `README.md`, and `src/frontend/blog-web/README.md` if implementation details or validation notes changed
- [ ] T027 Verify Swagger/OpenAPI output and direct API search behavior for `GET /api/posts?q=...` from `src/backend/BlogPlatform.Api/Controllers/PublicPostsController.cs` and the running `/swagger` document
- [ ] T028 Review the backend and frontend search changes in `src/backend/` and `src/frontend/blog-web/src/features/posts/` and remove any unnecessary abstractions or out-of-scope additions

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup starts first
- Foundational Backend depends on Setup and blocks story implementation
- User Story 1 tests come before any backend production code
- Backend Application, repository, and API changes must pass before frontend
  search wiring begins
- Final Polish depends on User Story 1 being complete

### Within User Story 1

- Application, repository, and API tests MUST be written and fail before search
  implementation begins
- Application visibility logic precedes controller wiring
- Repository SQL remains isolated to `PostgreSqlPostRepository`
- Frontend search updates wait for stable backend contracts and passing backend
  tests
- Frontend changes must preserve the current `DESIGN.md`-aligned UI rather than
  redesign the search experience

### Parallel Opportunities

- `T001` and `T002` can run in parallel
- `T005` through `T010` can run in parallel once `T003` and `T004` are done
- `T018` and `T020` can run in parallel after `T017`
- `T023` and `T024` can run in parallel after story implementation stabilizes

---

## Implementation Strategy

### MVP First

1. Complete Setup
2. Complete Foundational Backend contract changes
3. Deliver User Story 1 with failing backend tests first
4. Validate backend search before touching frontend filtering behavior
5. Wire the frontend to backend results and finish with full-stack validation

### Incremental Delivery

1. Prepare shared search test support
2. Lock down anonymous and authenticated search rules in tests
3. Implement the Application handler and repository query
4. Expose the search-enabled public list endpoint
5. Replace browser-only filtering with backend requests
6. Validate the feature end to end and update docs

## Notes

- `[P]` tasks indicate work that can proceed in parallel across different files
- No Redis, autocomplete, ranking, or external search infrastructure is part of
  this task list
