# Implementation Plan: Search Posts Through Backend API

**Branch**: `015-search-posts` | **Date**: 2026-05-22 | **Spec**:
`specs/015-search-posts/spec.md`

**Input**: Feature specification from `/specs/015-search-posts/spec.md`

**Note**: This plan keeps the slice intentionally small by reusing the existing
public post listing flow, moving search logic into the backend, and limiting the
frontend to wiring the current search box to server-filtered results.

## Summary

Replace browser-only filtering with backend-powered post search through the
existing listing route.

- Add one application search/list use case that accepts a search term and an
  optional authenticated user id.
- Apply visibility rules in Application: anonymous users see only public and
  available posts; authenticated users also see their own matching non-public
  available posts.
- Extend the post repository abstraction with a search-capable read method and
  implement it with parameterized raw SQL in Infrastructure.
- Update the public posts API endpoint to accept a query parameter while
  remaining thin and compatible with anonymous access.
- Replace client-side filtering in the React public posts page with backend
  requests keyed by the existing `q` query string.
- Keep tags and category matching bounded to what the current schema already
  supports, with no caching, ranking, or external search services.

## Implementation Design

**1. Technical approach**:

- Treat backend search as an extension of the existing post-list read flow, not
  as a new subsystem.
- Keep the route as `GET /api/posts?q=...` so the frontend search form and
  current URL structure remain stable.
- Pass the optional authenticated user id from the API layer into Application
  only when a valid bearer token is present.
- Normalize blank queries before repository execution so empty input behaves the
  same as the default listing path.

**2. Layers affected**:

- Application:
  - update or replace the current public list handler with a search-aware use
    case
  - enforce search visibility and ownership rules
- Infrastructure:
  - extend `IPostRepository`
  - add parameterized raw SQL for text matching plus owner/public filtering
- API:
  - update the public list endpoint to accept a `q` query parameter and optional
    authenticated context
- Frontend:
  - update the public list API module and page behavior to request filtered
    results from the backend instead of filtering locally
- Tests:
  - Application, Infrastructure, API, and practical frontend checks

**3. Application search use case strategy**:

- Prefer evolving `ListPublicPostsHandler` into a search-aware read use case so
  the existing endpoint and DTO shape stay stable.
- New handler input:
  - `query` as nullable or optional string
  - `requestingUserId` as nullable integer
- Handler responsibilities:
  - trim and normalize the incoming query
  - treat null, empty, or whitespace-only queries as the default listing case
  - call the repository with the normalized query and optional user id
  - apply visibility safeguards if the repository returns broader data than
    allowed
  - project results into the existing list-item DTO shape
- Keep category and tag search as a repository concern only for matching, not
  for expanding the response payload.

**4. Search visibility and authorization strategy**:

- Anonymous:
  - only public and available posts may match
- Authenticated:
  - matching public and available posts remain visible
  - matching owned posts may also be returned when they are available but not
    public
  - private or non-public posts owned by other users must never be returned
- Authorization does not move into SQL only:
  - SQL performs filtering efficiently
  - Application remains the owner of the business rule and can still guard the
    result set
- Unavailable posts stay excluded from this feature unless an already-existing
  owner-only rule explicitly says otherwise; this slice should not expand that
  rule set

**5. Repository and raw SQL strategy**:

- Extend `IPostRepository` with one search-oriented read method instead of
  adding multiple narrowly overlapping methods.
- SQL strategy:
  - join `post_categories` so category titles can participate in matching
  - optionally join `post_tags` and `tags` only if that can be done without
    widening the repository contract or result mapping
  - use `ILIKE` or equivalent case-insensitive matching for title, description,
    content, and supported related text fields
  - keep all SQL parameterized
  - group or distinct results if joins introduce duplicate post rows
- Keep the repository return type aligned with existing `BlogPost` rehydration
  so the Application layer does not absorb persistence details

**6. API endpoint strategy**:

- Reuse `GET /api/posts`
- Accept an optional `q` query parameter
- Keep the controller thin:
  - read the optional search term
  - read the optional authenticated user id if present
  - delegate to the Application handler
  - map the existing summary DTOs
- Preserve anonymous access
- Preserve ProblemDetails behavior for malformed requests if any request-shape
  validation is introduced, while keeping blank `q` valid
- Existing public post detail and reaction endpoints remain unchanged

**7. Frontend integration strategy**:

- Keep the current search box in `AppShell` and the `?q=` URL behavior
- Update the public-post listing API helper to send the query value to the
  backend
- Update `PublicPostListPage` to:
  - request backend results whenever `q` changes
  - stop filtering the already-loaded list in memory
  - keep the same empty-results messaging and visual layout
- If authenticated, allow the request to include the bearer token so the backend
  can include owned private matches
- Avoid new client-side state libraries or caching layers

**8. Loading, error, and empty state strategy**:

