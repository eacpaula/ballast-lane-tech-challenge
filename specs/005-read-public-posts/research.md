# Research: Public Visitor Reads Posts

## Decision 1: Start with Application tests only

- **Decision**: The first increment will be implemented with failing tests in
  `BlogPlatform.Application.Tests` only, adding Domain tests only if the
  existing `BlogPost` model needs a small public-availability expansion.
- **Rationale**: The core risks in this slice are public filtering, unavailable
  detail rejection, and repository-based read orchestration. Those are
  Application concerns and can be proven without HTTP or persistence details.
- **Alternatives considered**:
  - Start with API integration tests: rejected because it would pull HTTP and
    endpoint concerns into the first increment.
  - Start with repository tests: rejected because raw SQL behavior is not
    needed yet to prove public read rules.

## Decision 2: Reuse the existing post concept where possible

- **Decision**: Reuse the existing `BlogPost` concept if a small extension can
  express public and available state clearly; otherwise introduce only the
  smallest read-specific projection needed for this slice.
- **Rationale**: Reuse keeps the post model easier to explain, but the current
  shape does not yet carry visibility or availability information.
- **Alternatives considered**:
  - Create a full separate public-post entity: rejected because it risks
    duplicating too much of the existing post concept.
  - Ignore post state and push all filtering to repository magic: rejected
    because it would hide important business meaning.

## Decision 3: Keep public filtering outcomes in Application

- **Decision**: The public read handlers will rely on repository abstractions
  for data retrieval and will treat missing, unavailable, or non-public detail
  results as not available to the public.
- **Rationale**: The feature’s main business rule is not just reading posts,
  but ensuring only public, available content is returned to anonymous users.
- **Alternatives considered**:
  - Validate public availability in the API layer: rejected because the
    constitution requires business rules outside controllers.
  - Hide all availability logic inside persistence only: rejected because it
    obscures the public-read rule set.

## Decision 4: Expand repository abstractions minimally

- **Decision**: Extend `IPostRepository` only with the behaviors needed to list
  public, available posts and read one public, available post by id.
- **Rationale**: The slice needs two read operations and nothing more.
- **Alternatives considered**:
  - Introduce a generic read repository: rejected by the constitution as
    unnecessary abstraction.
  - Introduce search/filter/pagination interfaces now: rejected as scope creep
    beyond the simple default ordering required for this feature.

## Decision 5: Model unavailable detail reads explicitly

- **Decision**: Non-public, unavailable, or missing detail reads will be
  expressed as an explicit not-available public read outcome in Application.
- **Rationale**: This is the most important negative path and is easier to test
  and map later than relying on exceptions or raw repository semantics alone.
- **Alternatives considered**:
  - Throw exceptions for all missing or hidden posts: rejected because it makes
    expected business outcomes noisier in tests.
  - Return hidden posts and rely on later filtering: rejected because it risks
    leaking content outside the Application boundary.
