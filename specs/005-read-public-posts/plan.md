# Implementation Plan: Public Visitor Reads Posts

**Branch**: `005-read-public-posts` | **Date**: 2026-05-21 | **Spec**: `specs/005-read-public-posts/spec.md`

**Input**: Feature specification from `/specs/005-read-public-posts/spec.md`

**Note**: This plan keeps the first implementation increment inside the
Domain and Application layers. API, Infrastructure, database, and frontend
remain deferred until the public read behavior is stable and test-covered.

## Summary

Implement public post listing and public post detail reads as a TDD-first
backend-core slice.

- Start with failing xUnit tests in `tests/backend/BlogPlatform.Application.Tests`
  and add Domain tests only if the post model needs a small expansion for
  public-visibility or availability state.
- Reuse the existing post concept and repository abstraction where that keeps
  the slice smaller and clearer.
- Add only the minimum production types required to make the public-read tests
  compile: read result models, Application handlers or query services, and the
  minimum repository read behaviors needed to list and fetch only public,
  available posts.
- Keep public filtering and not-available outcomes in Application.
- Defer HTTP wiring, raw SQL persistence, and frontend integration to a later
  increment.

## Technical Context

**Technical Approach**: Extend the existing Domain/Application post slice with
Application read use cases that retrieve only public, available posts through
repository abstractions and return public list and public detail results

**Language/Version**: .NET 10 LTS for the first increment; TypeScript exists in
the repo but is not part of this implementation slice

**Layers Affected**: `src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Application.Tests`, and, only if needed to express
public availability state cleanly, `src/backend/BlogPlatform.Domain` and
`tests/backend/BlogPlatform.Domain.Tests`

**Domain/Application Concepts Needed**: existing `BlogPost` concept or a small
read-focused projection with public-availability state, `ListPublicPosts`
result model, `GetPublicPostById` result model, Application read handlers or
query services, and explicit business outcomes for unavailable or non-public
detail reads

**Repository Abstractions Needed**: existing `IPostRepository` expanded only
with the minimum read capabilities required to list public, available posts and
load a single public, available post by id; no direct data-access concerns in
Application

**Testing**: xUnit Application tests first; Domain tests only if the existing
post model gains public-visibility or availability behavior; API integration
and repository tests deferred until the core read use cases are stable

**Target Platform**: ASP.NET Core backend with browser frontend, but this
plan's initial increment is backend-core only

**Project Type**: Full-stack web application with a layered .NET backend

**Implementation Sequence**: failing Application tests -> minimum compile
surface -> public listing behavior -> public detail behavior -> result/refactor
cleanup -> focused test reruns

**Risks and Trade-offs**: the current post model does not yet express
public-visibility or availability state, so the first increment must choose
between a minimal Domain expansion and a read-specific projection; reusing
`IPostRepository` keeps the design smaller but grows the abstraction
incrementally; deferring API and persistence keeps focus high but means HTTP and
query mechanics are validated later

**Constraints**: No Entity Framework, Dapper, Mediator, or MediatR; raw SQL
with Npgsql remains an Infrastructure-only concern; no authentication is
required for this slice; the first increment excludes API, Infrastructure,
database, and frontend unless a compile boundary requires otherwise; no search,
filters, pagination, or advanced sorting in this feature

**Scale/Scope**: Two closely related public read use cases only, limited to
listing public, available posts and reading a single public, available post

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the first executable work is failing Application tests for
  public listing success, filtering out non-public or unavailable posts, public
  detail success, and unavailable detail rejection.
- PASS Backend-first: no frontend work is included in the first increment.
- PASS Architecture: Application owns public filtering outcomes and read
  orchestration; Domain remains unchanged unless a small visibility or
  availability state concern is justified by tests.
- PASS Data access: there is no SQL in this increment; later persistence work
  remains isolated to `src/backend/BlogPlatform.Infrastructure`.
- PASS Security: public reads explicitly require no authentication, and
  non-public or unavailable content is prevented from leaking through backend
  rules.
- PASS Scope: the plan stays limited to public listing and public detail reads
  and excludes mutation behavior, search, pagination, admin behavior, and
  frontend work.
- PASS API consistency: external HTTP contracts are deferred, but business
  outcomes are defined for later mapping to ProblemDetails-style responses.

## Project Structure

### Documentation (this feature)

```text
specs/005-read-public-posts/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── public-read-posts-use-cases.md
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
`tests/backend/BlogPlatform.Application.Tests`, reusing the existing post
concept and repository abstraction where possible. Domain changes should be
introduced only if tests prove they are necessary. API, Infrastructure,
frontend, and database artifacts remain unchanged in this phase.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations are required for this plan | The minimal Application-first design already satisfies the feature |
