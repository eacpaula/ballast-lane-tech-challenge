# Implementation Plan: Add Paginated Post Search With Redis Caching

**Branch**: `016-search-pagination-cache` | **Date**: 2026-05-22 | **Spec**:
`specs/016-search-pagination-cache/spec.md`

**Input**: Feature specification from `/specs/016-search-pagination-cache/spec.md`

**Note**: This plan extends the existing backend-powered post search/listing
flow with simple page-based pagination and short-lived Redis caching, while
keeping visibility rules in Application and keeping the frontend changes limited
to incremental page loading.

## Summary

Add stable pagination and 30-second Redis caching to the current `GET /api/posts`
listing/search flow, then update the React public feed to load additional pages
through simple infinite scrolling.

- Add page and page-size inputs to the existing public post list use case.
- Return a paginated response envelope rather than a bare array.
- Introduce a feature-scoped read-cache abstraction in Application and a Redis
  implementation in Infrastructure.
- Use viewer-safe cache keys that include search term, page, page size, and
  requester context.
- Keep cache usage limited to list/search reads, with documented short-lived
  stale data after mutations.
- Update the frontend post list/search flow to append pages, show next-page
  loading, and stop at end-of-list.

## Implementation Design

**1. Technical approach**:

- Reuse the existing `GET /api/posts` endpoint rather than adding a second
  search route.
- Add optional `page` and `pageSize` query parameters alongside the existing
  `q` search parameter.
- Keep pagination simple:
  - default `page = 1`
  - default `pageSize = 6`
  - clamp page size to a small upper bound suitable for the demo, such as `20`
- Keep ordering deterministic by using one explicit database sort for both
  default listing and search results.
- Resolve the final result page in Application, then consult or populate Redis
  through an abstraction instead of writing Redis logic in the controller.

**2. Layers affected**:

- Application:
  - paginated search/list query model
  - response envelope model
  - visibility-aware orchestration
  - cache abstraction for list/search reads
- Infrastructure:
  - paginated SQL query support in the post repository
  - Redis connection/configuration
  - Redis-backed implementation of the list/search cache abstraction
- API:
  - `page`, `pageSize`, and `q` query parameters
  - paginated response DTOs
  - OpenAPI updates
- Frontend:
  - paginated post API client
  - incremental page loading in the public post list/search screen
  - loading, empty, error, and end-of-list behavior
- Dev runtime:
  - Redis service in Docker Compose
  - API environment configuration for Redis

**3. Pagination strategy**:

- Prefer numeric page-based pagination because it is the simplest fit for the
  current feed and easiest to explain in an interview.
- Request model:
  - `q` optional search term
  - `page` optional positive integer
  - `pageSize` optional positive integer
- Validation rules:
  - invalid or non-positive `page` and `pageSize` should produce the existing
    ProblemDetails-style API validation behavior
  - blank `q` remains valid and behaves like the default list
- Query behavior:
  - the same visibility rules apply on every page
  - empty page results past the end of the data set return success with
    pagination metadata rather than an error
- Keep ordering stable by preserving one consistent sort key across requests.

**4. API response strategy**:

- Replace the bare list response for `GET /api/posts` with a paginated envelope.
- Response envelope should include:
  - items
  - page
  - pageSize
  - totalCount
  - totalPages
  - hasNextPage
- Keep item payload shape aligned with the current post summary DTO so the
  frontend redesign is unnecessary.
- Preserve existing detail and reaction endpoints unchanged.

**5. Cache abstraction strategy**:

- Add a feature-specific Application abstraction for cached post list/search
  pages rather than a broad generic caching framework.
- The abstraction should support:
  - get cached paginated result by cache key
  - store paginated result with TTL
- Application remains responsible for:
  - deciding when cached results are safe to use
  - defining viewer context for the key
  - keeping visibility rules separate from Infrastructure
- Controllers do not call Redis directly.

**6. Redis infrastructure strategy**:

- Add a Redis service to Docker Compose for local development and demo usage.
- Add API configuration values for Redis host, port, and other minimal
  connection details.
- Implement the Application cache abstraction in Infrastructure using Redis and
  JSON serialization for paginated read responses.
- Keep Redis concerns small:
  - no clustering
  - no pub/sub
  - no advanced resilience or invalidation machinery

**7. Cache key strategy**:

- Cache keys must include:
  - feed scope marker (`posts:list`)
  - normalized search term or an explicit empty-search marker
  - page
  - pageSize
  - viewer context
- Viewer context should distinguish:
  - anonymous
  - authenticated user by user id
- If administrator behavior differs later, the key shape should already allow
  user-specific separation without special-casing anonymous data.
- Cache keys must not reuse the same entry for:
  - anonymous search and authenticated search
  - two different authenticated users
  - two different page or page-size combinations

**8. Cache TTL strategy**:

- Use a fixed 30-second TTL for all paginated list/search cache entries.
- Apply the same TTL to default listing and search variants.
- The TTL is short enough to keep the feature simple and still demonstrate
  performance-oriented read caching.

**9. Cache invalidation trade-off**:

- Do not add explicit cache invalidation for post writes or reactions in this
  feature.
- Accept that post creation, update, deletion, or reactions may leave stale
  list/search pages visible for up to 30 seconds.
- Document this explicitly in quickstart and README notes so the trade-off is
  clear and intentional.