- Preserve the current page states:
  - loading when the route first loads or when the query changes
  - error if the backend search request fails
  - empty default state when there are no public posts at all
  - no-results state when a non-empty query returns zero matches
- Keep the search experience simple:
  - no autocomplete
  - no advanced debounce requirement
  - no optimistic UI
- Maintain existing accessibility expectations for the search form and result
  announcements

**9. Testing strategy**:

- Application tests first:
  - blank query behaves like default listing
  - anonymous users receive only public and available matches
  - authenticated users receive public matches plus owned private matches
  - private posts owned by other users are excluded
- Repository tests:
  - case-insensitive matching works
  - category matching works
  - tag matching is covered only if implemented in this slice
  - authenticated search can return owner-private matches without returning
    another user’s private posts
- API integration tests:
  - `GET /api/posts?q=architecture` works anonymously
  - authenticated `GET /api/posts?q=...` can include owned private matches
  - other users’ private posts remain hidden
  - blank `q` returns the default listing contract
- Frontend checks:
  - update any existing public-list behavior tests if present
  - otherwise rely on `npm run lint`, `npm run build`, and manual full-stack
    validation of search-triggered requests and result rendering

**10. Validation strategy**:

- Backend test order:
  - Application tests
  - Infrastructure repository tests
  - API integration tests
- Frontend validation:
  - `npm run lint`
  - `npm run build`
  - manual validation of the `?q=` flow against the running stack
- Full-stack checks:
  - anonymous search for a known seeded public term
  - authenticated search for a term that matches an owned private post once such
    a fixture exists in tests or seed-aligned setup
  - empty search reset to default listing

**11. Risks and trade-offs**:

- The current public list DTO does not expose category or tag data, so search
  can match those fields without changing the UI payload; this is useful but may
  make some matches less obvious to the user.
- Tag matching should be implemented only if it can be done with the existing
  schema and query shape without broadening the feature into tag-management
  work.
- Using the existing `GET /api/posts` route keeps the frontend simple, but it
  makes the public list endpoint responsible for both default listing and search
  behavior; that is acceptable at this scale.
- No debounce keeps the implementation smaller and easier to explain, at the
  cost of leaving request-frequency optimization for a later slice.

## Technical Context

**Language/Version**: .NET 10 LTS (backend), TypeScript (frontend)

**Primary Dependencies**: ASP.NET Core Web API, Npgsql, xUnit, React, Vite,
TailwindCSS

**Storage**: PostgreSQL

**Testing**: xUnit unit tests, API integration tests, PostgreSQL-backed
repository tests where practical, frontend lint/build and manual runtime checks

**Target Platform**: Web application with ASP.NET Core backend and browser-based
frontend

**Project Type**: Full-stack web application

**Performance Goals**: Fast enough for interview-scale demo search usage without
additional caching or search infrastructure

**Constraints**: Raw SQL with Npgsql only; ProblemDetails-style errors; backend
authorization enforcement; explicit DTOs; no Redis/cache; no Entity Framework,
Dapper, MediatR, or external search services

**Scale/Scope**: Small interview MVP enhancement for search within the existing
blog platform

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: backend search work is sequenced around failing Application,
  repository, and API tests before any production code.
- PASS Backend-first: Application, Infrastructure, and API search changes are
  planned before the dependent frontend wiring update.
- PASS Architecture: controllers stay thin, Application owns visibility rules,
  Infrastructure owns SQL execution, and the frontend consumes the API only.
- PASS Data access: all persistence work remains parameterized raw SQL in the
  post repository.
- PASS Security: anonymous and authenticated visibility rules are explicit, and
  private posts owned by other users are excluded by design and by tests.
- PASS Scope: the plan stays within backend-powered search and avoids caching,
  pagination expansion, suggestions, and external search systems.
- PASS API consistency: the existing listing endpoint and summary DTOs are
  reused, with ProblemDetails behavior preserved for failures.
- PASS Frontend governance: the search UI stays aligned with `DESIGN.md`,
  reuses the existing shell and layout, and does not introduce scattered or
  one-off design changes.

## Project Structure

### Documentation (this feature)

```text
specs/015-search-posts/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── backend/
│   ├── BlogPlatform.Api/
│   ├── BlogPlatform.Application/
│   ├── BlogPlatform.Domain/
│   └── BlogPlatform.Infrastructure/
└── frontend/
    └── blog-web/
        └── src/
            ├── app/
            ├── components/
            ├── features/
            ├── lib/
            └── styles/

tests/
├── backend/
│   ├── BlogPlatform.Api.Tests/
│   ├── BlogPlatform.Application.Tests/
│   ├── BlogPlatform.Domain.Tests/
│   └── BlogPlatform.Infrastructure.Tests/
```

**Structure Decision**: Reuse the existing layered backend and feature-based
frontend structure. Keep search changes localized to the posts feature rather
than introducing new subsystems.

## Complexity Tracking

No constitution violations are required for this plan.
