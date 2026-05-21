# Implementation Plan: Expose Backend API Endpoints for Existing Application Use Cases

**Branch**: `010-api-endpoints` | **Date**: 2026-05-21 | **Spec**: `specs/010-api-endpoints/spec.md`

**Input**: Feature specification from `/specs/010-api-endpoints/spec.md`

**Note**: This plan focuses on API composition only. It wires the existing
Application use cases and Infrastructure services into ASP.NET Core Web API
controllers, authentication, authorization, DTO mapping, error handling,
OpenAPI, and API-level tests.

## Summary

Expose the existing backend behaviors through thin ASP.NET Core Web API
controllers.

- Start with failing API integration tests for public, protected, and
  administrator-only flows in `tests/backend/BlogPlatform.Api.Tests`.
- Add only the minimum API and Infrastructure composition types required to
  make those tests compile and pass: controller classes, request/response DTOs,
  JWT and authorization configuration, ProblemDetails-style error mapping, and
  Swagger/OpenAPI setup.
- Keep business validation, ownership checks, and category/post authorization
  outcomes inside the existing Application handlers; controllers translate HTTP
  requests into Application calls and map Application results into HTTP
  responses.
- Wire Infrastructure through dependency injection only, including repository
  implementations, password security, and authentication-payload generation.

## Implementation Design

**Technical approach**:

- Use ASP.NET Core Web API controllers grouped by feature area: auth, public
  posts, post reactions, authenticated posts, and admin categories.
- Keep controller methods thin and explicit: parse route/body input, obtain
  authenticated user context, call the relevant Application handler, and map
  the result to HTTP.
- Add API integration tests first so the route surface, authentication
  behavior, and ProblemDetails responses are proven before controller logic is
  considered complete.

**Layers affected**:

- `src/backend/BlogPlatform.Api`
- `tests/backend/BlogPlatform.Api.Tests`
- minimal `src/backend/BlogPlatform.Infrastructure` additions for
  `IPasswordSecurityService`, `IAuthenticationPayloadFactory`, and DI
  composition if those services are not already available on the branch
- no planned changes to `src/backend/BlogPlatform.Domain` or
  `src/backend/BlogPlatform.Application`
- no planned frontend work

**Dependency injection strategy**:

- Register Application handlers as explicit service types rather than
  introducing Mediator-style orchestration.
- Register Infrastructure repositories and auth support services behind the
  existing Application abstractions.
- Keep connection settings, JWT settings, and auth support owned by API or
  Infrastructure configuration, not by Application.
- Prefer one explicit composition path in `Program.cs` or a small API
  composition extension rather than broad startup abstractions.

**Authentication strategy**:

- Use ASP.NET Core JWT bearer authentication with explicit issuer, audience,
  signing key, and expiration configuration.
- Treat the existing login use case result as the source for token-bearing
  response data and back it with an Infrastructure authentication-payload
  factory.
- Keep login credential validation in Application; JWT middleware only governs
  access to protected endpoints.

**Authorization strategy**:

- Use public endpoints by default where no authentication is required.
- Use authenticated access for post mutation endpoints.
- Use an administrator requirement for category management endpoints, most
  likely via role-based authorization from JWT claims.
- Continue to rely on Application handlers for ownership validation and
  non-trivial business authorization outcomes such as editing or deleting only
  owned posts.

**Controller and API endpoint strategy**:

- Auth controller:
  - register
  - login
- Public posts controller:
  - list public posts
  - get public post by id
- Public reactions controller:
  - react to a public post
- Protected posts controller:
  - create post
  - edit own post
  - remove own post
- Admin categories controller:
  - create category
  - update category
  - deactivate category
- Route patterns and verbs should stay conventional and explicit rather than
  introducing advanced versioning or generic controller bases.

**DTO mapping strategy**:

- Define API request DTOs for each endpoint payload rather than binding
  controllers directly to Application commands.
- Define response DTOs for successful auth, post list/detail, post mutation,
  reaction, and category management outcomes.
- Map authenticated user context from JWT claims into Application command input
  at the controller boundary.
- Keep DTO mapping simple and local to the API layer.

**Error handling strategy**:

- Add one consistent ProblemDetails-style mapping path for validation, not
  found, unauthorized, forbidden, conflict, and unexpected errors.
