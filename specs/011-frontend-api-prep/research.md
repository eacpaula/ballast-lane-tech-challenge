# Research: Prepare API for Frontend Integration

## Decision 1: Add the API as a Docker Compose service

- **Decision**: Add a dedicated API service to `docker-compose.yml` and run it
  on the same Compose network as PostgreSQL.
- **Rationale**: The current local database bootstrap is already containerized,
  so putting the API beside it removes a common local setup gap before frontend
  work starts.
- **Alternatives considered**:
  - Keep API startup host-only and document it better: rejected because the
    feature explicitly aims to make backend startup easier for frontend
    integration.
  - Add reverse proxy infrastructure now: rejected as unnecessary production
    complexity for an interview-scale project.

## Decision 2: Use service-name database connectivity in containers

- **Decision**: Keep the existing database connection settings shape and
  override the database host to the Compose service name `postgres` for the API
  container.
- **Rationale**: This preserves the current Infrastructure configuration path
  while making container-to-container communication explicit and predictable.
- **Alternatives considered**:
  - Add separate container-only connection code: rejected because it would
    duplicate configuration logic.
  - Hardcode container connection details in application code: rejected because
    runtime environment configuration should stay external.

## Decision 3: Configure narrow local-only CORS

- **Decision**: Add explicit CORS configuration for the local React/Vite
  development origins only.
- **Rationale**: The frontend needs browser access during development, but the
  repo does not need broad wildcard cross-origin access.
- **Alternatives considered**:
  - Allow every origin in development: rejected because it is broader than
    needed and weakens clarity around intended local usage.
  - Defer CORS until frontend implementation: rejected because it is part of
    the backend-readiness goal for this slice.

## Decision 4: Add a dedicated public available-categories endpoint

- **Decision**: Expose a dedicated read-only category-list endpoint for
  frontend forms, recommended as `GET /api/categories/available`.
- **Rationale**: This keeps the purpose explicit, avoids conflating public read
  behavior with administrator management routes, and provides exactly the data
  the frontend form needs.
- **Alternatives considered**:
  - Reuse admin category endpoints with relaxed authorization: rejected because
    that would blur public read and admin write responsibilities.
  - Add a generic `GET /api/categories` that might later imply management or
    broader semantics: rejected because a narrower intent is easier to explain
    and test in this MVP.

## Decision 5: Put category listing behavior in Application, not controllers

- **Decision**: Introduce a small Application query/handler for listing
  available categories and back it with a repository read method.
- **Rationale**: The constitution requires business validation and non-trivial
  behavior to stay outside controllers, and the API layer should not reach into
  Infrastructure directly.
- **Alternatives considered**:
  - Query the repository directly from the controller: rejected because it
    bypasses Application and weakens the architecture boundary.
  - Build the list from the existing write handlers: rejected because those
    handlers serve different use cases and authorization expectations.

## Decision 6: Add repository coverage for the new category read query

- **Decision**: Add a PostgreSQL-backed repository test for the new available
  category query in addition to Application and API tests.
- **Rationale**: The new behavior depends on SQL filtering, so repository
  verification is practical and aligned with the constitution’s test strategy.
- **Alternatives considered**:
  - Test only at the API layer: rejected because SQL-level filtering behavior
    would be less directly proven.
  - Skip repository tests because the slice is small: rejected because this is
    exactly the kind of raw SQL filtering behavior that benefits from direct
    coverage.

## Decision 7: Defer tag listing until a concrete UI need exists

- **Decision**: Do not include a tag-list endpoint in this feature plan.
- **Rationale**: The current frontend-preparation gap is clearly about category
  selection for post forms; adding tags now would increase scope without a
  confirmed first-slice dependency.
- **Alternatives considered**:
  - Add tag listing now “just in case”: rejected because it expands scope and
    introduces unnecessary API surface before the frontend design proves the
    need.
