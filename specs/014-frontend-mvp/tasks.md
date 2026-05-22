---

description: "Task list for frontend MVP implementation"

---

# Tasks: Frontend MVP

**Input**: Design documents from `/specs/014-frontend-mvp/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Backend API and repository tests are required for the confirmed
blocking API gaps. Frontend validation uses lint/build checks plus full-stack
manual validation against the running API and Docker Compose stack.

**Organization**: Tasks are grouped by user story to enable independent
implementation and testing of each deliverable path.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (`US1`, `US2`, `US3`)
- Include exact file paths in descriptions

## Path Conventions

- **Backend API**: `src/backend/BlogPlatform.Api/`
- **Backend Application**: `src/backend/BlogPlatform.Application/`
- **Backend Infrastructure**: `src/backend/BlogPlatform.Infrastructure/`
- **Backend tests**: `tests/backend/BlogPlatform.Api.Tests/`,
  `tests/backend/BlogPlatform.Application.Tests/`,
  `tests/backend/BlogPlatform.Infrastructure.Tests/`
- **Frontend app**: `src/frontend/blog-web/src/`
- **Frontend theme/styles**: `src/frontend/blog-web/tailwind.config.ts`,
  `src/frontend/blog-web/src/index.css`,
  `src/frontend/blog-web/src/styles/components.css`
- **Feature docs**: `specs/014-frontend-mvp/`

## Phase 1: Setup

**Purpose**: Create the frontend file structure and minimum runtime dependencies
for all subsequent work.

- [X] T001 Create the feature-based frontend folder structure under `src/frontend/blog-web/src/app`, `src/frontend/blog-web/src/components`, `src/frontend/blog-web/src/features/auth`, `src/frontend/blog-web/src/features/posts`, `src/frontend/blog-web/src/features/categories`, `src/frontend/blog-web/src/lib/api`, and `src/frontend/blog-web/src/lib/session`
- [X] T002 Add the routing dependency and any required type-safe frontend package updates in `src/frontend/blog-web/package.json`
- [X] T003 [P] Create frontend barrel or entry files for the new folders in `src/frontend/blog-web/src/app`, `src/frontend/blog-web/src/components`, `src/frontend/blog-web/src/features/auth`, `src/frontend/blog-web/src/features/posts`, `src/frontend/blog-web/src/features/categories`, `src/frontend/blog-web/src/lib/api`, and `src/frontend/blog-web/src/lib/session`
- [X] T004 [P] Review and trim the current starter `src/frontend/blog-web/src/App.tsx` usage so it can become a router host instead of a static validation shell

---

## Phase 2: Foundational Frontend

**Purpose**: Establish shared routing, API, session, and reusable UI primitives
that all user stories depend on.

**CRITICAL**: User story implementation should not begin until this phase is
complete.

- [X] T005 Configure the application bootstrap and router provider in `src/frontend/blog-web/src/main.tsx` and `src/frontend/blog-web/src/app/router.tsx`
- [X] T006 Create the top-level application shell, layout container, and session-aware navigation in `src/frontend/blog-web/src/app/AppShell.tsx` and `src/frontend/blog-web/src/App.tsx`
- [X] T007 Create the shared API base URL configuration and typed request helper in `src/frontend/blog-web/src/lib/api/config.ts`, `src/frontend/blog-web/src/lib/api/http.ts`, and `src/frontend/blog-web/src/lib/api/problem-details.ts`
- [X] T008 Create frontend session storage, JWT decoding, and visitor identifier helpers in `src/frontend/blog-web/src/lib/session/session-storage.ts`, `src/frontend/blog-web/src/lib/session/jwt.ts`, and `src/frontend/blog-web/src/lib/session/visitor-id.ts`
- [X] T009 Create the auth context and session restoration provider in `src/frontend/blog-web/src/features/auth/AuthProvider.tsx` and `src/frontend/blog-web/src/features/auth/useAuth.ts`
- [X] T010 Create authenticated and administrator route guards in `src/frontend/blog-web/src/app/AuthenticatedRoute.tsx` and `src/frontend/blog-web/src/app/AdministratorRoute.tsx`
- [X] T011 [P] Create shared button, card, badge, and form-action primitives in `src/frontend/blog-web/src/components/Button.tsx`, `src/frontend/blog-web/src/components/Card.tsx`, `src/frontend/blog-web/src/components/Badge.tsx`, and `src/frontend/blog-web/src/components/FormActions.tsx`
- [X] T012 [P] Create shared text input, textarea, select, and field-feedback primitives in `src/frontend/blog-web/src/components/TextInput.tsx`, `src/frontend/blog-web/src/components/Textarea.tsx`, `src/frontend/blog-web/src/components/Select.tsx`, and `src/frontend/blog-web/src/components/FieldMessage.tsx`
- [X] T013 [P] Create shared loading, empty, error, and confirmation-action primitives in `src/frontend/blog-web/src/components/LoadingState.tsx`, `src/frontend/blog-web/src/components/EmptyState.tsx`, `src/frontend/blog-web/src/components/ErrorMessage.tsx`, and `src/frontend/blog-web/src/components/ConfirmAction.tsx`
- [X] T014 Update centralized Tailwind-backed shared styling for the application shell and reusable primitives in `src/frontend/blog-web/src/index.css` and `src/frontend/blog-web/src/styles/components.css`

**Checkpoint**: Routing, API access, session handling, and shared UI primitives
are ready for story-level work.

---

## Phase 3: User Story 1 - Public Browsing and Authentication Entry (Priority: P1) 🎯 MVP

**Goal**: Let a visitor browse public posts, react to a post, register, log in,
and see session-aware navigation without needing any new backend endpoints.

**Independent Test**: Start the stack, open the frontend, verify public post
listing and detail pages load, submit a like or dislike reaction, register a
user, log in, and confirm the navigation updates to the authenticated state.

### Implementation for User Story 1

- [X] T015 [P] [US1] Create auth API modules and typed contracts in `src/frontend/blog-web/src/features/auth/auth.api.ts` and `src/frontend/blog-web/src/features/auth/auth.types.ts`
- [X] T016 [P] [US1] Create public post and reaction API modules plus typed contracts in `src/frontend/blog-web/src/features/posts/public-posts.api.ts`, `src/frontend/blog-web/src/features/posts/reactions.api.ts`, and `src/frontend/blog-web/src/features/posts/post.types.ts`
- [X] T017 [P] [US1] Create the reusable post card and reaction controls in `src/frontend/blog-web/src/features/posts/PostCard.tsx` and `src/frontend/blog-web/src/features/posts/ReactionControls.tsx`
- [X] T018 [US1] Implement the public post listing route and empty/loading/error states in `src/frontend/blog-web/src/features/posts/PublicPostListPage.tsx`
- [X] T019 [US1] Implement the public post detail route and reaction mutation feedback in `src/frontend/blog-web/src/features/posts/PublicPostDetailPage.tsx`
- [X] T020 [US1] Implement the registration page and API validation/conflict feedback in `src/frontend/blog-web/src/features/auth/RegisterPage.tsx`
- [X] T021 [US1] Implement the login page, token persistence, session restoration, and logout action in `src/frontend/blog-web/src/features/auth/LoginPage.tsx`, `src/frontend/blog-web/src/features/auth/AuthProvider.tsx`, and `src/frontend/blog-web/src/app/AppShell.tsx`
- [X] T022 [US1] Register the public, login, registration, and not-found routes in `src/frontend/blog-web/src/app/router.tsx`
- [X] T023 [US1] Verify keyboard focus, semantic HTML, and responsive behavior for the public and auth pages in `src/frontend/blog-web/src/features/posts/` and `src/frontend/blog-web/src/features/auth/`

**Checkpoint**: Anonymous and entry authentication flows are independently
functional.

---

## Phase 4: User Story 2 - Authenticated Author Post Management (Priority: P2)

**Goal**: Let an authenticated user access My Posts, create a post, edit an
owned post, and remove an owned post using backend-supported ownership rules.

**Independent Test**: After the prerequisite backend read endpoints are added,
log in as a regular user, open My Posts, create a post, edit that post, remove
that post, and confirm anonymous users cannot access the protected routes.

### Backend Prerequisite Tests for User Story 2 (MANDATORY) ⚠️

> Write these tests first and verify they fail before backend production code.

- [X] T024 [P] [US2] Add Application tests for owned-post listing and owned-post detail handlers in `tests/backend/BlogPlatform.Application.Tests/Posts/ListOwnedPostsHandlerTests.cs` and `tests/backend/BlogPlatform.Application.Tests/Posts/GetOwnedPostByIdHandlerTests.cs`
- [X] T025 [P] [US2] Add API integration tests for protected owned-post reads in `tests/backend/BlogPlatform.Api.Tests/Posts/OwnedPostListApiTests.cs` and `tests/backend/BlogPlatform.Api.Tests/Posts/OwnedPostDetailApiTests.cs`
- [X] T026 [P] [US2] Add repository tests for author-scoped post reads in `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryOwnedReadTests.cs`

### Backend Prerequisite Implementation for User Story 2

- [X] T027 [US2] Extend the post repository contract for author-scoped reads in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- [X] T028 [US2] Implement owned-post list and owned-post detail handlers plus result models in `src/backend/BlogPlatform.Application/Posts/ListOwnedPostsHandler.cs`, `src/backend/BlogPlatform.Application/Posts/OwnedPostListItem.cs`, `src/backend/BlogPlatform.Application/Posts/GetOwnedPostByIdHandler.cs`, and `src/backend/BlogPlatform.Application/Posts/GetOwnedPostByIdResult.cs`
- [X] T029 [US2] Implement parameterized SQL for author-scoped post reads in `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
- [X] T030 [US2] Add protected owned-post read DTOs and endpoints in `src/backend/BlogPlatform.Api/Contracts/Posts/OwnedPostSummaryResponse.cs`, `src/backend/BlogPlatform.Api/Contracts/Posts/OwnedPostDetailResponse.cs`, and `src/backend/BlogPlatform.Api/Controllers/PostsController.cs`
- [X] T031 [US2] Re-run backend tests covering owned-post authorization, not-found, and ownership behavior in `tests/backend/BlogPlatform.Application.Tests/Posts/`, `tests/backend/BlogPlatform.Api.Tests/Posts/`, and `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/`