- Map Application result error codes to stable HTTP status codes without moving
  rule logic into controllers.
- Preserve safe error messages for unexpected failures and avoid leaking
  connection, SQL, or token internals.

**Swagger/OpenAPI strategy**:

- Keep OpenAPI generation enabled for local exploration.
- Document JWT bearer usage in Swagger if authentication is active for the API
  slice.
- Ensure the documented routes reflect the actual controller implementation and
  explicit DTOs.

**API testing strategy**:

- Use ASP.NET Core API integration tests in
  `tests/backend/BlogPlatform.Api.Tests`.
- Cover:
  - successful registration and login
  - public post list and detail reads
  - public reaction submission
  - authenticated post creation, edit, and removal
  - unauthenticated rejection for protected endpoints
  - non-owner rejection for protected post mutation
  - administrator success and non-admin rejection for category management
  - ProblemDetails-style responses for validation, conflict, not found, and
    forbidden outcomes
- Use the configured PostgreSQL environment for end-to-end API behavior where
  the test host needs real persistence.

**Validation strategy**:

- Prove the HTTP contract through failing-then-passing API integration tests.
- Re-run Application and Infrastructure tests as regression checks when API
  composition touches shared auth or persistence services.
- Confirm Swagger/OpenAPI generation still works after authentication and
  controller registration are added.

**Risks and trade-offs**:

- The branch currently shows only the default API template, so the first API
  slice has to establish controllers, test host setup, and auth wiring at the
  same time.
- JWT and role claim design must stay minimal; overengineering the auth system
  would add interview noise without improving the MVP.
- ProblemDetails mapping can become repetitive if spread across controllers, so
  the plan favors a single consistent error translation path.
- API integration tests against real persistence are more representative, but
  they require disciplined test setup and configuration.

## Technical Context

**Language/Version**: .NET 10 LTS

**Primary Dependencies**: ASP.NET Core Web API, JWT bearer authentication,
OpenAPI/Swagger support, xUnit, existing Application handlers, existing or
planned Infrastructure auth and repository services

**Storage**: PostgreSQL through existing Infrastructure abstractions and raw SQL
repositories

**Testing**: xUnit API integration tests in `tests/backend/BlogPlatform.Api.Tests`,
plus regression runs for Application and Infrastructure test suites

**Target Platform**: ASP.NET Core backend exposing HTTP endpoints for a browser
or API client

**Project Type**: Full-stack web application with a layered .NET backend

**Performance Goals**: Keep API behavior acceptable for interview-scale demo
usage; optimize for correctness, clarity, and consistent failure handling
before advanced tuning

**Constraints**: Thin controllers, explicit DTOs, ProblemDetails-style error
responses, JWT-based protected access, Application-owned business validation, no
Entity Framework, Dapper, Mediator, or MediatR

**Scale/Scope**: One API slice covering auth, public posts, public reactions,
protected post mutation, and admin category management for the existing MVP

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the plan starts with failing API integration tests for
  public, protected, and administrator flows before controller implementation.
- PASS Backend-first: the slice is limited to API and supporting backend
  composition only; no frontend work is included.
- PASS Architecture: controllers remain thin, Application stays independent
  from API and Infrastructure, and Infrastructure is wired only through DI.
- PASS Data access: no new persistence pattern is introduced; repositories
  remain behind Application abstractions and raw SQL/Npgsql stays in
  Infrastructure.
- PASS Security: JWT configuration, protected endpoint rejection, admin-only
  access, and ownership-preserving endpoint behavior are explicitly planned.
- PASS Scope: the plan exposes only the already approved use cases and avoids
  broader identity, versioning, or CMS expansions.
- PASS API consistency: explicit DTOs, ProblemDetails-style responses, and
  OpenAPI derived from implemented endpoints are first-class concerns.

## Project Structure

### Documentation (this feature)

```text
specs/010-api-endpoints/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── api-http-contracts.md
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
```

**Structure Decision**: This slice should primarily touch
`src/backend/BlogPlatform.Api` and `tests/backend/BlogPlatform.Api.Tests`, with
minimal supporting Infrastructure changes for auth-support services and DI
registration if those are not already present.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | No constitutional violations are required for this plan | Thin controllers plus explicit DI and tests satisfy the feature without extra frameworks |
