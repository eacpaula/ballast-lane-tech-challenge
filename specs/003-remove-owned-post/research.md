# Research: Authenticated User Removes Only Their Own Blog Post

## Decision 1: Start with Application tests only

- **Decision**: The first increment will be implemented with failing tests in
  `BlogPlatform.Application.Tests` only, adding Domain tests only if the
  existing `BlogPost` type needs removal-specific behavior.
- **Rationale**: The core risks in this slice are ownership, missing-post
  handling, and correct repository orchestration. Those are Application-layer
  concerns. Unlike create and edit, removal does not introduce new field
  invariants by default.
- **Alternatives considered**:
  - Start with API integration tests: rejected because it would pull HTTP and
    authentication-edge concerns into the first increment.
  - Start with repository tests: rejected because raw SQL behavior is not
    needed yet to prove ownership and deletion rules.
  - Force Domain tests up front: rejected because that would create artificial
    Domain work if removal remains pure orchestration.

## Decision 2: Reuse the existing BlogPost concept

- **Decision**: Reuse the existing `BlogPost` Domain concept from the create and
  edit slices as the loaded record used for ownership checks.
- **Rationale**: The remove-post feature works against an existing persisted
  post and does not require a separate entity shape.
- **Alternatives considered**:
  - Create a dedicated removable-post model: rejected because it duplicates the
    existing entity without clarifying behavior.
  - Pass owner identifiers alone from the repository: rejected because the use
    case is clearer when it loads the same post concept already used elsewhere.

## Decision 3: Keep ownership validation in Application

- **Decision**: The remove-post handler will load the existing post and compare
  `AuthenticatedUserId` against the post owner before issuing a delete through
  the repository abstraction.
- **Rationale**: Ownership depends on both caller context and persisted post
  data, which makes it an Application concern rather than a controller or
  repository concern.
- **Alternatives considered**:
  - Validate ownership in the API layer: rejected because the constitution
    requires business validation outside controllers.
  - Hide ownership checks in the repository: rejected because it obscures a core
    business rule inside persistence logic.

## Decision 4: Expand repository abstractions minimally

- **Decision**: Extend `IPostRepository` only with the behaviors needed to load
  a post by id and remove it.
- **Rationale**: The use case only needs to retrieve the target post and invoke
  a deletion action. Anything more is unnecessary abstraction.
- **Alternatives considered**:
  - Introduce a generic repository: rejected by the constitution as unnecessary
    abstraction.
  - Introduce separate query/command stacks: rejected as overengineering for a
    small interview feature.

## Decision 5: Model business failures explicitly

- **Decision**: Missing authenticated user, missing post, and non-owner access
  will be expressed as explicit remove-post use case failures in Application.
- **Rationale**: These are the critical negative paths and are easier to test,
  reason about, and map later than exception-driven control flow.
- **Alternatives considered**:
  - Throw exceptions for all failure cases: rejected because it makes the core
    business tests noisier and couples expected outcomes to error handling.
  - Rely on repository exceptions for missing posts: rejected because it blurs a
    business outcome with storage details.

## Decision 6: Recommend hard delete for the first persistence slice

- **Decision**: If a persistence decision is needed in a later slice, prefer a
  hard delete as the simplest implementation for this feature.
- **Rationale**: The current spec only requires removal behavior, not recovery,
  auditing, or retention. A hard delete keeps the repository contract and SQL
  implementation smaller for an interview-scale MVP.
- **Alternatives considered**:
  - Soft delete: rejected for now because it introduces extra state,
    filtering rules, and downstream query complexity that the current feature
    does not require.
