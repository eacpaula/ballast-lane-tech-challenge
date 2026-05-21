# Tasks: Expose Backend API Endpoints for Existing Application Use Cases

**Input**: Design documents from `/specs/010-api-endpoints/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`

**Tests**: API integration tests are mandatory for this slice. Each API story
starts with failing endpoint tests before controller and composition code is
added.

**Organization**: Tasks are grouped by API user story so each endpoint area
can be implemented and validated independently while shared configuration is
completed first.

## Phase 1: Setup

**Purpose**: Prepare API project dependencies, settings, and test project
baseline for the new controller layer.

- [X] T001 Update API package dependencies for controllers, authentication, authorization, and OpenAPI support in `src/backend/BlogPlatform.Api/BlogPlatform.Api.csproj`
- [X] T002 Update API test project dependencies for ASP.NET Core integration testing in `tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj`
- [X] T003 Create the API test support workspace in `tests/backend/BlogPlatform.Api.Tests/TestSupport/.gitkeep` and `tests/backend/BlogPlatform.Api.Tests/README.md`
- [X] T004 Add JWT and database configuration placeholders for the API layer in `src/backend/BlogPlatform.Api/appsettings.json`, `src/backend/BlogPlatform.Api/appsettings.Development.json`, and `.env.example`

---

## Phase 2: Foundational API Composition

**Purpose**: Add shared API composition, auth, error handling, and test-host
infrastructure that blocks all story work.

- [X] T005 Create API JWT settings binding model in `src/backend/BlogPlatform.Api/Configuration/JwtAuthenticationSettings.cs`
- [X] T006 [P] Create API dependency-registration helpers for Application and Infrastructure services in `src/backend/BlogPlatform.Api/Extensions/ServiceCollectionExtensions.cs`
- [X] T007 [P] Create Infrastructure auth-support services for password hashing and authentication payload creation if they are not already present in `src/backend/BlogPlatform.Infrastructure/Security/PostgreSqlPasswordSecurityService.cs` and `src/backend/BlogPlatform.Infrastructure/Security/JwtAuthenticationPayloadFactory.cs`
- [X] T008 Create consistent ProblemDetails-style error mapping support in `src/backend/BlogPlatform.Api/Errors/ProblemDetailsFactoryExtensions.cs`
- [X] T009 Create API authentication and authorization configuration in `src/backend/BlogPlatform.Api/Extensions/AuthenticationExtensions.cs`
- [X] T010 Create API integration test host bootstrap and seeded-auth helpers in `tests/backend/BlogPlatform.Api.Tests/TestSupport/BlogPlatformApiFactory.cs` and `tests/backend/BlogPlatform.Api.Tests/TestSupport/ApiAuthenticationTestHelper.cs`
- [X] T011 Update `src/backend/BlogPlatform.Api/Program.cs` to register controllers, OpenAPI, auth, error handling, and the shared composition path

**Checkpoint**: The API project has a reusable composition path, JWT/auth
settings model, ProblemDetails mapping strategy, and an integration-test host
ready for story-level endpoint tests.

---

## Phase 3: User Story 1 - Visitor registers and logs in through the API (Priority: P1) 🎯 MVP

**Goal**: Expose registration and login through HTTP with explicit DTOs and
consistent success and failure responses.

**Independent Test**: `dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj --filter AuthController` passes for successful registration, duplicate/invalid registration, successful login, and invalid login.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T012 [US1] Add failing API tests for successful registration in `tests/backend/BlogPlatform.Api.Tests/Auth/AuthRegisterSuccessTests.cs`
- [X] T013 [P] [US1] Add failing API tests for invalid or duplicate registration in `tests/backend/BlogPlatform.Api.Tests/Auth/AuthRegisterFailureTests.cs`
- [X] T014 [P] [US1] Add failing API tests for successful login and invalid login in `tests/backend/BlogPlatform.Api.Tests/Auth/AuthLoginTests.cs`

### Implementation for User Story 1

- [X] T015 [P] [US1] Create auth request and response DTOs in `src/backend/BlogPlatform.Api/Contracts/Auth/RegisterUserRequest.cs`, `src/backend/BlogPlatform.Api/Contracts/Auth/LoginUserRequest.cs`, and `src/backend/BlogPlatform.Api/Contracts/Auth/AuthResponse.cs`
- [X] T016 [US1] Implement the auth controller in `src/backend/BlogPlatform.Api/Controllers/AuthController.cs`
- [X] T017 [US1] Re-run the auth API tests and stabilize success, validation, and conflict response mapping in `src/backend/BlogPlatform.Api/Controllers/AuthController.cs` and `src/backend/BlogPlatform.Api/Errors/ProblemDetailsFactoryExtensions.cs`

**Checkpoint**: Auth registration and login are exposed through HTTP with
stable DTOs and ProblemDetails-style failures.

---

## Phase 4: User Story 2 - Public visitor reads and reacts to public posts (Priority: P2)

**Goal**: Expose anonymous public post listing, detail reads, and like/dislike
reactions through HTTP without requiring authentication.

