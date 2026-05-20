<!--
Sync Impact Report
Version change: unversioned template -> 1.0.0
Modified principles:
- Template Core Principle 1 -> I. Test-First Development (Non-Negotiable)
- Template Core Principle 2 -> II. Backend-First Delivery
- Template Core Principle 3 -> III. Lightweight Clean Architecture
- Template Core Principle 4 -> IV. Custom Data Access Discipline
- Template Core Principle 5 -> V. Security and Authorization
- added VI. Scope-Controlled MVP
- added VII. Reviewable Interview-Quality Code
- added VIII. Consistent API Behavior
- added IX. Practical Test Strategy
Added sections:
- Delivery Constraints
- Spec and Task Enforcement
Removed sections:
- None
Templates requiring updates:
- ✅ updated .specify/templates/plan-template.md
- ✅ updated .specify/templates/spec-template.md
- ✅ updated .specify/templates/tasks-template.md
- ✅ no command templates present under .specify/templates/commands/
Follow-up TODOs:
- None
-->
# Ballast Lane Tech Challenge Constitution

## Core Principles

### I. Test-First Development (Non-Negotiable)
Every backend feature MUST begin with failing tests before production code is
written. Tests MUST describe expected behavior from user stories and business
rules, with priority on ownership, authorization, validation, and error cases.
Production code MUST NOT be introduced until the relevant unit, integration, or
repository tests exist and demonstrably fail. Rationale: the challenge is meant
to show deliberate engineering discipline, not post-hoc test decoration.

### II. Backend-First Delivery
Specifications, plans, and task lists MUST prioritize backend API behavior,
domain rules, authentication, authorization, persistence, and automated tests
before frontend implementation. Frontend work SHOULD start only after the core
backend flow for that story is stable, test-covered, and contractually clear.
Rationale: this repository is evaluated primarily on correctness of business
rules and API behavior, not on premature UI breadth.

### III. Lightweight Clean Architecture
The codebase MUST preserve clear boundaries between Domain, Application,
Infrastructure/Data, API, Tests, and Frontend layers. Controllers MUST handle
HTTP concerns only; business validation and non-trivial authorization decisions
MUST live outside controllers. The architecture MUST stay lightweight: MediatR,
CQRS, generic repositories, and comparable abstraction-heavy patterns MUST NOT
be introduced unless the constitution is amended first. Rationale: interview
quality improves when dependencies are explicit and the design remains easy to
explain.

### IV. Custom Data Access Discipline
Persistence MUST use raw SQL with Npgsql only. Entity Framework, Dapper, and
other ORM or micro-ORM libraries MUST NOT be introduced. All SQL MUST be
parameterized and kept inside Infrastructure/Data repositories or gateways.
Application and Domain code MUST NOT depend on Npgsql types, SQL text, or table
layout details. Rationale: the challenge explicitly values direct persistence
control while preserving maintainable boundaries.

### V. Security and Authorization
Passwords MUST be securely hashed and MUST NEVER be stored or logged in plain
text. JWT bearer authentication MUST use explicit issuer, audience, signing key,
and expiration configuration. Protected endpoints MUST reject unauthenticated
requests, admin endpoints MUST reject non-admin users, and post mutation
operations MUST enforce ownership rules in the backend. Frontend checks MAY
improve UX but MUST NOT be treated as authorization. Rationale: security and
authorization correctness are core evaluation criteria for this exercise.

### VI. Scope-Controlled MVP
The MVP MUST remain intentionally small. It MUST include registration, login,
public post listing and details, post CRUD, ownership validation, like/dislike,
tag creation, and admin category management. It MUST NOT include comments, image
uploads, rich text editing, password recovery, external identity providers,
notifications, analytics, scheduling, or broader CMS behavior unless the scope
is explicitly expanded later. Rationale: disciplined scope control is necessary
to finish, test, and explain the solution well.

### VII. Reviewable Interview-Quality Code
Code MUST favor readability, explicitness, and straightforward control flow over
clever abstractions. Generated code MUST be manually reviewed before acceptance.
Important implementation decisions SHOULD stay aligned with
`docs/DotNET - Technical Test - Informal User Story.md` and
`docs/DotNET - Technical Test - Architectural Decisions.md`. Rationale: the
repository must support technical discussion and review under interview
conditions.

### VIII. Consistent API Behavior
The API MUST return ProblemDetails-style responses for validation failures, not
found results, unauthorized access, forbidden access, conflicts, and unexpected
errors. DTOs MUST remain explicit rather than exposing persistence models
directly. OpenAPI documentation MUST be generated from the implemented API so it
stays aligned with real behavior. Rationale: consistent contracts improve
frontend integration, testing, and review clarity.

### IX. Practical Test Strategy
Unit tests MUST cover domain and application business rules. API integration
tests MUST cover public, protected, and admin-only endpoint behavior. Repository
tests SHOULD cover critical raw SQL behavior against PostgreSQL where practical,
especially around mapping, filtering, and write safety. The test suite MUST
prioritize high-risk behavior over superficial coverage targets. Rationale:
useful tests prove the architecture and rules that matter most in this challenge.

## Delivery Constraints

The implementation MUST use .NET 10 LTS for the backend, ASP.NET Core Web API
controllers for HTTP endpoints, PostgreSQL for persistence, JWT bearer
authentication, xUnit for backend testing, and React + Vite + TypeScript for the
frontend. The intended backend shape is a lightweight Clean Architecture split
across Domain, Application, Infrastructure/Data, API, and Tests. Any proposal to
change the stack or introduce a new core library MUST show a concrete reduction
in risk or complexity and MUST NOT violate the core principles above.

## Spec and Task Enforcement

Future specifications, plans, and tasks MUST enforce test-first development
explicitly.

- Specs MUST identify the user story behavior, business rules, negative cases,
  and the backend tests that will prove them before implementation begins.
- Plans MUST include a constitution check that verifies failing-test sequencing,
  backend-first delivery, architecture boundary compliance, security coverage,
  and scope control.
- Tasks MUST place backend test tasks before the related implementation tasks and
  MUST schedule frontend tasks only after the supporting backend flow is stable
  and test-covered.
- Any task that bypasses tests, weakens backend authorization, or expands scope
  beyond the approved MVP MUST be treated as non-compliant until corrected.

## Governance

This constitution supersedes conflicting local habits, ad hoc feature expansion,
and template defaults. Changes MUST be made by updating this file together with
any affected Spec-Kit templates or guidance documents in the same change set.

Amendments follow semantic versioning:

- MAJOR: removes a principle, materially weakens an obligation, or redefines a
  governance rule incompatibly.
- MINOR: adds a principle or section, or materially expands project obligations.
- PATCH: clarifies wording without changing substantive expectations.

Compliance MUST be reviewed during specification, planning, task generation, and
code review. A feature or change that conflicts with this constitution MUST be
revised or explicitly justified through a constitution amendment before work
continues.

**Version**: 1.0.0 | **Ratified**: 2026-05-20 | **Last Amended**: 2026-05-20
