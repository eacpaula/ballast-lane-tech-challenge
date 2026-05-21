# Feature Specification: Implement Raw SQL Repositories for Existing Application Use Cases

**Feature Branch**: `009-raw-sql-repositories`

**Created**: 2026-05-21

**Status**: Draft

## Feature Summary

This feature adds PostgreSQL-backed raw SQL repository implementations for the
application use cases that already exist in the backend core. The slice is
limited to Infrastructure persistence behavior, real-database integration
tests, and only the minimum supporting registration needed to exercise those
repositories against the configured local PostgreSQL environment.

## Goal

Provide repository implementations that satisfy the existing Application-layer
abstractions and prove their behavior with real PostgreSQL integration tests,
while keeping business rules in the Application layer and keeping SQL explicit,
readable, and isolated inside Infrastructure.

## Functional Requirements

- **FR-001**: The system MUST implement PostgreSQL-backed raw SQL repository
  classes for the existing Application repository abstractions already used by
  current use cases.
- **FR-002**: Repository implementations MUST remain focused on persistence,
  mapping, and data retrieval only and MUST NOT move business validation or
  authorization rules into Infrastructure.
- **FR-003**: User persistence MUST support the existing registration and login
  use cases.
- **FR-004**: Post persistence MUST support the existing create, update,
  remove, and public-read use cases.
- **FR-005**: Category persistence MUST support the existing administrator
  category-management use cases.
- **FR-006**: Post reaction persistence MUST support the existing public
  like/dislike use case.
- **FR-007**: Repository SQL MUST remain explicit and readable rather than
  hidden behind ORM or generic abstraction behavior.
- **FR-008**: Integration tests MUST validate repository behavior against a
  real PostgreSQL database using the existing Docker-based local environment.
- **FR-009**: Integration tests MUST be defined before repository
  implementation is considered complete for each persistence area in scope.
- **FR-010**: Infrastructure registration MAY be added only if needed to
  support repository integration testing or later repository consumption, but
  API endpoint work is out of scope for this slice.

## Repository Scope

- The repository abstractions currently in scope are:
  - `IUserRepository`
  - `IPostRepository`
  - `ICategoryRepository`
  - `IPostReactionRepository`
- User persistence scope includes:
  - creating a user record
  - loading a user by email
- Post persistence scope includes:
  - creating a post
  - loading a post by id
  - updating a post
  - removing a post
  - listing publicly readable posts
  - loading a publicly readable post by id
- Category persistence scope includes:
  - verifying category availability
  - checking duplicate titles
  - creating a category
  - loading a category by id
  - updating a category
  - deactivating a category
- Post reaction persistence scope includes:
  - creating a reaction record for a public post
- Standalone tag persistence is not required in this slice unless an existing
  Application abstraction is expanded to require it.

## Integration Test Scope

- Repository integration tests MUST run against a real PostgreSQL database.
- The existing Docker/PostgreSQL setup MUST be used for local validation.
- Tests SHOULD verify mapping, inserts, updates, deletes, public-read filters,
  duplicate-title checks, availability checks, and reaction persistence where
  applicable.
- Tests SHOULD isolate or reset database state so repository results are
  repeatable across runs.
- The first planning increment for this feature should prioritize integration
  test coverage design before repository implementation tasks.

## Out of Scope

- API controllers
- API authentication middleware
- JWT middleware wiring
- Frontend integration
- Full end-to-end testing
- Database migration framework
- Entity Framework
- Dapper
- Mediator or MediatR
- Advanced query optimization
- Production deployment
- Full RBAC or permission management beyond what current use cases require

## Acceptance Criteria

1. **Given** the existing Application repository abstractions, **when** this
   feature is implemented, **then** PostgreSQL-backed raw SQL repositories
   exist for the persistence areas already required by current use cases.
2. **Given** the repository integration test suite, **when** it is run against
   the local PostgreSQL environment, **then** it verifies repository behavior
   for users, posts, categories, and post reactions using real database state.
3. **Given** the post repository, **when** public-read queries are exercised,
   **then** only publicly readable and available posts are returned for the
   public read use cases.
4. **Given** the category repository, **when** create, update, and deactivate
   operations are exercised, **then** the persistence behavior needed by the
   administrator category-management use cases is available.
5. **Given** the user repository, **when** registration and login persistence
   paths are exercised, **then** user creation and lookup-by-email behavior
   work against PostgreSQL.
6. **Given** the Infrastructure layer, **when** repository implementations are
   added, **then** business rules remain outside the repositories and the
   Application layer remains independent from Infrastructure/Data details.

## Definition of Done

- A single infrastructure-focused specification exists for PostgreSQL raw SQL
  repositories and their integration test coverage.
- The repository scope is explicitly limited to the abstractions already used
  by current Application-layer use cases.
- The specification makes integration tests a first-class planning concern for
  the next phase.
- The specification does not move business rules into repositories and does not
  expand into API, frontend, migration-framework, or full RBAC work.
- No open clarification markers remain in the specification.
