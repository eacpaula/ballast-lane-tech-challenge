# Implementation Plan: Authenticated User Creates a Blog Post

**Branch**: `001-create-blog-post` | **Date**: 2026-05-20 | **Spec**: `specs/001-create-blog-post/spec.md`

**Input**: Feature specification from `/specs/001-create-blog-post/spec.md`

**Note**: This plan intentionally keeps the first implementation increment inside
the Domain and Application layers. API, Infrastructure, and frontend follow only
after the core use case is stable and test-covered.

## Summary

Implement the create-post use case with a TDD-first backend-core slice.

- Start with failing xUnit tests in `tests/backend/BlogPlatform.Domain.Tests`
  and `tests/backend/BlogPlatform.Application.Tests`.
- Add only the minimum production types required to make those tests compile:
  `CreateBlogPostCommand`, `CreateBlogPostResult`, `CreateBlogPostHandler`,
  `BlogPost`, `IPostRepository`, and `ICategoryRepository`.
- Put invariant enforcement in Domain and orchestration in Application.
- Defer HTTP endpoint wiring, raw SQL persistence, and frontend integration to a
  later increment.

## Technical Context

**Language/Version**: .NET 10 LTS for the first increment; TypeScript exists in
the repo but is not part of this implementation slice

**Primary Dependencies**: xUnit for test-first work; existing .NET class library
projects in `src/backend`

**Storage**: PostgreSQL exists at system level, but the first increment uses
repository abstractions only and does not touch concrete persistence

**Testing**: xUnit Domain and Application tests first; API integration and
repository tests deferred until the core use case is stable

**Target Platform**: ASP.NET Core backend with browser frontend, but this plan's
initial increment is backend-core only

**Project Type**: Full-stack web application with a layered .NET backend

**Performance Goals**: Normal interactive create-post behavior for a small demo
application; no special optimization work in the first increment

**Constraints**: No Entity Framework, Dapper, Mediator, or MediatR; raw SQL with
Npgsql remains an Infrastructure-only concern; first increment excludes API,
Infrastructure, and frontend unless a compile boundary requires otherwise

**Scale/Scope**: Single use case for authenticated post creation with minimal
validation and category existence checks

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the first executable work is failing Domain/Application tests
  for valid creation, invalid content, and invalid category behavior.
- PASS Backend-first: no frontend work is included in the first increment.
- PASS Architecture: Domain holds post invariants, Application coordinates the
  use case, and persistence remains behind repository abstractions.
- PASS Data access: there is no SQL in this increment; later persistence work
  remains isolated to `src/backend/BlogPlatform.Infrastructure`.
- PASS Security: this increment assumes an already authenticated user id at the
  Application boundary and defers unauthenticated endpoint rejection to a later
  API increment without weakening the requirement.
- PASS Scope: the plan stays limited to post creation and excludes editing,
  deleting, reactions, admin category management, and frontend behavior.
- PASS API consistency: the external API is documented as a deferred contract,
  but no HTTP implementation is introduced before core behavior is proven.

## Project Structure

### Documentation (this feature)

```text
specs/001-create-blog-post/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── create-blog-post-use-case.md
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

**Structure Decision**: The first implementation increment touches only
`src/backend/BlogPlatform.Domain`, `src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Domain.Tests`, and
`tests/backend/BlogPlatform.Application.Tests`. API, Infrastructure, frontend,
and database artifacts remain unchanged in this phase.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations are required for this plan | The minimal Domain/Application-first design already satisfies the feature |