### Frontend Implementation for User Story 2

- [X] T032 [P] [US2] Create authenticated post API modules and typed contracts in `src/frontend/blog-web/src/features/posts/owned-posts.api.ts` and `src/frontend/blog-web/src/features/posts/post.types.ts`
- [X] T033 [P] [US2] Create the reusable post form and delete/remove confirmation flow in `src/frontend/blog-web/src/features/posts/PostForm.tsx` and `src/frontend/blog-web/src/features/posts/RemovePostAction.tsx`
- [X] T034 [US2] Implement the My Posts page with loading, empty, error, and remove-action states in `src/frontend/blog-web/src/features/posts/MyPostsPage.tsx`
- [X] T035 [US2] Implement the create post page and category loading flow in `src/frontend/blog-web/src/features/posts/CreatePostPage.tsx`
- [X] T036 [US2] Implement the edit post page with owned-post loading and API validation handling in `src/frontend/blog-web/src/features/posts/EditPostPage.tsx`
- [X] T037 [US2] Register authenticated author routes and route-guard redirects in `src/frontend/blog-web/src/app/router.tsx`
- [X] T038 [US2] Verify protected-route access, form error handling, responsive layout, and keyboard accessibility for author pages in `src/frontend/blog-web/src/features/posts/` and `src/frontend/blog-web/src/app/`

