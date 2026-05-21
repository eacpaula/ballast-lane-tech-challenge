# Implementation Plan: Public Visitor Reacts To Posts

**Branch**: `006-post-reactions` | **Date**: 2026-05-21 | **Spec**: `specs/006-post-reactions/spec.md`

**Input**: Feature specification from `/specs/006-post-reactions/spec.md`

**Note**: This plan keeps the first implementation increment inside the
Domain and Application layers. API, Infrastructure, database, and frontend
remain deferred until the public reaction behavior is stable and test-covered.

## Summary

Implement public post reactions as a TDD-first backend-core slice.

- Start with failing xUnit tests in `tests/backend/BlogPlatform.Domain.Tests`
  and `tests/backend/BlogPlatform.Application.Tests`.
- Reuse the existing public-post eligibility model through
  `BlogPost.IsPubliclyReadable`.
- Add only the minimum production types required to make the reaction tests
  compile: small Domain reaction concepts, a reaction command and result,
  an Application handler, a post repository read dependency, and a dedicated
  reaction repository abstraction.
- Keep reaction-type validation and actor-identity invariants in Domain.
- Keep public-post checks, missing-post outcomes, and persistence orchestration
  in Application.
- Defer HTTP wiring, raw SQL persistence, spam controls, analytics, and
  frontend integration to later increments.

## Technical Context

**Technical Approach**: Extend the existing Domain/Application post slice with
a public reaction use case that validates like or dislike input, validates a
simple actor identity, confirms the target post is publicly readable, and
persists an accepted reaction through repository abstractions

**Language/Version**: .NET 10 LTS for the first increment; TypeScript exists in
the repo but is not part of this implementation slice

**Layers Affected**: `src/backend/BlogPlatform.Domain`,
`src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Domain.Tests`, and
`tests/backend/BlogPlatform.Application.Tests`

**Domain/Application Concepts Needed**: existing `BlogPost` concept for public
availability checks, a small `PostReaction` Domain concept or equivalent,
`ReactionType`, a simple actor-identity boundary such as `ReactionActor`,
`ReactToPostCommand`, `ReactToPostResult`, `ReactToPostHandler`, and explicit
business outcomes for invalid reaction type, invalid actor identity, and
post-not-available failures

**Repository Abstractions Needed**: existing `IPostRepository` reused for
loading the target public post, plus a dedicated reaction repository
abstraction with the minimum behavior needed to create a reaction record; no
direct data-access concerns in Application

**Testing**: xUnit Domain and Application tests first; API integration and
repository tests deferred until the core reaction use case is stable

**Target Platform**: ASP.NET Core backend with browser frontend, but this
plan's initial increment is backend-core only

**Project Type**: Full-stack web application with a layered .NET backend

**Implementation Sequence**: failing Domain/Application tests -> minimum compile
surface -> Domain reaction invariants -> Application reaction orchestration ->
result/refactor cleanup -> focused test reruns

**Risks and Trade-offs**: keeping visitor identification abstract and simple
keeps the slice small, but does not solve spam or deduplication concerns;
introducing a small reaction Domain concept adds surface area, but makes actor
and reaction-type rules explicit and testable; using a separate reaction
repository keeps responsibilities clearer than overloading `IPostRepository`,
but adds one more interface to the Application layer

**Constraints**: No Entity Framework, Dapper, Mediator, or MediatR; raw SQL
with Npgsql remains an Infrastructure-only concern; no authentication is
required for anonymous reactions; the first increment excludes API,
Infrastructure, database, anti-spam, analytics, and frontend unless a compile
boundary requires otherwise

**Scale/Scope**: One public reaction use case only, limited to submitting a
like or dislike for a public, available post with simple actor association

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
  tests for valid like acceptance, valid dislike acceptance, invalid reaction
  rejection, invalid actor rejection, and post-not-available rejection.
- PASS Backend-first: no frontend work is included in the first increment.
- PASS Architecture: Domain owns reaction-type and actor invariants while
  Application owns public-post checks, missing-post outcomes, and repository
  orchestration.
- PASS Data access: there is no SQL in this increment; later persistence work
  remains isolated to `src/backend/BlogPlatform.Infrastructure`.
- PASS Security: anonymous reactions explicitly require no authentication, but
  public-post eligibility and actor input validity are still backend-enforced.
- PASS Scope: the plan stays limited to one reaction submission use case and
  excludes analytics, anti-spam, history, frontend work, and broader identity
  behavior.
- PASS API consistency: external HTTP contracts are deferred, but business
  outcomes are defined for later mapping to ProblemDetails-style responses.

## Project Structure

### Documentation (this feature)

```text
specs/006-post-reactions/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── post-reactions-use-case.md
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
