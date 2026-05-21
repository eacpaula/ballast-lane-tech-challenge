# Research: Implement Raw SQL Repositories for Existing Application Use Cases

## Decision 1: Start with repository integration tests

- **Decision**: The first implementation increment will begin with failing
  PostgreSQL-backed integration tests in
  `tests/backend/BlogPlatform.Infrastructure.Tests`.
- **Rationale**: The main risk in this slice is SQL correctness, mapping, and
  database-state behavior rather than pure Application orchestration.
- **Alternatives considered**:
  - Start by writing repository classes first: rejected because it weakens the
    test-first requirement for persistence behavior.
  - Start with API integration tests: rejected because API wiring is out of
    scope for this slice.

## Decision 2: Use Npgsql directly with explicit parameterized SQL

- **Decision**: Implement repositories with `Npgsql` and explicit
  parameterized SQL commands.
- **Rationale**: This matches the constitution and keeps the persistence layer
  explicit, reviewable, and constrained.
- **Alternatives considered**:
  - Entity Framework: rejected by feature and constitution constraints.
  - Dapper: rejected by feature and constitution constraints.
  - Custom generic data mapper: rejected because it adds indirection without
    strong value for this challenge.

## Decision 3: Keep repository contracts aligned to existing abstractions only

- **Decision**: Implement only the repository abstractions already required by
  current Application handlers: user, post, category, and post reaction.
- **Rationale**: This avoids speculative Infrastructure work and keeps the
  slice tied to existing behavior.
- **Alternatives considered**:
  - Add tag repositories now: rejected because there is no current Application
    abstraction requiring a standalone tag repository.
  - Expand persistence abstractions preemptively: rejected as scope creep.

## Decision 4: Keep business validation out of Infrastructure

- **Decision**: Repositories will persist and retrieve data only; business
  validation remains in Application.
- **Rationale**: This preserves Clean Architecture boundaries and keeps
  repositories easier to explain.
- **Alternatives considered**:
  - Duplicate validation in repositories: rejected because it scatters business
    rules and couples Infrastructure to domain decisions.
  - Push availability or duplicate-title decisions fully into SQL-only
    outcomes: rejected because Application already owns those business rules.

## Decision 5: Reuse the existing Docker/PostgreSQL environment and schema

- **Decision**: Use the existing Docker Compose PostgreSQL environment plus the
  current schema and seed scripts as the baseline for repository integration
  tests.
- **Rationale**: The environment already exists and was validated, so reusing
  it keeps the persistence slice grounded in real local setup.
- **Alternatives considered**:
  - Introduce a separate database bootstrap for tests: rejected because it
    duplicates the environment story unnecessarily.
  - Use an in-memory substitute: rejected because SQL behavior is the main
    thing that needs proving.

## Decision 6: Use explicit test reset workflows

- **Decision**: Add a deterministic database reset or cleanup mechanism for
  Infrastructure tests rather than relying on seeded state remaining unchanged
  between tests.
- **Rationale**: Repository tests need repeatability, and data-modifying tests
  will otherwise interfere with one another.
- **Alternatives considered**:
  - Share one mutable seeded database across tests with no cleanup: rejected
    because it is fragile and order-dependent.
  - Rebuild the full Docker environment for every single test: rejected because
    it is too slow and heavy for the intended scale.

## Decision 7: Keep DI additions minimal and Infrastructure-scoped

- **Decision**: Add dependency-injection registration only if it helps
  integration tests or later repository composition, and keep it inside
  Infrastructure.
- **Rationale**: Minimal registration support is useful, but broad API
  composition work is outside this slice.
- **Alternatives considered**:
  - No DI support at all: rejected because a small Infrastructure registration
    surface may simplify tests and later assembly.
  - Full API startup integration now: rejected because API work is explicitly
    out of scope.

## Decision 8: Favor readable SQL over query abstraction

- **Decision**: Keep SQL grouped by repository responsibility and optimize for
  readability first.
- **Rationale**: The challenge values explainability and direct persistence
  control.
- **Alternatives considered**:
  - Shared SQL builder abstractions: rejected because they add cognitive
    overhead and reduce transparency.
  - Aggressive optimization work now: rejected because correctness is the
    higher-priority concern for this slice.