**Independent Test**: `dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj --filter PublicPosts` and `--filter PostReactions` pass for anonymous reads, valid reactions, invalid reaction type, and unavailable-post rejection.

### Tests for User Story 2 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T018 [US2] Add failing API tests for anonymous public post listing in `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostListTests.cs`
- [X] T019 [P] [US2] Add failing API tests for public post detail success and unavailable/not-found rejection in `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostDetailTests.cs`
- [X] T020 [P] [US2] Add failing API tests for valid like and dislike reactions in `tests/backend/BlogPlatform.Api.Tests/Reactions/PublicPostReactionSuccessTests.cs`
- [X] T021 [P] [US2] Add failing API tests for invalid reaction type and unavailable-post rejection in `tests/backend/BlogPlatform.Api.Tests/Reactions/PublicPostReactionFailureTests.cs`

### Implementation for User Story 2

- [X] T022 [P] [US2] Create public post and reaction DTOs in `src/backend/BlogPlatform.Api/Contracts/Posts/PublicPostSummaryResponse.cs`, `src/backend/BlogPlatform.Api/Contracts/Posts/PublicPostDetailResponse.cs`, `src/backend/BlogPlatform.Api/Contracts/Reactions/ReactToPostRequest.cs`, and `src/backend/BlogPlatform.Api/Contracts/Reactions/ReactionResponse.cs`
- [X] T023 [P] [US2] Implement the public posts controller in `src/backend/BlogPlatform.Api/Controllers/PublicPostsController.cs`
- [X] T024 [P] [US2] Implement the public post reactions controller in `src/backend/BlogPlatform.Api/Controllers/PostReactionsController.cs`
- [X] T025 [US2] Re-run the public API tests and stabilize anonymous access plus not-found/error response behavior in `src/backend/BlogPlatform.Api/Controllers/PublicPostsController.cs`, `src/backend/BlogPlatform.Api/Controllers/PostReactionsController.cs`, and `src/backend/BlogPlatform.Api/Errors/ProblemDetailsFactoryExtensions.cs`

**Checkpoint**: Public read and reaction flows are available anonymously and
return stable success and failure contracts.

---

## Phase 5: User Story 3 - Authenticated user manages their own posts through the API (Priority: P3)

**Goal**: Expose post creation, edit, and removal through protected endpoints
that rely on JWT auth and the existing ownership rules.

**Independent Test**: `dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj --filter ProtectedPosts` passes for unauthorized rejection, authenticated create/update/delete success, and non-owner rejection.

### Tests for User Story 3 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T026 [US3] Add failing API tests for unauthorized access to protected post endpoints in `tests/backend/BlogPlatform.Api.Tests/Posts/ProtectedPostUnauthorizedTests.cs`
- [X] T027 [P] [US3] Add failing API tests for successful authenticated post creation in `tests/backend/BlogPlatform.Api.Tests/Posts/CreatePostApiTests.cs`
- [X] T028 [P] [US3] Add failing API tests for successful owned-post update and non-owner rejection in `tests/backend/BlogPlatform.Api.Tests/Posts/UpdatePostApiTests.cs`
- [X] T029 [P] [US3] Add failing API tests for successful owned-post removal and non-owner rejection in `tests/backend/BlogPlatform.Api.Tests/Posts/DeletePostApiTests.cs`

### Implementation for User Story 3

- [X] T030 [P] [US3] Create protected post request and response DTOs in `src/backend/BlogPlatform.Api/Contracts/Posts/CreatePostRequest.cs`, `src/backend/BlogPlatform.Api/Contracts/Posts/UpdatePostRequest.cs`, and `src/backend/BlogPlatform.Api/Contracts/Posts/PostMutationResponse.cs`
- [X] T031 [US3] Implement the protected posts controller in `src/backend/BlogPlatform.Api/Controllers/PostsController.cs`
- [X] T032 [US3] Re-run the protected post API tests and stabilize authenticated-user claim mapping plus forbidden/unauthorized responses in `src/backend/BlogPlatform.Api/Controllers/PostsController.cs` and `src/backend/BlogPlatform.Api/Extensions/AuthenticationExtensions.cs`

**Checkpoint**: Protected post mutation endpoints require authentication and
preserve the existing ownership behavior through HTTP.

---

## Phase 6: User Story 4 - Administrator manages categories through the API (Priority: P4)

**Goal**: Expose administrator-only category management endpoints with stable
admin success, non-admin rejection, and unauthenticated rejection behavior.

**Independent Test**: `dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj --filter CategoriesController` passes for admin create/update/deactivate, unauthenticated rejection, and non-admin rejection.

