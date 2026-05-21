# Research: Authenticated User Creates a Blog Post

## Decision 1: Start with Domain and Application tests only

- **Decision**: The first increment will be implemented with failing tests in
  `BlogPlatform.Domain.Tests` and `BlogPlatform.Application.Tests` only.
- **Rationale**: The constitution requires test-first development and
  backend-first delivery. The user also explicitly asked to keep API,
  Infrastructure, and frontend out of the first implementation unless strictly
  necessary.
- **Alternatives considered**:
  - Start with API integration tests: rejected because it pulls HTTP and
    authentication concerns into the first increment.
  - Start with repository tests: rejected because raw SQL and PostgreSQL are not
    required to prove the core business behavior yet.

## Decision 2: Represent authentication as an already-resolved user id at the Application boundary

- **Decision**: The create-post use case will accept the authenticated user id
  as part of the input contract instead of reading HTTP or token state directly.
- **Rationale**: This keeps the Application layer independent of ASP.NET Core
  and allows the first increment to test business behavior without controllers
  or middleware.
- **Alternatives considered**:
  - Inject HTTP context or claims access into Application: rejected because it
    violates Clean Architecture boundaries.
  - Implement unauthenticated handling first in API: rejected because the user
    requested that API work stay out of the first increment.

## Decision 3: Split responsibilities between Domain invariants and Application orchestration

- **Decision**: Domain will enforce post invariants such as non-empty title and
  non-empty content. Application will coordinate category existence checks,
  repository calls, and result shaping.
- **Rationale**: This keeps validation close to the business entity while
  reserving cross-entity and persistence-related decisions for Application.
- **Alternatives considered**:
  - Put all validation in the handler: rejected because it weakens the Domain
    model.
  - Put category existence in Domain: rejected because category lookup depends
    on repository abstractions and external state.

## Decision 4: Use minimal repository abstractions for the first increment

- **Decision**: Introduce only `ICategoryRepository` and `IPostRepository`.
- **Rationale**: The use case needs only two external capabilities: verify that
  a category can be used and save the newly created post.
- **Alternatives considered**:
  - Generic repositories: rejected by the constitution as unnecessary
    abstraction.
  - Unit of Work abstraction: rejected for the first increment because no
    multi-repository transaction behavior is required yet.

## Decision 5: Treat public retrieval as a downstream concern

- **Decision**: In the first increment, "available for later public retrieval"
  means the Application layer returns a created post result with the data needed
  for later read-side exposure.
- **Rationale**: The spec requires the create flow only. Public listing and
  details are separate behaviors and should not expand this feature.
- **Alternatives considered**:
  - Implement read endpoints immediately: rejected as scope creep.
  - Add persistence query support now: rejected because the feature does not
    require it to validate create-post behavior.
