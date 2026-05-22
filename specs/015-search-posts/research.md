# Research: Search Posts Through Backend API

## Decision 1: Reuse `GET /api/posts` with an optional search query

- **Decision**: Add search behavior to the existing public post listing route
  through an optional `q` query parameter instead of creating a separate search
  endpoint.
- **Rationale**: The frontend already writes `?q=` into the URL and the current
  public browsing page is the consumer of search. Reusing the list route keeps
  the contract simple and minimizes frontend disruption.
- **Alternatives considered**:
  - `GET /api/posts/search`: adds a new endpoint without improving the small
    feature scope.
  - A protected separate endpoint for authenticated search: unnecessarily splits
    one visibility problem across two routes.

## Decision 2: Keep visibility rules in Application and pass optional user context

- **Decision**: The Application handler should accept the search term plus an
  optional authenticated user id and remain responsible for deciding which
  matches are visible.
- **Rationale**: Search visibility is a business rule, not just a query detail.
  Infrastructure can help with filtering, but Application must remain the place
  where anonymous, authenticated, and owner-only visibility rules are enforced.
- **Alternatives considered**:
  - Put all visibility decisions directly into SQL only: efficient, but weakens
    the architecture boundary.
  - Build separate anonymous and authenticated handlers: more code without clear
    benefit at this scale.

## Decision 3: Extend the existing post list handler instead of introducing a separate search subsystem

- **Decision**: Evolve the current public post list use case into a
  search-capable handler rather than adding a parallel search pipeline.
- **Rationale**: Blank search must behave exactly like the default listing, so
  both behaviors naturally belong to one read flow.
- **Alternatives considered**:
  - Create a brand-new search handler and keep the current list handler:
    duplicates behavior and makes blank-query handling more awkward.

## Decision 4: Use one search-oriented repository method with parameterized SQL

- **Decision**: Add one post repository method that accepts a normalized query
  and optional user id, and implement it with parameterized raw SQL.
- **Rationale**: A single search-oriented method avoids multiple overlapping
  repository calls and keeps the Infrastructure change small.
- **Alternatives considered**:
  - Compose search from several repository methods in Application: too chatty
    and likely to duplicate filtering logic.
  - Add a generic query abstraction: too heavy for the project conventions.

## Decision 5: Use case-insensitive matching with PostgreSQL text operators

- **Decision**: Use PostgreSQL case-insensitive matching for supported search
  fields.
- **Rationale**: Case-insensitive behavior is a user expectation and is already
  practical with the current persistence stack without introducing full-text
  search.
- **Alternatives considered**:
  - Case-sensitive matching: simpler but poorer UX.
  - Full-text search: out of scope for the interview-sized feature.

## Decision 6: Include category matching and make tag matching conditional on query simplicity

- **Decision**: Include category-title matching in scope. Include tag matching
  only if it can be implemented through existing schema joins without widening
  the response model or adding unrelated repository surface.
- **Rationale**: Category linkage already exists directly on posts. Tags also
  exist in the schema, but the feature should not grow into broader tag
  behavior if the join complexity starts driving design decisions.
- **Alternatives considered**:
  - Ignore related text fields entirely: leaves part of the approved business
    scope unmet.
  - Always fully support tags regardless of query complexity: risks inflating
    the slice beyond its purpose.

## Decision 7: Keep frontend change minimal and avoid new request orchestration tooling

- **Decision**: Replace the in-memory filtering in `PublicPostListPage` with
  backend requests keyed by the existing `q` parameter, while keeping local page
  state and the existing shell/search form.
- **Rationale**: The frontend already has the right interaction shape. The
  missing piece is where filtering happens.
- **Alternatives considered**:
  - Introduce a new client-side data library: unnecessary for a single search
    flow.
  - Add debounce or autocomplete immediately: useful later, but not required to
    prove the backend-powered behavior.
