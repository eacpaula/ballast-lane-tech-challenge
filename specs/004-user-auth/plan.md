# Implementation Plan: User Registration and Login

**Branch**: `004-user-auth` | **Date**: 2026-05-21 | **Spec**: `specs/004-user-auth/spec.md`

**Input**: Feature specification from `/specs/004-user-auth/spec.md`

**Note**: This plan keeps the first implementation increment inside the
Domain and Application layers. API, Infrastructure, JWT middleware, database,
and frontend remain deferred until the registration and login business behavior
is stable and test-covered.

## Summary

Implement user registration and login as a TDD-first backend-core slice.

- Start with failing xUnit tests in `tests/backend/BlogPlatform.Domain.Tests`
  and `tests/backend/BlogPlatform.Application.Tests`.
- Introduce only the minimum production types required to make those tests
  compile: a small user-account Domain concept, register/login commands and
  results, Application handlers, one user repository abstraction, a password
  hashing abstraction, and an authentication payload abstraction.
- Keep field validation and stored-password safety rules in Domain.
- Keep duplicate-email checks, credential verification, and use-case
  orchestration in Application.
- Defer HTTP wiring, raw SQL persistence, and JWT middleware setup to later
  increments.

## Technical Context

**Technical Approach**: Add a small authentication core to the existing
Domain/Application slice with separate registration and login use cases that
validate input, enforce duplicate-email rules, hash passwords before
persistence, verify submitted credentials, and return authentication result
data through abstractions

**Language/Version**: .NET 10 LTS for the first increment; TypeScript exists in
the repo but is not part of this implementation slice

**Layers Affected**: `src/backend/BlogPlatform.Domain`,
`src/backend/BlogPlatform.Application`,
`tests/backend/BlogPlatform.Domain.Tests`, and
`tests/backend/BlogPlatform.Application.Tests`

**Domain/Application Concepts Needed**: `UserAccount` or equivalent Domain
concept for persisted user identity and password hash, registration input
validation for name or username, email, and password, `RegisterUserCommand`,
`RegisterUserResult`, `RegisterUserHandler`, `LoginUserCommand`,
`LoginUserResult`, `LoginUserHandler`, and explicit business outcomes for
duplicate email and invalid credentials

**Repository Abstractions Needed**: a user repository abstraction with the
minimum behaviors required to check whether an email is already used, create a
user, and load a user by email for login; no direct data-access concerns in
Application

**Security Abstractions Needed**: one password hashing abstraction responsible
for hashing registration passwords and verifying login passwords, and one
authentication payload abstraction responsible for producing login success data
without embedding JWT or HTTP concerns into Application

**Testing**: xUnit Domain and Application tests first; API integration and
repository tests deferred until the core use cases are stable

**Target Platform**: ASP.NET Core backend with browser frontend, but this
plan's initial increment is backend-core only

**Project Type**: Full-stack web application with a layered .NET backend

**Implementation Sequence**: failing Domain/Application tests -> minimum compile
surface -> registration orchestration -> login orchestration -> result/refactor
cleanup -> focused test reruns

**Risks and Trade-offs**: introducing a new user/account model increases the
surface area more than recent post slices, but keeping it small avoids a full
identity framework; using abstractions for password and authentication payload
generation keeps security boundaries clear, but means later Infrastructure/API
work must supply concrete implementations; deferring JWT middleware avoids scope
creep, but token transport details stay unresolved in this first increment

**Constraints**: No Entity Framework, Dapper, Mediator, or MediatR; raw SQL
with Npgsql remains an Infrastructure-only concern; passwords must never be
stored in plain text; the first increment excludes API, Infrastructure,
database, JWT middleware, and frontend unless a compile boundary requires
otherwise

**Scale/Scope**: Two closely related use cases for registration and login only,
limited to validation, duplicate-email rejection, password safety, credential
verification, and abstraction-based authentication result generation

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the first executable work is failing Domain/Application
  tests for valid registration, invalid registration, duplicate email, valid
  login, and invalid credential rejection.
- PASS Backend-first: no frontend work is included in the first increment.
- PASS Architecture: Domain holds user/account validation and password-hash
  state safety while Application owns duplicate-email checks, credential
  verification orchestration, and authentication result shaping.
- PASS Data access: there is no SQL in this increment; later persistence work
  remains isolated to `src/backend/BlogPlatform.Infrastructure`.
- PASS Security: password hashing and verification are explicit abstractions,
  plain-text storage is prohibited, and credential failures are treated as
  backend-enforced authentication outcomes.
- PASS Scope: the plan stays limited to registration and login and excludes JWT
  middleware, password recovery, external identity providers, refresh tokens,
  admin user management, and frontend work.
- PASS API consistency: external HTTP contracts are deferred, but business
  outcomes are defined for later mapping to ProblemDetails-style responses.

## Project Structure

### Documentation (this feature)

```text
specs/004-user-auth/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── user-auth-use-cases.md
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