**Checkpoint**: Authenticated author post management is independently
functional.

---

## Phase 5: User Story 3 - Administrator Category Management (Priority: P3)

**Goal**: Let an administrator list, create, update, and deactivate categories
from a dedicated management page while non-admin users remain blocked.

**Independent Test**: After the prerequisite admin category list endpoint is
added, log in as the seeded administrator, open category management, create a
category, update it, deactivate it, and confirm non-admin users are denied.

### Backend Prerequisite Tests for User Story 3 (MANDATORY) ⚠️

- [X] T039 [P] [US3] Add Application tests for administrator category listing in `tests/backend/BlogPlatform.Application.Tests/Categories/ListAllCategoriesHandlerTests.cs`
- [X] T040 [P] [US3] Add API integration tests for administrator category listing and access control in `tests/backend/BlogPlatform.Api.Tests/Categories/AdminCategoryListApiTests.cs`
- [X] T041 [P] [US3] Add repository tests for full category listing in `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/CategoryRepositoryAdminListTests.cs`

### Backend Prerequisite Implementation for User Story 3

- [X] T042 [US3] Extend the category repository contract for administrator listing in `src/backend/BlogPlatform.Application/Abstractions/ICategoryRepository.cs`
- [X] T043 [US3] Implement the administrator category list handler and result model in `src/backend/BlogPlatform.Application/Posts/ListAllPostCategoriesHandler.cs` and `src/backend/BlogPlatform.Application/Posts/ManagedPostCategoryListItem.cs`
- [X] T044 [US3] Implement parameterized SQL for full category listing in `src/backend/BlogPlatform.Infrastructure/Categories/PostgreSqlCategoryRepository.cs`
- [X] T045 [US3] Add the administrator category list endpoint and DTO in `src/backend/BlogPlatform.Api/Contracts/Categories/AdminCategoryListItemResponse.cs` and `src/backend/BlogPlatform.Api/Controllers/CategoriesController.cs`
- [X] T046 [US3] Re-run backend tests covering administrator category read authorization and unavailable-category visibility in `tests/backend/BlogPlatform.Application.Tests/Categories/`, `tests/backend/BlogPlatform.Api.Tests/Categories/`, and `tests/backend/BlogPlatform.Infrastructure.Tests/Categories/`

