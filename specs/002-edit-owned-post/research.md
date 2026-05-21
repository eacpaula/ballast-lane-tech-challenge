# Research: Authenticated User Edits Only Their Own Blog Post

## Decision 1: Start with Domain and Application tests only

- **Decision**: The first increment will be implemented with failing tests in
  `BlogPlatform.Domain.Tests` and `BlogPlatform.Application.Tests` only.
- **Rationale**: The constitution requires test-first development and
  backend-first delivery. The user also explicitly asked to keep API,
  Infrastructure, and frontend out of the first implementation unless strictly
  necessary.
- **Alternatives considered**:
  - Start with API integration tests: rejected because it would pull HTTP and
    authorization-edge concerns into the first increment.
  - Start with repository tests: rejected because raw SQL behavior is not needed
    to prove ownership and edit rules yet.

## Decision 2: Reuse the existing BlogPost concept

- **Decision**: Reuse the existing `BlogPost` Domain concept from the create-post
  slice and extend it with edit-oriented behavior only if the tests demand it.
- **Rationale**: The edit-post feature operates on the same core entity, so
  reusing it keeps the model easier to explain and prevents duplicate post
  concepts.
- **Alternatives considered**:
  - Create a separate editable-post model: rejected because it duplicates the
    existing post concept without adding clarity.
  - Keep all update logic in Application only: rejected because required field
    invariants belong in Domain.

## Decision 3: Keep ownership validation in Application

- **Decision**: The edit-post handler will load the existing post and compare
  `AuthenticatedUserId` against the post owner before applying updates.
- **Rationale**: Ownership depends on both the authenticated user context and a
  loaded persisted record, which makes it an Application concern rather than a
  controller or infrastructure concern.
- **Alternatives considered**:
  - Validate ownership in the API layer: rejected because the constitution
    requires business validation outside controllers.
  - Push ownership checks into the repository: rejected because it hides a core
    business rule inside persistence details.

## Decision 4: Expand repository abstractions minimally

- **Decision**: Extend `IPostRepository` only with the behaviors needed to load a
  post by id and persist updates.
- **Rationale**: The edit-post use case needs to find a target post and save the
  changed post, but nothing more.
- **Alternatives considered**:
  - Introduce a generic repository: rejected by the constitution as unnecessary
    abstraction.
  - Introduce separate query and command stacks: rejected as overengineering for
    a small interview feature.

## Decision 5: Treat missing-post and unauthorized-owner outcomes as explicit business results

- **Decision**: Missing post and non-owner access will be expressed as explicit
  edit-post use case failures in Application.
- **Rationale**: These are the most important negative paths for test-first work
  and should be visible in unit tests before outer-layer mapping is added.
- **Alternatives considered**:
  - Throw exceptions for all failure cases: rejected because result-based
    failures are easier to test and easier to map consistently later.
  - Defer missing-post handling to repository exceptions: rejected because it
    blurs a business outcome with storage details.
