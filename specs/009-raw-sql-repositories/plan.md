# Implementation Plan: Implement Raw SQL Repositories for Existing Application Use Cases

**Branch**: `009-raw-sql-repositories` | **Date**: 2026-05-21 | **Spec**: `specs/009-raw-sql-repositories/spec.md`

**Input**: Feature specification from `/specs/009-raw-sql-repositories/spec.md`

**Note**: This plan focuses on Infrastructure/Data Access only. It introduces
integration-test-first raw SQL repositories for existing Application
abstractions while keeping API controllers, frontend integration, and migration
framework work deferred.

## Summary

Implement PostgreSQL-backed raw SQL repositories for the Application-layer
abstractions already used by the blog-platform backend.

- Start with failing PostgreSQL-backed integration tests in
  `tests/backend/BlogPlatform.Infrastructure.Tests` for each repository area in
  scope.
- Add only the minimum Infrastructure production types required to make those
  tests compile and pass: database connection support, repository classes,
  mapping helpers where justified, and optional service registration for test
  setup or later composition.
- Keep all business validation, authorization, ownership checks, duplicate
  handling decisions, and public-read rule decisions in Application; the
  repositories only persist and retrieve data.
- Reuse the existing Docker Compose PostgreSQL environment, schema scripts, and
  seed data as the baseline for test validation.

## Implementation Design

**Layers affected**:

- `src/backend/BlogPlatform.Infrastructure`
- `tests/backend/BlogPlatform.Infrastructure.Tests`
- optionally a small DI extension or configuration surface in
  `src/backend/BlogPlatform.Infrastructure`
- no planned changes to `src/backend/BlogPlatform.Application`,
  `src/backend/BlogPlatform.Domain`, `src/backend/BlogPlatform.Api`, or
  `src/frontend`

**Repository abstractions to implement**:

- `IUserRepository`
- `IPostRepository`
- `ICategoryRepository`
- `IPostReactionRepository`

**Database connection strategy**:

- Use PostgreSQL through `Npgsql`.
- Keep connection-string ownership in Infrastructure.
- Use explicit SQL commands and parameter binding for every repository method.
- Prefer a simple connection factory or equivalent small Infrastructure helper
  over broad data-access abstractions.

**Integration test strategy**:

- Write failing repository integration tests first in
  `tests/backend/BlogPlatform.Infrastructure.Tests`.
- Run tests against a real PostgreSQL database prepared by the existing Docker
  Compose environment.
- Cover inserts, updates, deletes, lookups, public-read filters, duplicate
  checks, availability checks, and reaction persistence as needed by the
  current Application use cases.

**Test database and reset strategy**:

- Reuse the existing schema and seed scripts as the known-good database shape.
- Add a deterministic reset strategy for repository tests so test state does not
  bleed between runs.
- Keep the reset mechanism explicit and simple for interview review, even if it
  means using SQL cleanup or rebuilding a disposable database state between
  test runs.

**Raw SQL implementation strategy**:

- Keep SQL statements readable and grouped by repository responsibility.
- Map only the fields needed by the current Domain/Application models.
- Keep repository methods focused on persistence contracts rather than business
  outcomes; for example, duplicate-title enforcement belongs in Application,
  while duplicate-title existence checks belong in the category repository.

**Dependency injection strategy**:

- Add Infrastructure service registration only if it meaningfully reduces
  friction for integration test setup or later composition.
- Keep DI additions minimal and repository-focused.
- Do not use DI changes as a vehicle for API behavior or controller work in
  this slice.

**Validation strategy**:

- Prove each repository behavior through failing-then-passing real-database
  integration tests.
- Re-run the Infrastructure test suite against PostgreSQL.
- Re-run the Application tests if any shared abstractions or persistence-facing
  contracts are touched.
- Confirm the repositories persist and retrieve data without taking over
  Application-layer validation rules.

**Risks and trade-offs**:

- Raw SQL is explicit and reviewable but requires manual mapping care and more
  integration-test coverage.
- Reusing one shared PostgreSQL environment is simple, but test isolation must
  be handled deliberately to avoid state leakage.
- Adding too much Infrastructure indirection would weaken the challenge’s
  clarity, so the plan favors simple repository classes over generic data
  layers.

## Technical Context

**Technical Approach**: Implement `Npgsql`-based Infrastructure repositories
behind existing Application abstractions and prove them through
PostgreSQL-backed integration tests before wider composition work

**Language/Version**: .NET 10 LTS

**Primary Dependencies**: Npgsql, xUnit, existing Docker Compose PostgreSQL
environment, existing schema and seed SQL scripts

**Storage**: PostgreSQL

**Testing**: xUnit PostgreSQL-backed integration tests in
`tests/backend/BlogPlatform.Infrastructure.Tests`; Application tests rerun only
as regression coverage when shared contracts change

**Target Platform**: ASP.NET Core backend with PostgreSQL persistence, using
the local Docker environment for validation

**Project Type**: Full-stack web application with a layered .NET backend

**Performance Goals**: Keep repository behavior acceptable for interview-scale
demo usage; optimize for correctness, clarity, and explicit filtering before
advanced query tuning

**Constraints**: Raw SQL with Npgsql only; no Entity Framework, Dapper,
Mediator, or MediatR; no migration framework in this slice; business logic
stays in Application; repositories persist and retrieve data only

**Scale/Scope**: One Infrastructure slice covering user, post, category, and
post-reaction repository behavior needed by current Application use cases

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the first executable work is failing PostgreSQL-backed
  integration tests for repository contracts before repository implementation.
- PASS Backend-first: no frontend work is included in this slice.
- PASS Architecture: Application abstractions stay stable; Infrastructure
  implements persistence only; business validation remains outside
  repositories.
- PASS Data access: persistence remains raw SQL with Npgsql only and is
  isolated to `src/backend/BlogPlatform.Infrastructure`.
- PASS Security: password hashing remains outside repositories; repositories
  only store and retrieve already-validated data, while authentication and
  authorization decisions remain outside Infrastructure.
- PASS Scope: the plan stays limited to repository implementations needed by
  current use cases and defers API/controller work, migration tooling, and full
  RBAC expansion.
- PASS API consistency: no HTTP behavior is introduced in this slice; later
  ProblemDetails and OpenAPI concerns remain with API work.

## Project Structure

### Documentation (this feature)

```text
specs/009-raw-sql-repositories/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── repository-integration-contracts.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── backend/
│   ├── BlogPlatform.Domain/
│   ├── BlogPlatform.Application/
│   ├── BlogPlatform.Infrastructure/
│   └── BlogPlatform.Api/
└── frontend/
    └── blog-web/

tests/
└── backend/
    ├── BlogPlatform.Domain.Tests/
    ├── BlogPlatform.Application.Tests/
    ├── BlogPlatform.Infrastructure.Tests/
    └── BlogPlatform.Api.Tests/

database/
├── migrations/
├── scripts/
└── seeds/
```

**Structure Decision**: This slice should primarily touch
`src/backend/BlogPlatform.Infrastructure` and
`tests/backend/BlogPlatform.Infrastructure.Tests`, with optional minimal
configuration helpers in Infrastructure only. Existing Application and Domain
code should remain unchanged unless a test exposes a contract gap.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations are required for this plan | Direct repository classes plus focused integration tests satisfy the feature without extra abstraction |
