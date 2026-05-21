# Implementation Plan: Frontend MVP

**Branch**: `main` | **Date**: 2026-05-21 | **Spec**:
`specs/014-frontend-mvp/spec.md`

**Input**: Feature specification from `/specs/014-frontend-mvp/spec.md`

**Note**: This plan focuses on a small, interview-ready React MVP that consumes
the existing ASP.NET Core API, while sequencing any newly discovered backend
read gaps ahead of dependent UI work.

## Summary

Build the first real frontend flow for the blog platform on top of the current
API, using simple React patterns, centralized Tailwind tokens, and a
feature-based structure.

- Add client-side routing, an application shell, and session-aware navigation.
- Create a shared API client and lightweight auth/session layer using the
  existing environment-based API URL.
- Implement public browsing, auth, author post management, and admin category
  management with consistent loading, empty, success, and error handling.
- Keep state local to features or shared through a small context layer rather
  than introducing Redux or another large state library.
- Sequence a small backend-first prerequisite slice before dependent frontend
  work for the currently missing author/admin read endpoints.

## Implementation Design

**1. Technical approach**:

- Treat the frontend MVP as a browser client for the existing API, not as a new
  domain layer.
- Prefer built-in browser and React primitives first: `fetch`, local storage,
  context, hooks, and route composition.
- Add only the minimum new frontend runtime dependencies required for routing.
- Keep backend business rules in the API and translate backend errors into
  understandable UI feedback rather than recreating rules client-side.

**2. Files and folders affected**:

- `src/frontend/blog-web/package.json`
- `src/frontend/blog-web/src/main.tsx`
- `src/frontend/blog-web/src/App.tsx`
- `src/frontend/blog-web/src/index.css`
- `src/frontend/blog-web/src/styles/components.css`
- new frontend folders under `src/frontend/blog-web/src/`:
  - `app/` for router, shell, providers, and app bootstrap helpers
  - `features/auth/`
  - `features/posts/`
  - `features/categories/`
  - `components/` for shared UI primitives and feedback blocks
  - `lib/api/` for the shared HTTP client and endpoint modules
  - `lib/session/` for token, role, and visitor-identifier helpers
- `src/frontend/blog-web/README.md`
- `README.md` only if run-flow notes need expansion
- backend files only if the prerequisite API gaps are confirmed and scheduled as
  a supporting slice before dependent UI implementation

**3. Routing strategy**:

- Use `react-router-dom` with a top-level shell route and nested page routes.
- Split routes into public, authenticated, and administrator-only groups.
- Keep guards simple:
  - public routes always render
  - authenticated routes require a valid stored session
  - admin routes require an authenticated session with an administrator role
- Include a not-found route and redirect users away from invalid or expired
  protected flows into login.

**4. API client strategy**:

- Use one shared HTTP client wrapper around `fetch`, reading the base URL from
  the existing frontend environment configuration.
- Keep endpoint functions explicit and grouped by feature:
  - auth
  - public posts
  - post mutations
  - reactions
  - categories
- Normalize JSON parsing, bearer token attachment, and ProblemDetails handling
  in one place so pages stay thin.
- Avoid Axios, generated clients, or broader data libraries unless the MVP
  clearly proves they are necessary.

**5. Authentication/session strategy**:

- Store the JWT in local storage together with the minimum user profile fields
  returned by login.
- Decode the JWT payload locally for role-aware navigation and route guards only
  after login or session restoration.
- Keep registration as a separate success flow that redirects the user to login,
  since the current backend registration endpoint does not return a token.
- Clear the session immediately on logout and on unrecoverable unauthorized
  responses.

**6. Component strategy**:

- Reuse the existing Tailwind shell primitives and extend them with small shared
  components such as buttons, fields, cards, banners, page headers, and empty
  states.
- Prefer composition over large generic abstractions.
- Keep feature components close to their route or use-case folder and reserve
  `components/` for primitives that are reused across multiple features.
- Ensure forms, buttons, cards, tables or lists, and action bars remain
  visually aligned with `DESIGN.md`.

**7. Public blog pages strategy**:

- Landing page: list public posts with readable summaries and empty-state
  fallback.
- Public post page: render the post detail, reaction controls, and mutation
  status feedback for like/dislike actions.
- Use a simple visitor identifier helper stored locally for anonymous reaction
  requests.
- Keep public pages fully usable without authentication.

**8. Auth pages strategy**:

- Registration page: collect name, email, and password, show validation or
  conflict errors, and redirect to login on success.
- Login page: collect email and password, store the session on success, and
  redirect to the default post-auth destination.
- Session-aware navigation should immediately update after login, logout, or
  refresh restoration.

**9. Authenticated post management strategy**:

- My Posts page depends on a protected list endpoint for the authenticated
  author. That endpoint is not currently present and must be implemented first.
- Create Post page can proceed after the existing available-categories endpoint
  and protected create endpoint are wired through the frontend.
- Edit Post page depends on a protected read endpoint for the author’s own post
  details, because the public detail endpoint is insufficient for unavailable or
  non-public owned posts.
- Remove Post action can be exposed from My Posts and edit contexts once the
  protected author reads exist.