**10. Frontend infinite scroll strategy**:

- Keep the existing search input and route query behavior.
- Update the post-list API client to send `q`, `page`, and `pageSize`.
- Replace the current single-request feed with a page accumulator:
  - reset pages when `q` changes
  - append items when the next page loads
- Use a simple infinite-scroll trigger, such as an intersection observer on a
  sentinel element near the end of the list.
- States to preserve:
  - initial loading
  - next-page loading
  - empty search results
  - feed-empty default state
  - error state
  - end-of-list state when `hasNextPage` is false
- Keep the existing visual layout and avoid virtualization or major redesign.

**11. Testing strategy**:

- Application tests first:
  - page and page-size validation behavior
  - anonymous paginated results remain public-only
  - authenticated paginated results can include owned private matches
  - page past end of results returns empty items with valid metadata
  - cache lookup and cache population orchestration respect viewer context
- Repository tests:
  - deterministic paginated result ordering
  - correct page slicing for default listing and search
  - visibility-safe paginated authenticated search
- API integration tests:
  - `GET /api/posts?page=1&pageSize=...`
  - `GET /api/posts?q=...&page=...&pageSize=...`
  - invalid page parameters return ProblemDetails-style failures
  - authenticated requests never leak another user’s private results
- Cache-focused tests:
  - key differentiation between anonymous and authenticated viewers
  - repeated request path can serve cached result
  - 30-second TTL behavior is configured correctly where practical
- Frontend validation:
  - keep automated checks light
  - rely on lint/build plus full-stack manual pagination checks if no dedicated
    browser test framework exists

**12. Docker Compose strategy**:

- Add a `redis` service to `docker-compose.yml`
- Add API dependency on Redis service startup where appropriate
- Add local environment variable examples for Redis connection settings
- Keep the frontend service unchanged except for relying on the API’s new
  paginated response contract
- Full local stack becomes:
  - PostgreSQL
  - Redis
  - API
  - frontend

**13. Documentation update strategy**:

- Update root README with:
  - Redis service presence
  - full-stack startup commands
  - paginated search/list endpoint notes
  - short-lived stale-cache trade-off
- Update frontend README if the client-side feed contract changes materially
- Update quickstart with:
  - Redis-included stack validation
  - anonymous pagination checks
  - authenticated private-result isolation checks

**14. Risks and trade-offs**:

- Changing `GET /api/posts` from a bare list to a paginated envelope is a real
  contract change and must be reflected consistently in tests and frontend code.
- Page-based pagination is simpler than cursor pagination, but it is more prone
  to minor duplication or shifting if data changes between requests; that trade
  is acceptable at this demo scale.
- Caching authenticated pages increases correctness risk, so key composition and
  tests around viewer isolation are critical.
- Redis adds one more runtime dependency to the local stack, but it is still
  manageable within the interview scope if configuration stays minimal.

## Technical Context

**Language/Version**: .NET 10 LTS (backend), TypeScript (frontend)

**Primary Dependencies**: ASP.NET Core Web API, Npgsql, Redis client for .NET,
xUnit, React, Vite, TailwindCSS

**Storage**: PostgreSQL for system data, Redis for 30-second read cache entries

**Testing**: xUnit unit tests, API integration tests, PostgreSQL-backed
repository tests, practical cache behavior tests, frontend lint/build and
manual full-stack validation

**Target Platform**: Web application with ASP.NET Core backend and browser-based
frontend, all runnable through local Docker Compose

**Project Type**: Full-stack web application

**Performance Goals**: Reduce repeated list/search read work for identical
requests within a 30-second window while keeping the feature simple enough for
demo usage

**Constraints**: Raw SQL with Npgsql only; ProblemDetails-style errors; backend
authorization enforcement; explicit DTOs; Redis only for read caching; no
advanced production Redis concerns; no Entity Framework, Dapper, MediatR, or
external search services

**Scale/Scope**: Small performance-focused enhancement for the current
post-list/search flow

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: backend pagination and caching behavior will begin with
  failing Application, repository, and API tests before production code.
- PASS Backend-first: pagination contract, visibility logic, repository query
  work, Redis integration, and API response changes are all planned before the
  dependent frontend infinite-scroll update.
- PASS Architecture: controllers stay thin, Application owns visibility and
  cache orchestration, Infrastructure owns Redis and SQL, and the frontend
  consumes the API contract only.
- PASS Data access: PostgreSQL work remains parameterized raw SQL within the
  post repository.
- PASS Security: viewer-specific cache keys and authenticated visibility tests
  explicitly prevent private-result leakage.
- PASS Scope: the plan stays within simple pagination and 30-second read cache
  behavior and avoids advanced invalidation, ranking, clustering, or full-text
  infrastructure.
- PASS API consistency: the plan preserves ProblemDetails-style failure
  handling, explicit DTOs, and OpenAPI alignment while evolving the list
  response contract.
- PASS Frontend governance: the feed update remains aligned with `DESIGN.md`,
  reuses the existing page structure, and adds only focused loading/end-of-list
  UI states.

## Project Structure

### Documentation (this feature)

```text
specs/016-search-pagination-cache/
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
frontend organization. Add Redis-specific implementation only inside
Infrastructure and keep pagination UI changes localized to the public posts
feature.

## Complexity Tracking

No constitution violations are required for this plan.
