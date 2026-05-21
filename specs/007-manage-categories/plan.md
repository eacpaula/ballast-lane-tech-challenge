# Implementation Plan: Administrator Manages Post Categories

**Branch**: `007-manage-categories` | **Date**: 2026-05-21 | **Spec**: `specs/007-manage-categories/spec.md`

**Input**: Feature specification from `/specs/007-manage-categories/spec.md`

**Note**: This plan keeps the first implementation increment inside the
Domain and Application layers. API, Infrastructure, JWT middleware, database,
and frontend remain deferred until the category-management behavior is stable
and test-covered.

## Summary

Implement administrator category management as a TDD-first backend-core slice.

- Start with failing xUnit tests in `tests/backend/BlogPlatform.Domain.Tests`
  and `tests/backend/BlogPlatform.Application.Tests`.
- Introduce only the minimum production types required to make those tests
  compile: a small category Domain concept, create/update/deactivate commands
  and results, Application handlers, a category repository abstraction, and a
  small Application-level authorization input for admin checks.
- Keep title validation and normalized category state in Domain.
- Keep administrator authorization, duplicate-title checks, missing-category
  outcomes, and deactivation orchestration in Application.
- Recommend deactivation over hard delete for this challenge because the schema
  already models category availability and deactivation avoids breaking post
  references while keeping the slice small.
- Defer HTTP wiring, raw SQL persistence, and frontend integration to later
  increments.

## Technical Context

**Technical Approach**: Extend the existing Domain/Application slice with
administrator-only category management use cases that validate titles, reject
duplicate names, mark categories unavailable instead of deleting them, and
persist category changes through repository abstractions

**Language/Version**: .NET 10 LTS for the first increment; TypeScript exists in
the repo but is not part of this implementation slice

**Layers Affected**: `src/backend/BlogPlatform.Domain`,
`src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Domain.Tests`, and
`tests/backend/BlogPlatform.Application.Tests`

**Domain/Application Concepts Needed**: a small `PostCategory` Domain concept
or equivalent with title normalization and availability state,
`CreatePostCategoryCommand`, `CreatePostCategoryResult`,
`CreatePostCategoryHandler`, `UpdatePostCategoryCommand`,
`UpdatePostCategoryResult`, `UpdatePostCategoryHandler`,
`DeactivatePostCategoryCommand`, `DeactivatePostCategoryResult`,
`DeactivatePostCategoryHandler`, and explicit business outcomes for invalid
title, duplicate title, missing category, and non-admin failures

**Authorization Concepts Needed**: reuse the existing authenticated-user style
input pattern with an Application-level admin indicator or equivalent admin
authorization context; do not introduce full role, permission, or module
management in this slice

**Repository Abstractions Needed**: expand the current category repository
abstraction from simple availability checks to include the minimum behaviors
needed to create, load, update, deactivate, and check duplicate titles; no
direct data-access concerns in Application

**Testing**: xUnit Domain and Application tests first; API integration and
repository tests deferred until the core category-management use cases are
stable

**Target Platform**: ASP.NET Core backend with browser frontend, but this
plan's initial increment is backend-core only

**Project Type**: Full-stack web application with a layered .NET backend

**Implementation Sequence**: failing Domain/Application tests -> minimum compile
surface -> category Domain invariants -> create/update/deactivate Application
orchestration -> result/refactor cleanup -> focused test reruns

**Risks and Trade-offs**: deactivation is simpler and safer than hard delete
because categories may already be referenced by posts, but it leaves eventual
cleanup policy for later; introducing a new category Domain concept adds code
surface, but keeps title validation and availability state explicit; using a
small admin indicator in Application keeps the slice simple, but defers richer
authorization claims mapping to later API/auth integration

**Constraints**: No Entity Framework, Dapper, Mediator, or MediatR; raw SQL
with Npgsql remains an Infrastructure-only concern; the first increment
excludes API, Infrastructure, database, JWT middleware, and frontend unless a
compile boundary requires otherwise

**Scale/Scope**: Three closely related administrator-only use cases for create,
update, and deactivate category behavior only, limited to title validation,
duplicate-title rejection, admin checks, and repository-based persistence

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Test-first: Does each backend story identify the failing unit, integration,
  and repository tests that must exist before production code?
- Backend-first: Is backend API, business logic, persistence, and security work
  sequenced ahead of dependent frontend work?
- Architecture: Do planned changes preserve clear boundaries between Domain,
  Application, Infrastructure/Data, API, Tests, and Frontend?
- Data access: Is all persistence work limited to parameterized raw SQL with
  Npgsql in Infrastructure/Data only?
- Security: Are authentication, admin authorization, ownership rules, and
  protected endpoint rejection covered by planned tests?
- Scope: Does the plan stay within the approved MVP and avoid CMS-style
  expansions?
- API consistency: Will validation and failures use ProblemDetails-style
  responses, explicit DTOs, and OpenAPI derived from implementation?

- PASS Test-first: the first executable work is failing Domain/Application
  tests for valid category creation, valid category update, valid category
  deactivation, invalid title rejection, duplicate-title rejection, missing
  category rejection, and non-admin rejection.
- PASS Backend-first: no frontend work is included in the first increment.
- PASS Architecture: Domain owns category title and availability invariants
  while Application owns admin authorization checks, duplicate-title checks,
  missing-category outcomes, and repository orchestration.
- PASS Data access: there is no SQL in this increment; later persistence work
  remains isolated to `src/backend/BlogPlatform.Infrastructure`.
- PASS Security: category management is explicitly admin-only, and non-admin
  rejection is treated as backend-enforced authorization behavior.
- PASS Scope: the plan stays limited to category management and excludes full
  role management, user management, hierarchy, slugging, SEO, and frontend
  work.
- PASS API consistency: external HTTP contracts are deferred, but business
  outcomes are defined for later mapping to ProblemDetails-style responses.

## Project Structure

### Documentation (this feature)

```text
specs/007-manage-categories/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── manage-post-categories-use-cases.md
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

**Structure Decision**: The first implementation increment should touch
`src/backend/BlogPlatform.Domain`, `src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Domain.Tests`, and
`tests/backend/BlogPlatform.Application.Tests`. API, Infrastructure, frontend,
and database artifacts remain unchanged in this phase.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations are required for this plan | The minimal Domain/Application-first design already satisfies the feature |
