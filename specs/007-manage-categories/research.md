# Research: Administrator Manages Post Categories

## Decision 1: Start with Domain and Application tests

- **Decision**: The first increment will be implemented with failing tests in
  `BlogPlatform.Domain.Tests` and `BlogPlatform.Application.Tests`.
- **Rationale**: The core risks are not only orchestration, but also business
  invariants around valid category titles and category availability state.
  Those rules are clearer and more reviewable when expressed directly in a
  small Domain concept.
- **Alternatives considered**:
  - Start with Application tests only: rejected because it would push title and
    category-state rules into handlers.
  - Start with API integration tests: rejected because it would introduce HTTP
    concerns before the core rules are stable.

## Decision 2: Reuse the current authenticated-user input style

- **Decision**: Reuse the existing pattern of passing authenticated actor
  information into Application commands, adding a small admin indicator or
  equivalent authorization input instead of building full role management.
- **Rationale**: The current codebase already passes authentication context
  into Application for post ownership features. Extending that pattern keeps
  the authorization boundary consistent without introducing permissions
  infrastructure.
- **Alternatives considered**:
  - Introduce full role and permission domain models now: rejected as scope
    creep beyond this slice.
  - Defer authorization checks to controllers only: rejected because the
    constitution requires business authorization outside controllers.

## Decision 3: Model category titles explicitly

- **Decision**: Represent categories with a small Domain concept that
  normalizes and validates the title and tracks whether the category remains
  available.
- **Rationale**: The feature requires explicit title validation and later
  deactivation behavior. A dedicated Domain concept keeps those rules explicit
  and testable.
- **Alternatives considered**:
  - Keep category data as raw strings in handlers: rejected because it spreads
    validation logic across Application code.
  - Use a generic lookup entity for categories and tags together: rejected
    because it blurs distinct business rules.

## Decision 4: Prefer deactivation over hard delete

- **Decision**: Recommend deactivation for the first increment by marking a
  category unavailable instead of hard deleting it.
- **Rationale**: The schema already includes an availability concept for post
  categories. Deactivation avoids breaking existing post references while still
  satisfying the requirement that the category becomes unavailable for future
  use.
- **Alternatives considered**:
  - Hard delete categories: rejected because it increases risk around posts
    that still reference the category.
  - Support both hard delete and deactivation now: rejected because it expands
    the business surface without adding interview value.

## Decision 5: Expand the category repository minimally

- **Decision**: Expand `ICategoryRepository` only with the behaviors required
  to create, load, update, deactivate, and check duplicate titles.
- **Rationale**: The slice needs only category management behaviors and should
  avoid generic repositories or unused authorization abstractions.
- **Alternatives considered**:
  - Introduce a separate admin-category repository and keep the existing
    category repository unchanged: rejected because it duplicates category
    persistence concerns.
  - Introduce a generic CRUD abstraction: rejected by the constitution as
    unnecessary abstraction.

## Decision 6: Model duplicate-title checks in Application

- **Decision**: Keep duplicate-title checks in Application while Domain handles
  single-category title validity.
- **Rationale**: Duplicate-title validation depends on repository state and is
  therefore an Application concern rather than a pure Domain invariant.
- **Alternatives considered**:
  - Push duplicate detection into Domain: rejected because Domain should not
    depend on repository state.
  - Hide duplicate handling entirely in persistence: rejected because it would
    obscure a key business rule.
