# Implementation Plan: Authenticated User Edits Only Their Own Blog Post

**Branch**: `002-edit-owned-post` | **Date**: 2026-05-20 | **Spec**: `specs/002-edit-owned-post/spec.md`

**Input**: Feature specification from `/specs/002-edit-owned-post/spec.md`

**Note**: This plan keeps the first implementation increment inside the Domain
and Application layers. API, Infrastructure, and frontend remain deferred until
the edit-post business behavior is stable and test-covered.

## Summary

Implement the owned-post edit use case as a TDD-first backend-core slice.

- Start with failing xUnit tests in `tests/backend/BlogPlatform.Domain.Tests`
  and `tests/backend/BlogPlatform.Application.Tests`.
- Reuse the existing `BlogPost` concept from the create-post feature where that
  reduces duplication and preserves a consistent domain model.
- Add only the minimum production types required to make the edit-post tests
  compile: `EditBlogPostCommand`, `EditBlogPostResult`, `EditBlogPostHandler`,
  and the repository read/update behaviors needed for post lookup and save.
- Keep ownership validation in Application and keep post field invariants in
  Domain.
- Defer HTTP wiring, raw SQL persistence, and frontend integration to a later
  increment.

## Technical Context

**Technical Approach**: Extend the existing Domain/Application post slice with
an edit-post use case that loads a post, verifies ownership, applies validated
changes, and persists the updated post through repository abstractions

**Language/Version**: .NET 10 LTS for the first increment; TypeScript exists in
the repo but is not part of this implementation slice

**Layers Affected**: `src/backend/BlogPlatform.Domain`,
`src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Domain.Tests`, and
`tests/backend/BlogPlatform.Application.Tests`

**Domain/Application Concepts Needed**: `BlogPost` update behavior,
`EditBlogPostCommand`, `EditBlogPostResult`, `EditBlogPostHandler`, ownership
comparison against the authenticated user, and validation for required title and
content/body fields

**Repository Abstractions Needed**: existing `IPostRepository` expanded with the
minimum read/update capabilities required to load a post by id and persist
edits; no direct data-access concerns in Application

**Testing**: xUnit Domain and Application tests first; API integration and
repository tests deferred until the core use case is stable

**Target Platform**: ASP.NET Core backend with browser frontend, but this plan's
initial increment is backend-core only

**Project Type**: Full-stack web application with a layered .NET backend

**Implementation Sequence**: failing Domain/Application tests -> minimum compile
surface -> Application ownership behavior -> validation/result refactor ->
focused test reruns

**Risks and Trade-offs**: reusing the existing `BlogPost` concept reduces
duplication but may require careful evolution of Domain behavior to avoid mixing
create and edit responsibilities; deferring API and Infrastructure keeps focus
high but means unauthenticated HTTP behavior and persistence mechanics are
validated later

**Constraints**: No Entity Framework, Dapper, Mediator, or MediatR; raw SQL
with Npgsql remains an Infrastructure-only concern; first increment excludes
API, Infrastructure, and frontend unless a compile boundary requires otherwise

**Scale/Scope**: Single use case for editing an owned post with ownership
checks, missing-post handling, and required title/content validation

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the first executable work is failing Domain/Application tests
  for owned-post success, missing authenticated user, missing post, non-owner,
  and invalid content behavior.
- PASS Backend-first: no frontend work is included in the first increment.
- PASS Architecture: Domain holds field invariants while Application owns post
  lookup, ownership checks, and orchestration.
- PASS Data access: there is no SQL in this increment; later persistence work
  remains isolated to `src/backend/BlogPlatform.Infrastructure`.
- PASS Security: ownership validation stays in Application and is treated as the
  main authorization rule for this slice.
- PASS Scope: the plan stays limited to editing an existing owned post and
  excludes creation, deletion, reactions, admin behavior, and frontend work.
- PASS API consistency: external HTTP contracts are deferred, but business
  outcomes are defined for later mapping to ProblemDetails-style responses.

## Project Structure

### Documentation (this feature)

```text
specs/002-edit-owned-post/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── edit-owned-post-use-case.md
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
`src/backend/BlogPlatform.Domain`, `src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Domain.Tests`, and
`tests/backend/BlogPlatform.Application.Tests`. Existing create-post types may
be reused or extended where appropriate, but API, Infrastructure, frontend, and
database artifacts remain unchanged in this phase.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations are required for this plan | The minimal Domain/Application-first design already satisfies the feature |