### Frontend Implementation for User Story 3

- [X] T047 [P] [US3] Create administrator category API modules and typed contracts in `src/frontend/blog-web/src/features/categories/categories.api.ts` and `src/frontend/blog-web/src/features/categories/category.types.ts`
- [X] T048 [P] [US3] Create the reusable category form and category row or card components in `src/frontend/blog-web/src/features/categories/CategoryForm.tsx` and `src/frontend/blog-web/src/features/categories/CategoryListItem.tsx`
- [X] T049 [US3] Implement the administrator category management page with list, create, update, and deactivate flows in `src/frontend/blog-web/src/features/categories/AdminCategoriesPage.tsx`
- [X] T050 [US3] Register the administrator-only category route and navigation state in `src/frontend/blog-web/src/app/router.tsx` and `src/frontend/blog-web/src/app/AppShell.tsx`
- [X] T051 [US3] Verify administrator route protection, forbidden-state messaging, responsive layout, and keyboard accessibility for category management in `src/frontend/blog-web/src/features/categories/` and `src/frontend/blog-web/src/app/`

**Checkpoint**: Administrator category management is independently functional.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Validate the full stack, document the demo flow, and confirm the
slice stayed within scope.

- [X] T052 [P] Run frontend quality checks with `npm run lint` and `npm run build` from `src/frontend/blog-web/`
- [X] T053 [P] Validate the frontend against the running local stack with `docker compose up -d postgres api frontend` and browser/API checks from `specs/014-frontend-mvp/quickstart.md`
- [X] T054 Validate public, auth, author, reaction, and admin flows end to end and record follow-up notes in `specs/014-frontend-mvp/quickstart.md`
- [X] T055 Check the browser console for errors or warnings while exercising the MVP routes in `src/frontend/blog-web/src/`
- [X] T056 Verify desktop and mobile-width layout behavior across the main routes in `src/frontend/blog-web/src/`
- [X] T057 Update the frontend and root run-flow documentation, routes, demo credentials, and known limitations in `README.md` and `src/frontend/blog-web/README.md`
- [X] T058 Review the frontend structure, shared components, and API helpers in `src/frontend/blog-web/src/` and simplify any unnecessary abstractions
- [X] T059 Confirm no out-of-scope behavior was introduced beyond required backend API gap closure in `specs/014-frontend-mvp/spec.md`, `specs/014-frontend-mvp/plan.md`, and `src/frontend/blog-web/src/`

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup starts first
- Foundational Frontend depends on Setup and blocks all story work
- User Story 1 depends on Foundational Frontend only
- User Story 2 depends on Foundational Frontend and its backend prerequisite
  tests and implementation
- User Story 3 depends on Foundational Frontend and its backend prerequisite
  tests and implementation
- Final Polish depends on the selected user stories being complete

### Within Each User Story

- For `US2` and `US3`, backend prerequisite tests MUST be written and fail
  before backend implementation begins
- Backend prerequisite implementation MUST pass before the dependent frontend
  pages are built
- Frontend route registration follows the feature pages and guards it depends on
- Shared components and style tokens must remain centralized instead of adding
  one-off styles

### Parallel Opportunities

- `T003` and `T004` can run in parallel after `T001`-`T002`
- `T011`-`T013` can run in parallel after `T005`-`T010`
- `T015`-`T017` can run in parallel after the foundational phase
- `T024`-`T026` can run in parallel at the start of `US2`
- `T032` and `T033` can run in parallel after `T031`
- `T039`-`T041` can run in parallel at the start of `US3`
- `T047` and `T048` can run in parallel after `T046`
- `T052` and `T053` can run in parallel after story work is stable

---

## Implementation Strategy

### MVP First

1. Complete Setup
2. Complete Foundational Frontend
3. Deliver `US1` for public browsing, reactions, and auth entry
4. Add the required backend read contracts for `US2`
5. Deliver `US2` for authenticated author workflow
6. Add the required backend read contract for `US3`
7. Deliver `US3` for admin category management

### Incremental Delivery

1. Establish the shared frontend architecture and styling primitives
2. Ship the public and authentication browser flow first
3. Expand into author tooling only after the owned-post read contracts are in
   place
4. Expand into administrator tooling only after the admin category list contract
   is in place
5. Finish with full-stack validation and documentation

## Notes

- The only planned backend changes are the confirmed API gaps required for the
  specified frontend pages
- No Redux-class state library, rich text editor, or broader CMS behavior is
  included in this task list
