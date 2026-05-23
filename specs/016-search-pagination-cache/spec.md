# Feature Specification: Add Paginated Post Search With Redis Caching

## 1. Feature Summary

Extend the existing backend-powered post search so post listing and search
results can be paginated and briefly cached, while preserving the current
visibility rules for anonymous and authenticated users.

## 2. Goal

Provide a small, demo-friendly performance improvement for post browsing by
adding deterministic pagination and short-lived cached search results, then
update the frontend to consume the paginated results through a simple
scroll-based loading flow.

## 3. Functional Requirements

- **FR-001**: The system MUST support paginated post listing and paginated post
  search for anonymous visitors.
- **FR-002**: The system MUST support paginated post listing and paginated post
  search for authenticated users.
- **FR-003**: Anonymous users MUST receive only public and available posts in
  paginated results.
- **FR-004**: Authenticated users MUST receive the same public and available
  posts as anonymous users in the default paginated listing when no search term
  is provided.
- **FR-005**: Authenticated users MUST receive public and available posts plus
  their own matching private or non-public posts when existing visibility rules
  allow those posts to appear in paginated search results.
- **FR-006**: Authenticated users MUST NOT receive private or non-public posts
  owned by other users in either live or cached results.
- **FR-007**: Empty search input MUST behave like the default paginated post
  listing.
- **FR-008**: The system MUST return pagination metadata together with the
  current page of post results while preserving the existing public post summary
  fields already used by the frontend.
- **FR-009**: Pagination order MUST be deterministic and stable enough for a
  simple technical challenge demo so repeated requests for the same page do not
  shuffle results unexpectedly.
- **FR-010**: The system MUST cache read-only post list and post search
  responses for 30 seconds.
- **FR-011**: Cached responses MUST be segmented so anonymous results, one
  user’s private-inclusive results, and another user’s results cannot be mixed
  or leaked.
- **FR-012**: The system MUST use caching only for read/search/list operations
  in this feature.
- **FR-013**: Write operations such as post create, post update, post delete,
  and reactions MAY serve slightly stale cached list results during the 30-second
  cache window, and this trade-off MUST be documented.
- **FR-014**: The frontend MUST replace the current single-load search/list
  behavior with paginated requests and simple scroll or infinite loading while
  preserving the existing visual direction and accessibility baseline.

## 4. Pagination Rules

- Pagination applies to both default listing and search results.
- Default pagination uses `page = 1` and `pageSize = 6` when callers do not
  provide explicit values.
- `pageSize` is bounded to a small demo-friendly maximum of `20`.
- Pagination must be stable for identical inputs during the same data window.
- The same visibility rules apply on every page, not only on the first page.
- Invalid or non-positive `page` and `pageSize` values return the existing
  validation failure behavior rather than being silently corrected.
- A page request beyond the available result set returns an empty item list with
  correct pagination metadata rather than an error.
- Empty or whitespace-only search requests use the same pagination rules as the
  default listing.

## 5. Cache Rules

- Cache applies only to read/list/search operations.
- Cache time-to-live is exactly 30 seconds.
- Cache may be used for anonymous listing, anonymous search, authenticated
  listing, and authenticated search, as long as the viewer context is isolated.
- Cache does not need complex invalidation for post mutations or reactions in
  this feature because the cache lifetime is intentionally short.
- The short-lived stale-data trade-off must be visible in the feature
  documentation and accepted as part of the demo scope.

## 6. Cache Key Strategy

- Cache keys must include:
  - search text or empty-search marker
  - page number
  - page size
  - viewer context
- Viewer context must distinguish:
  - anonymous visitors
  - authenticated regular users by user identity
  - authenticated administrators by user identity when their results differ
- Cache keys must be specific enough to prevent one user from receiving another
  user’s private-inclusive results.

## 7. Backend Scope

- Add backend pagination parameters to the existing post listing and search flow.
- Return paginated response metadata together with result items while preserving
  the existing public post summary payload shape.
- Add Redis-backed caching for paginated read/search/list responses.
- Add or update Application behavior to orchestrate pagination and viewer-safe
  cached reads without moving business rules into controllers.
- Add or update repository behavior to support deterministic paginated search
  and listing queries.
- Add Redis service configuration to the local Docker Compose stack.

## 8. Frontend Scope

- Update the current post listing and search flow to consume paginated backend
  results.
- Keep the existing search input and general page structure.
- Implement simple scroll or infinite pagination loading for additional pages.
- Show loading state while fetching the next page.
- Show an end-of-list state when no more results are available.
- Preserve existing error and empty-result states and keep the UI aligned with
  `DESIGN.md`.

## 9. Test Scope

- Add or update backend tests for paginated anonymous and authenticated search
  behavior.
- Add or update backend tests for pagination metadata and stable page behavior.
- Add or update backend tests for Redis cache usage where practical.
- Add or update tests to prove cache keys do not leak private results between
  anonymous and authenticated viewers or between different authenticated users.
- Add or update frontend tests where practical for paginated loading behavior,
  while keeping final UI validation grounded in lint/build checks and manual
  full-stack verification.

## 10. Out of Scope

- Advanced cache invalidation strategy
- Distributed cache stampede protection
- Redis clustering
- Full-text search engine behavior
- External search infrastructure
- Advanced ranking or scoring
- Cursor-based pagination unless it becomes trivial without changing scope
- Complex frontend virtualization
- Major frontend redesign
- Caching write operations

## 11. Acceptance Criteria

- An anonymous visitor can paginate through post results and search results and
  sees only public and available posts.
- An authenticated user can paginate through matching post results and sees
  public posts in the default listing plus their own private or non-public
  matching posts only in search results when allowed, but never another user’s
  private posts.
- Empty search behaves like the default paginated listing.
- Repeated identical requests within 30 seconds can use cached results without
  leaking viewer-specific data.
- The frontend can load the next page through simple infinite scroll behavior,
  shows a loading state during fetch, and shows an end-of-list state when no
  further results are available.
- End-to-end validation works through the local Docker Compose stack with
  PostgreSQL, API, frontend, and Redis running together.

## 12. Definition of Done

- Pagination and caching behavior are fully specified with no open
  clarification markers.
- The 30-second cache policy and viewer-safe cache segmentation are explicitly
  defined.
- Backend and frontend scope remain limited to simple pagination plus short-lived
  caching.
- The stale-data trade-off for write operations is documented.
- The feature remains appropriate for a technical interview demo and does not
  expand into broader search infrastructure.
