# Research: Add Paginated Post Search With Redis Caching

## Decision 1: Keep the existing `GET /api/posts` route and extend it with `page` and `pageSize`

- **Decision**: Reuse the current post listing/search endpoint and add page-based
  pagination parameters rather than introducing a separate paginated route.
- **Rationale**: The current frontend already treats `GET /api/posts` as the
  source of truth for default listing and search. Extending the same route keeps
  the feature easier to explain and avoids duplicate contracts.
- **Alternatives considered**:
  - Create a new paginated search endpoint: adds contract sprawl without solving
    a real problem.
  - Introduce separate anonymous and authenticated list routes: unnecessary for
    a small visibility-aware feed.

## Decision 2: Use simple page/pageSize pagination with bounded defaults

- **Decision**: Use `page` and `pageSize` rather than cursor pagination.
- **Rationale**: Page-based pagination is enough for an interview-scale feed and
  fits naturally with caching because cache keys can include exact page inputs.
- **Alternatives considered**:
  - Cursor pagination: more robust at scale, but more complexity than this
    feature needs.

## Decision 3: Return a paginated envelope instead of a bare array

- **Decision**: Evolve the list/search response contract into a metadata-bearing
  envelope.
- **Rationale**: Infinite scroll requires more than an item array. The frontend
  needs at least page state and a reliable signal about whether more data is
  available.
- **Alternatives considered**:
  - Keep array-only response and infer next-page existence heuristically: too
    fragile and less explicit.

## Decision 4: Add a feature-scoped Application cache abstraction

- **Decision**: Introduce a post-list/search cache abstraction in Application
  rather than using Redis directly from the API or the repository.
- **Rationale**: Viewer context, cache key semantics, and safe reuse policy are
  application concerns. Infrastructure should provide the Redis-backed
  implementation only.
- **Alternatives considered**:
  - Put Redis calls in controllers: violates architecture boundaries.
  - Put Redis calls in the repository: mixes persistence query execution with
    read-cache orchestration.

## Decision 5: Use viewer-specific cache keys with a 30-second TTL

- **Decision**: Cache keys must include normalized query text, page, page size,
  and viewer context, and all entries should expire after 30 seconds.
- **Rationale**: Authenticated search can include owned private posts. Without
  viewer-specific keys, cached results could leak private data across sessions.
- **Alternatives considered**:
  - Anonymous-only caching: simpler but fails the requirement for authenticated
    cached reads.
  - Shared authenticated cache without user identity: unsafe.

## Decision 6: Accept short-lived stale data after writes

- **Decision**: Do not implement explicit cache invalidation for post writes or
  reactions in this feature.
- **Rationale**: The cache lifetime is intentionally short and the feature goal
  is to demonstrate simple performance-oriented caching, not full coherence
  management.
- **Alternatives considered**:
  - Fine-grained invalidation on every mutation: useful later, but too large for
    this slice.

## Decision 7: Keep the frontend feed update minimal with simple infinite loading

- **Decision**: Reuse the current search input and public feed UI, but replace
  the single-page request with incremental loading and an end-of-list state.
- **Rationale**: The current UI already has the right high-level structure. The
  feature only needs pagination-aware data flow and incremental loading states.
- **Alternatives considered**:
  - Traditional numbered pagination controls: workable, but less aligned with
    the requested scroll/infinite behavior.
  - Virtualized list rendering: unnecessary complexity for the expected result
    volumes.