**10. Admin category management strategy**:

- Category create, update, and deactivate mutations already exist, but an
  admin-facing list endpoint is currently missing for management UX.
- The admin category page therefore requires a small backend-first addition to
  list all categories, including unavailable ones.
- Once that list exists, keep the page simple: category list, inline or modal
  form entry, and deactivate action feedback.

**11. Error/loading/empty state strategy**:

- Standardize one lightweight request-state pattern across features:
  initial loading, settled success, empty, recoverable error, and submitting.
- Map ProblemDetails responses to field-level or page-level messages depending
  on the failure shape.
- Keep success feedback explicit for mutations that do not visibly rerender the
  whole page.
- Show a recoverable auth-expired path for protected requests that return
  unauthorized responses.

**12. Styling strategy based on `DESIGN.md`**:

- Reuse the existing Tailwind token setup as the single source of styling
  primitives.
- Build pages from the established typography, spacing, container, radius,
  surface, and shadow tokens instead of one-off values.
- Keep the UI restrained, professional, and technical rather than decorative.
- Accessibility remains part of the implementation baseline:
  semantic headings, visible focus states, readable contrast, keyboard-reachable
  actions, and mobile-safe layouts.

**13. Validation strategy**:

- Backend-first prerequisite changes must be proven through failing and then
  passing backend API tests before dependent frontend pages are built.
- Frontend validation for the MVP should stay lightweight:
  - `npm run lint`
  - `npm run build`
  - manual route and flow validation against the running API
  - Docker Compose validation with frontend, API, and database together
- If a tiny amount of frontend test coverage is added later, it should target
  route guards and shared request helpers first, not a large test framework
  rollout.

**14. Risks and trade-offs**:

- The current backend contract is not yet sufficient for all specified frontend
  pages; the plan absorbs that honestly by sequencing a small backend
  prerequisite slice first.
- Decoding JWT claims client-side is appropriate for route UX but must never be
  treated as real authorization.
- Avoiding a data-fetching library keeps the architecture smaller, but it
  requires disciplined reuse of the shared request-state and error-handling
  helpers.
- Deferring richer testing and state tooling keeps the MVP finishable and
  explainable within interview scope.

## Technical Context

**Language/Version**: TypeScript 6, React 19, Vite 8, TailwindCSS 4,
`react-router-dom` as the planned routing dependency

**Primary Dependencies**: React, React DOM, Vite, TailwindCSS, browser `fetch`,
local storage, existing backend JWT auth and ProblemDetails responses

**Storage**: Browser local storage for JWT session state and anonymous visitor
identifier; PostgreSQL remains backend-only

**Testing**: Existing backend API tests for prerequisite endpoint additions,
frontend lint/build validation, and manual full-stack validation through the
running API and Docker Compose stack

**Target Platform**: Browser-based frontend served by Vite locally and through
the existing Docker Compose runtime

**Project Type**: Full-stack web application with a separate React frontend

**Performance Goals**: Fast enough for interview demo usage with immediate
feedback on navigation, forms, and page-state changes

**Constraints**: No Redux-class state library by default; no redesign outside
`DESIGN.md`; no backend business-rule migration into frontend code; no backend
changes unless required API gaps are confirmed

**Scale/Scope**: Small frontend MVP that demonstrates the complete blog flow
without expanding into full CMS behavior

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: any newly required backend endpoint work is explicitly
  sequenced ahead of frontend implementation and must begin with failing backend
  API tests; frontend validation stays pragmatic and lightweight.
- PASS Backend-first: missing author/admin read endpoints are treated as
  backend prerequisites before dependent UI pages.
- PASS Architecture: frontend work stays inside the React app, backend business
  rules remain in Application/API layers, and Infrastructure is consumed only
  through existing HTTP contracts.
- PASS Data access: no frontend code crosses into raw SQL or persistence
  concerns.
- PASS Security: the plan preserves backend authorization as the source of
  truth and limits client-side role use to route guidance and presentation.
- PASS Scope: the slice delivers the approved MVP only and avoids richer CMS
  behaviors, editor complexity, and large client-side infrastructure.
- PASS API consistency: frontend design assumes existing ProblemDetails and
  explicit DTO contracts; any prerequisite endpoint additions must follow the
  same conventions.
- PASS Frontend governance: the plan reuses centralized Tailwind tokens, keeps
  components reusable, avoids direct Ballast Lane branding, and includes
  accessibility from the start.

## Project Structure

### Documentation (this feature)

```text
specs/014-frontend-mvp/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-mvp-consumption-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── backend/
│   └── BlogPlatform.Api/
└── frontend/
    └── blog-web/
        ├── src/
        │   ├── app/
        │   ├── components/
        │   ├── features/
        │   ├── lib/
        │   └── styles/
        ├── tailwind.config.ts
        ├── vite.config.ts
        └── package.json
```

**Structure Decision**: keep the frontend organized by app shell, shared
primitives, and feature folders. Introduce only the folders needed to separate
routing, API access, session helpers, and feature UIs without creating a
larger framework inside the app.

## Complexity Tracking

No constitution violations are expected for this slice.
