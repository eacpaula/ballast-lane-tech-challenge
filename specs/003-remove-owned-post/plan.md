# Implementation Plan: Authenticated User Removes Only Their Own Blog Post

**Branch**: `003-remove-owned-post` | **Date**: 2026-05-21 | **Spec**: `specs/003-remove-owned-post/spec.md`

**Input**: Feature specification from `/specs/003-remove-owned-post/spec.md`

**Note**: This plan keeps the first implementation increment inside the
Domain and Application layers. API, Infrastructure, database, and frontend
remain deferred until the remove-post business behavior is stable and
test-covered.

## Summary

Implement the owned-post removal use case as a TDD-first backend-core slice.

- Start with failing xUnit tests in `tests/backend/BlogPlatform.Application.Tests`
  and add Domain tests only if the existing `BlogPost` concept requires
  removal-specific behavior.
- Reuse the existing post concepts and repository abstraction already introduced
  by the create-post and edit-owned-post slices.
- Add only the minimum production types required to make the remove-post tests
  compile: `RemoveBlogPostCommand`, `RemoveBlogPostResult`,
  `RemoveBlogPostHandler`, and the minimum repository delete behavior needed to
  load and remove a post.
- Keep ownership validation in Application.
- Defer HTTP wiring, SQL persistence, and frontend integration to a later
  increment.

## Technical Context

**Technical Approach**: Extend the existing Domain/Application post slice with
an Application use case that loads a post, verifies authenticated ownership,
and removes the post through repository abstractions

**Language/Version**: .NET 10 LTS for the first increment; TypeScript exists in
the repo but is not part of this implementation slice

**Layers Affected**: `src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Application.Tests`, and, only if needed for shared
invariants or state semantics, `src/backend/BlogPlatform.Domain` and
`tests/backend/BlogPlatform.Domain.Tests`

**Domain/Application Concepts Needed**: existing `BlogPost` entity for loaded
ownership data, `RemoveBlogPostCommand`, `RemoveBlogPostResult`,
`RemoveBlogPostHandler`, authenticated-user ownership comparison, and explicit
business outcomes for authentication required, post not found, and forbidden
removal

**Repository Abstractions Needed**: existing `IPostRepository` expanded only
with the minimum read/delete capabilities required to load a post by id and
remove it; no direct data-access concerns in Application

**Testing**: xUnit Application tests first; Domain tests only if the existing
post model gains removal-related behavior; API integration and repository tests
deferred until the core use case is stable

**Target Platform**: ASP.NET Core backend with browser frontend, but this
plan's initial increment is backend-core only

**Project Type**: Full-stack web application with a layered .NET backend

**Implementation Sequence**: failing Application tests -> minimum compile
surface -> Application ownership/removal behavior -> result/refactor cleanup ->
focused test reruns

**Risks and Trade-offs**: reusing the existing `IPostRepository` keeps the
model simple but grows the abstraction incrementally; choosing a business-level
delete result over exception-driven flow makes tests clearer but requires later
HTTP mapping work; deferring persistence leaves hard-vs-soft delete unresolved
in code, so the plan recommends the simplest later implementation explicitly

**Constraints**: No Entity Framework, Dapper, Mediator, or MediatR; raw SQL
with Npgsql remains an Infrastructure-only concern; first increment excludes
API, Infrastructure, database, and frontend unless a compile boundary requires
otherwise

**Scale/Scope**: Single use case for removing an owned post with authentication
required, missing-post handling, ownership checks, and repository-backed
deletion

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the first executable work is failing Application tests for
  owned-post removal success, missing authenticated user, missing post, and
  non-owner rejection.
- PASS Backend-first: no frontend work is included in the first increment.
- PASS Architecture: Application owns post lookup, ownership checks, and
  orchestration; Domain remains unchanged unless a small shared invariant or
  state concern is justified by tests.
- PASS Data access: there is no SQL in this increment; later persistence work
  remains isolated to `src/backend/BlogPlatform.Infrastructure`.
- PASS Security: ownership validation stays in Application and is treated as the
  main authorization rule for this slice.
- PASS Scope: the plan stays limited to removing an existing owned post and
  excludes creation, editing, reactions, admin behavior, and frontend work.
- PASS API consistency: external HTTP contracts are deferred, but business
  outcomes are defined for later mapping to ProblemDetails-style responses.

## Project Structure

### Documentation (this feature)

```text
specs/003-remove-owned-post/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── remove-owned-post-use-case.md
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

**Structure Decision**: The first implementation increment should touch only
`src/backend/BlogPlatform.Application` and
`tests/backend/BlogPlatform.Application.Tests`, reusing the existing
`BlogPost` entity and `IPostRepository` abstraction. Domain changes should be
introduced only if tests prove they are necessary. API, Infrastructure,
frontend, and database artifacts remain unchanged in this phase.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations are required for this plan | The minimal Application-first design already satisfies the feature |
