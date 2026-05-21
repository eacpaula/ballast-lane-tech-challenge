# Research: Expose Backend API Endpoints for Existing Application Use Cases

## Decision 1: Use thin ASP.NET Core controllers over the existing handlers

- **Decision**: Implement one controller group per endpoint area and delegate
  directly to the existing Application handlers.
- **Rationale**: The existing backend logic is already organized as explicit
  handler classes, so controllers can remain small and easy to explain.
- **Alternatives considered**:
  - Introduce MediatR or another mediator layer: rejected by project
    constraints and unnecessary for the current size.
  - Keep Minimal API endpoints instead of controllers: rejected because the
    feature explicitly targets controller-based API exposure and clearer
    separation of HTTP concerns.

## Decision 2: Use explicit JWT bearer authentication with minimal custom auth support

- **Decision**: Configure JWT bearer authentication with explicit issuer,
  audience, signing key, and expiration settings, and back login payload
  creation with a small Infrastructure authentication factory.
- **Rationale**: This satisfies the constitution while keeping authentication
  support small and aligned with the current login use case.
- **Alternatives considered**:
  - Full external identity integration: rejected as out of scope.
  - Cookie/session auth: rejected because the challenge and constitution are
    aligned to JWT bearer access.

## Decision 3: Use role-based admin authorization plus Application-owned ownership checks

- **Decision**: Enforce administrator-only category access through API
  authorization configuration and continue to rely on Application handlers for
  post ownership validation.
- **Rationale**: Admin role checks are simple at the API boundary, while post
  ownership remains a business rule that already belongs in Application.
- **Alternatives considered**:
  - Put all authorization logic in controller branches: rejected because it
    would duplicate and weaken the business-layer rules.
  - Push every authorization rule into ASP.NET policies: rejected because
    ownership is request-specific business logic already represented in
    Application.

## Decision 4: Map Application outcomes to ProblemDetails in one consistent path

- **Decision**: Add a single consistent API error translation strategy for
  Application result failures and middleware-level failures.
- **Rationale**: The API spec requires stable error behavior, and a single
  mapping path avoids repetitive controller logic.
- **Alternatives considered**:
  - Hand-code status mappings inside every action: rejected because it leads to
    duplication and drift.
  - Return raw Application result objects directly: rejected because the API
    needs HTTP-specific semantics and ProblemDetails-style failures.

## Decision 5: Keep DTOs explicit and local to the API layer

- **Decision**: Create dedicated request and response DTOs for auth, public
  post reads, reactions, protected post mutations, and admin category
  management.
- **Rationale**: Explicit DTOs keep HTTP contracts stable and prevent
  persistence or Application models from leaking into API contracts.
- **Alternatives considered**:
  - Bind directly to Application command types: rejected because commands are
    not HTTP contracts and should remain decoupled from the API layer.
  - Use generic response wrappers everywhere: rejected because the challenge
    values readability over abstraction.

## Decision 6: Lead the slice with API integration tests

- **Decision**: Start with failing API integration tests in
  `tests/backend/BlogPlatform.Api.Tests` for public, protected, and admin
  scenarios.
- **Rationale**: The core risk in this slice is HTTP composition, auth wiring,
  and response mapping, not domain logic.
- **Alternatives considered**:
  - Start with controllers first: rejected because it weakens the
    constitution’s test-first requirement.
  - Use only unit tests around controllers: rejected because they would miss
    middleware, routing, auth, and serialization behavior.

## Decision 7: Keep OpenAPI simple and derived from the implementation

- **Decision**: Keep local OpenAPI discovery enabled and document JWT bearer
  usage when authentication is active.
- **Rationale**: This satisfies the API consistency principle and makes the
  slice easier to inspect during the interview.
- **Alternatives considered**:
  - Delay OpenAPI until later: rejected because the spec explicitly includes it.
  - Add advanced versioning or documentation customizations now: rejected as
    scope creep.
