# Implementation Plan: Configure PostgreSQL Database Environment and Schema Initialization

**Branch**: `008-postgres-db-setup` | **Date**: 2026-05-21 | **Spec**: `specs/008-postgres-db-setup/spec.md`

**Input**: Feature specification from `/specs/008-postgres-db-setup/spec.md`

**Note**: This plan focuses on Infrastructure preparation only. It creates the
local PostgreSQL bootstrap path that later Infrastructure/Data Access work will
use, while keeping repository code, API wiring, and frontend behavior deferred.

## Summary

Prepare a repeatable local PostgreSQL environment for the blog platform using
Docker Compose plus raw SQL schema and seed scripts.

- Start with failing validation steps for local database startup, schema
  initialization, seed availability, and reset behavior before adding the
  supporting infrastructure files.
- Create the smallest useful local environment: one PostgreSQL service, a
  repository-managed SQL initialization flow, and compact demo data for the
  already implemented use cases.
- Keep schema scope aligned with the current MVP slices only: users, roles,
  user-role assignment, posts, post categories, tags, post tags, and post
  reactions.
- Defer repository classes, migration frameworks, API startup integration, and
  full RBAC expansion until the database bootstrap path is stable and
  documented.

## Implementation Design

**Files and folders to create or update**:

- Create `docker-compose.yml` at repository root for the PostgreSQL service.
- Create or update a local environment example such as `.env.example` if the
  Compose flow needs documented variables.
- Populate `database/scripts/` with ordered schema initialization SQL files.
- Populate `database/seeds/` with ordered demo seed SQL files.
- Optionally use `database/migrations/` only for future work notes; do not
  introduce a migration framework in this slice.
- Update database setup documentation in `docs/Database-Implementation-Strategy.md`
  and `README.md` if present.

**Docker Compose strategy**:

- Use a single PostgreSQL service for local development and interview demos.
- Prefer a stable service definition with mounted initialization artifacts or a
  clearly documented manual execution order.
- Keep the Compose scope database-only so startup remains fast and easy to
  explain.

**Database initialization strategy**:

- Support a clean bootstrap from an empty local state.
- Make schema creation and seed loading explicit and rerunnable.
- Keep reset behavior simple: tear down, recreate, reapply schema, and reapply
  seeds predictably.

**Schema script strategy**:

- Convert the existing schema diagram into ordered raw SQL files for only the
  current-use-case tables.
- Preserve clear dependencies between tables so foreign keys can be applied in
  a deterministic order.
- Defer permissions, modules, settings, and role-permission tables until later
  Infrastructure or authorization features truly require them.

**Seed data strategy**:

- Seed one administrator, one regular user, role assignments, categories,
  public posts, tags, post-tag links, and reactions.
- Keep the dataset intentionally small so it is easy to inspect manually.
- Document demo credentials clearly as local-only values.

**Environment variable strategy**:

- Use environment variables only for local PostgreSQL runtime concerns such as
  database name, username, password, and port.
- Avoid introducing broader API/application configuration work in this slice.
- Document defaults and expected overrides in a developer-friendly way.

**README and database documentation strategy**:

- Document how to start the database, initialize the schema, apply seeds,
  reset the environment, and inspect data.
- Keep the instructions concise enough for technical interview demo usage.
- Explain which schema-diagram tables are intentionally deferred and why.

**Validation strategy**:

- Prove the environment can start from scratch on a clean machine with Docker.
- Prove schema scripts execute in order without manual rewriting.
- Prove seed scripts load the expected demo dataset.
- Prove the environment can be reset and rebuilt predictably.
- Leave repository-level persistence tests for the next Infrastructure/Data
  Access slice once bootstrap stability exists.

**Risks and trade-offs**:

- Ordered raw SQL is explicit and interview-friendly but requires manual care
  for script ordering and re-runs.
- A Compose-only local database setup is easy to explain but does not solve
  production deployment concerns, which are intentionally deferred.
- Deferring RBAC-adjacent tables keeps scope tight but means full authorization
  persistence will need a later schema expansion.

## Technical Context

**Technical Approach**: Add a Docker Compose-managed PostgreSQL environment,
ordered raw SQL schema scripts under `database/`, ordered seed scripts for demo
data, and concise local run/reset/inspection documentation based on the
existing schema diagram

**Language/Version**: SQL for schema and seed scripts; YAML for container
orchestration; Markdown for local setup documentation; .NET 10 LTS remains the
consumer of this environment later but is not the first implementation focus

**Primary Dependencies**: PostgreSQL container image, Docker Compose, raw SQL
scripts, existing repository documentation

**Storage**: PostgreSQL

**Testing**: Database bootstrap validation through repeatable local startup,
schema verification queries, seed verification queries, and later
PostgreSQL-backed repository tests built on top of this environment

**Target Platform**: Local development and technical interview demo usage on a
developer machine running Docker

**Project Type**: Full-stack web application with a dedicated local database
bootstrap slice

**Performance Goals**: Keep local startup and reset simple and predictable for
interview-scale data volumes; prioritize clarity and repeatability over
high-volume optimization

**Constraints**: Raw SQL only; no Entity Framework, Dapper, Mediator, or
MediatR; no migration framework in this slice; scope limited to current use
cases; keep database artifacts easy to inspect manually

**Scale/Scope**: Single Infrastructure preparation slice covering Docker
Compose, schema scripts, seed scripts, local environment variables, and
database documentation only

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the first executable work is failing database bootstrap
  validation for container startup, schema initialization, seed availability,
  and reset behavior before repository implementation begins.
- PASS Backend-first: this slice prepares backend persistence prerequisites and
  contains no frontend work.
- PASS Architecture: the plan affects local infrastructure assets,
  documentation, and the `database/` workspace only; Application and Domain
  boundaries remain unchanged.
- PASS Data access: the plan uses raw SQL scripts only and keeps future Npgsql
  repository work isolated to Infrastructure/Data.
- PASS Security: demo credentials are explicitly local/demo-only, and password
  seed expectations are treated as setup concerns rather than shortcuts for
  production authentication.
- PASS Scope: the plan stays limited to current-use-case tables and explicitly
  defers permissions, modules, settings, role-permission behavior, repository
  code, API wiring, and cloud deployment.
- PASS API consistency: no HTTP contract changes are introduced in this slice;
  API-facing concerns remain deferred.

## Project Structure

### Documentation (this feature)

```text
specs/008-postgres-db-setup/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── database-bootstrap-workflow.md
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

**Structure Decision**: This slice should primarily touch repository-root local
environment files such as `docker-compose.yml`, `database/` SQL artifacts,
documentation, and optional environment example files. Backend source projects
remain unchanged until repository implementation begins.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations are required for this plan | A small Docker Compose plus raw SQL bootstrap flow satisfies the feature without extra framework complexity |