### Tests for User Story 4 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [X] T033 [US4] Add failing API tests for unauthorized and non-admin rejection on category endpoints in `tests/backend/BlogPlatform.Api.Tests/Categories/CategoryAuthorizationApiTests.cs`
- [X] T034 [P] [US4] Add failing API tests for successful admin category creation and duplicate-title conflict in `tests/backend/BlogPlatform.Api.Tests/Categories/CreateCategoryApiTests.cs`
- [X] T035 [P] [US4] Add failing API tests for successful admin category update and not-found behavior in `tests/backend/BlogPlatform.Api.Tests/Categories/UpdateCategoryApiTests.cs`
- [X] T036 [P] [US4] Add failing API tests for successful admin category deactivation in `tests/backend/BlogPlatform.Api.Tests/Categories/DeactivateCategoryApiTests.cs`

### Implementation for User Story 4

- [X] T037 [P] [US4] Create category request and response DTOs in `src/backend/BlogPlatform.Api/Contracts/Categories/CreateCategoryRequest.cs`, `src/backend/BlogPlatform.Api/Contracts/Categories/UpdateCategoryRequest.cs`, and `src/backend/BlogPlatform.Api/Contracts/Categories/CategoryResponse.cs`
- [X] T038 [US4] Implement the categories controller in `src/backend/BlogPlatform.Api/Controllers/CategoriesController.cs`
- [X] T039 [US4] Re-run the category API tests and stabilize admin-role authorization plus conflict/not-found responses in `src/backend/BlogPlatform.Api/Controllers/CategoriesController.cs` and `src/backend/BlogPlatform.Api/Extensions/AuthenticationExtensions.cs`

**Checkpoint**: Administrator-only category management is available through
HTTP with correct auth and error behavior.

---

## Final Phase: Polish & Cross-Cutting

**Purpose**: Finalize API documentation, demo flow guidance, and end-to-end
validation without expanding scope.

- [X] T040 [P] Update OpenAPI configuration and local API exploration notes in `src/backend/BlogPlatform.Api/Program.cs`, `src/backend/BlogPlatform.Api/BlogPlatform.Api.http`, and `README.md`
- [X] T041 [P] Document API run instructions, demo credentials, Swagger URL, and the basic demo flow in `README.md` and `docs/Database-Implementation-Strategy.md`
- [X] T042 Re-run the full API, Application, and Infrastructure backend test suites through `tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj`, `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`, and `tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj`
- [X] T043 Validate the local Swagger/OpenAPI surface and basic manual demo flow through `src/backend/BlogPlatform.Api/Program.cs` and `src/backend/BlogPlatform.Api/BlogPlatform.Api.http`
- [X] T044 Review the API slice for thin-controller compliance and confirm no out-of-scope features were introduced in `src/backend/BlogPlatform.Api` and `specs/010-api-endpoints/tasks.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 starts first.
- Phase 2 depends on Phase 1 and blocks all story work.
- Phase 3 depends on Phase 2.
- Phase 4 depends on Phase 2 and can proceed after the shared test host and
  error/auth configuration exist.
- Phase 5 depends on Phases 2 and 3 because protected post flows need working
  authenticated user handling.
- Phase 6 depends on Phases 2 and 3 because administrator endpoints need
  working auth and role claims.
- Final Phase depends on the selected user stories being complete.

### User Story Dependencies

- `US1` is the MVP slice and should be completed first.
- `US2` depends only on the shared API host and can proceed after Phase 2.
- `US3` depends on `US1` because protected flows require authenticated user
  handling and token-bearing login behavior.
- `US4` depends on `US1` because administrator flows require authenticated role
  handling.

### Within Each User Story

- API integration tests MUST be written and fail before controller or DTO
  implementation.
- DTOs SHOULD be added before controller actions that consume or return them.
- Controller actions remain thin and must delegate business behavior to
  Application handlers.
- Error mapping and auth configuration should be stabilized after the story’s
  endpoint tests are running.

### Parallel Opportunities

- `T006` and `T007` can run in parallel after `T005`.
- `T013` and `T014` can run in parallel after `T012`.
- `T019`-`T021` can run in parallel after `T018`.
- `T022`-`T024` can run in parallel once the public endpoint test files exist.
- `T027`-`T029` can run in parallel after `T026`.
- `T034`-`T036` can run in parallel after `T033`.
- `T040` and `T041` can run in parallel after the controllers are stable.

---

## Implementation Strategy

### MVP First

1. Complete Setup.
2. Complete Foundational API Composition.
3. Deliver `US1` with failing API tests first.
4. Validate auth API behavior independently before exposing public, protected,
   and admin endpoints.

### Incremental Delivery

1. Establish API composition, auth, ProblemDetails, and integration-test host
   support.
2. Add auth endpoints and validate registration/login behavior.
3. Add public post and reaction endpoints next.
4. Add protected post mutation endpoints after authenticated API behavior is
   stable.
5. Add administrator category endpoints last.
6. Finish with Swagger/demo validation and documentation updates.

## Notes

- `[P]` tasks indicate parallelizable work across different files.
- `US1` through `US4` are derived from the endpoint areas in scope because the
  API spec is organized by exposed use-case groups rather than original
  product-user stories.
- No frontend, refresh-token, external identity, EF, Dapper, MediatR, or API
  versioning tasks are included.
