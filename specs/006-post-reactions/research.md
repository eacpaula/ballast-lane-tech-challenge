# Research: Public Visitor Reacts To Posts

## Decision 1: Start with Domain and Application tests

- **Decision**: The first increment will be implemented with failing tests in
  `BlogPlatform.Domain.Tests` and `BlogPlatform.Application.Tests`.
- **Rationale**: The core risks are not only orchestration, but also business
  invariants around valid reaction types and valid actor identity. Those rules
  are easier to explain and protect when expressed directly in small Domain
  concepts.
- **Alternatives considered**:
  - Start with Application tests only: rejected because it would push
    reaction-type and actor validation into handler code that is harder to
    reuse and reason about.
  - Start with API integration tests: rejected because it would introduce HTTP
    concerns before the core rules are stable.

## Decision 2: Reuse the current public-post eligibility model

- **Decision**: Reuse `BlogPost.IsPubliclyReadable` to determine whether a post
  can accept a public reaction.
- **Rationale**: Public reactions should follow the same visibility and
  availability rule as public reads. Reusing the existing model keeps the rule
  consistent and avoids introducing a second public-post interpretation.
- **Alternatives considered**:
  - Add a separate reaction-specific visibility rule: rejected because it risks
    diverging from the existing public-read behavior.
  - Trust persistence filtering only: rejected because it hides the business
    rule outside the Application boundary.

## Decision 3: Model reaction type explicitly

- **Decision**: Represent the allowed reaction values as a small Domain concept
  such as `ReactionType` instead of passing free-form strings through
  Application.
- **Rationale**: The feature requires explicit rejection of invalid reaction
  values. A dedicated Domain concept makes the allowed values narrow and
  testable.
- **Alternatives considered**:
  - Keep reaction type as a raw string in the handler: rejected because it
    spreads validation logic across Application code.
  - Use a large generalized enum for future reaction variants: rejected because
    the MVP is limited to like and dislike only.

## Decision 4: Keep actor association simple and explicit

- **Decision**: Use a small actor-identity concept that accepts either a user
  identifier or a visitor identifier, with the rule that at least one valid
  identity must be present and authenticated user identity takes precedence when
  both are supplied.
- **Rationale**: The spec requires reactions to support anonymous and later
  authenticated contexts without transport coupling. This keeps the rule small,
  testable, and independent from HTTP or auth middleware.
- **Alternatives considered**:
  - Require only visitor identifiers now: rejected because the spec explicitly
    allows later authenticated use.
  - Build a full current-user/current-visitor framework: rejected as scope
    creep.

## Decision 5: Use a dedicated reaction repository abstraction

- **Decision**: Introduce a separate `IPostReactionRepository` abstraction with
  only the minimum create behavior needed for this slice.
- **Rationale**: Reactions are a distinct persistence concern from post CRUD
  and public reads. A dedicated repository keeps the boundaries easier to
  explain and avoids bloating `IPostRepository` with unrelated write behavior.
- **Alternatives considered**:
  - Add reaction creation to `IPostRepository`: rejected because it mixes post
    entity lifecycle concerns with reaction recording.
  - Introduce a generic write repository: rejected by the constitution as
    unnecessary abstraction.

## Decision 6: Treat each valid request as a new reaction record

- **Decision**: For this first increment, each accepted request will create a
  reaction record without deduplication, overwrite rules, analytics, or history
  queries.
- **Rationale**: The spec requires submission behavior only. Duplicate
  prevention, toggling, and historical analysis are separate concerns that
  would expand the scope beyond the current slice.
- **Alternatives considered**:
  - Enforce one reaction per actor per post now: rejected because it adds
    identity-resolution and replacement rules that the spec does not require.
  - Track aggregate like/dislike counters in this increment: rejected because
    analytics are explicitly out of scope.
