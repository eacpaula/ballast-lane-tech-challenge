# Feature Specification: Expose Backend API Endpoints for Existing Application Use Cases

**Feature Branch**: `010-api-endpoints`

**Created**: 2026-05-21

**Status**: Draft

## 1. Feature Summary

This feature exposes the already implemented backend use cases through a
consistent HTTP API. The slice covers dependency wiring, authentication and
authorization setup, controller endpoints, request and response DTOs,
ProblemDetails-style failures, OpenAPI visibility, and API-level automated
tests.

## 2. Goal

Provide a reviewable ASP.NET Core Web API surface for the existing blog
platform behaviors so public, authenticated, and administrator flows can be
exercised through HTTP without moving business rules into controllers.

## 3. Functional Requirements

- **FR-001**: The system MUST expose registration and login through Auth API
  endpoints.
- **FR-002**: The system MUST expose public post listing and public post detail
  endpoints that do not require authentication.
- **FR-003**: The system MUST expose a public post reaction endpoint for like
  and dislike submissions.
- **FR-004**: The system MUST expose authenticated post creation, edit, and
  removal endpoints for the current post-management use cases.
- **FR-005**: The system MUST expose administrator-only category creation,
  update, and deactivation endpoints.
- **FR-006**: Controllers MUST delegate business behavior to the existing
  Application use cases and MUST NOT re-implement business rules.
- **FR-007**: The API MUST define explicit request and response DTOs for the
  exposed endpoints.
- **FR-008**: The API MUST provide consistent HTTP status codes and structured
  error responses for validation, authentication, authorization, conflict, not
  found, and unexpected failure cases.
- **FR-009**: The API MUST register the required Application and
  Infrastructure services so the existing use cases can run through HTTP.
- **FR-010**: The API MUST include Swagger or OpenAPI discovery for local API
  exploration.
- **FR-011**: The API MUST include automated API-level tests for public,
  protected, and administrator-only flows.

## 4. API Endpoint Scope

- **Auth**
  - Register a new user account.
  - Log in with existing credentials.
- **Public posts**
  - List publicly readable posts.
  - Read a publicly readable post by id.
- **Public reactions**
  - Submit a like or dislike reaction for a public post.
- **Authenticated posts**
  - Create a new post as the authenticated user.
  - Update an owned post as the authenticated user.
  - Delete or remove an owned post as the authenticated user.
- **Administrator categories**
  - Create a post category.
  - Update a post category.
  - Deactivate a post category.

The feature scope is limited to exposing the current use cases through HTTP.
It does not expand those use cases with new business capabilities.

## 5. Authentication and Authorization Scope

- Authentication MUST be configured for endpoints that require an authenticated
  user.
- Administrator-only endpoints MUST require administrator authorization.
- Public endpoints MUST remain accessible without authentication.
- Authenticated post mutation endpoints MUST rely on the existing Application
  ownership rules rather than controller-only checks.
- Login success MUST return authentication result data suitable for the API’s
  token-based access flow.
- If JWT authentication is not already configured, this feature MUST add the
  minimum configuration required for the protected and admin flows in scope.

## 6. Error Handling Requirements

- Validation failures MUST return ProblemDetails-style responses with clear
  error information.
- Unauthenticated access to protected endpoints MUST return an authentication
  failure response.
- Authenticated but unauthorized access to administrator-only or ownership
  restricted endpoints MUST return a forbidden response.
- Not found or unavailable resource access MUST return a not found style
  response when the use case indicates the target cannot be read or managed.
- Conflicts such as duplicate registration email or duplicate category title
  MUST return conflict-style responses.
- Unexpected failures MUST return a safe server-error response without leaking
  infrastructure internals.

## 7. API Test Scope

- API tests MUST cover successful registration and login flows.
- API tests MUST cover public post list and public post detail flows without
  authentication.
- API tests MUST cover public reaction submission behavior.
- API tests MUST cover authenticated post create, edit, and remove flows.
- API tests MUST cover rejection of unauthenticated requests to protected post
  endpoints.
- API tests MUST cover rejection of non-owner post mutation attempts through
  the API surface.
- API tests MUST cover administrator category management success and non-admin
  rejection flows.
- API tests MUST verify consistent status codes and structured error responses
  for key validation, unauthorized, forbidden, not found, and conflict cases.

## 8. Out of Scope

- Frontend integration
- Frontend screens
- Advanced API versioning
- Refresh tokens
- Password recovery
- Email confirmation
- External identity providers
- Full role or permission management UI
- Advanced logging or observability
- Production deployment
- Entity Framework
- Dapper
- Mediator or MediatR

## 9. Acceptance Criteria

1. **Given** the existing Application use cases, **when** the API feature is
   implemented, **then** HTTP endpoints exist for auth, public posts, post
   reactions, authenticated post management, and administrator category
   management.
2. **Given** a public visitor, **when** they call public post listing, public
   post detail, or post reaction endpoints, **then** the API allows the
   request without requiring authentication.
3. **Given** a protected post-management endpoint, **when** the caller is not
   authenticated, **then** the API rejects the request with an authentication
   failure response.
4. **Given** an administrator-only category endpoint, **when** the caller is
   authenticated but not an administrator, **then** the API rejects the
   request with a forbidden response.
5. **Given** a valid request to any exposed use case, **when** the controller
   delegates to the Application layer, **then** the response reflects the
   existing business outcomes without moving those rules into the controller.
6. **Given** an invalid or conflicting request, **when** the API handles the
   failure, **then** it returns a consistent ProblemDetails-style error
   contract and appropriate HTTP status code.
7. **Given** the API test suite, **when** it is run, **then** it verifies the
   primary public, protected, and administrator flows for the exposed
   endpoints.

## 10. Definition of Done

- A single API-focused specification exists for exposing the current backend
  use cases through ASP.NET Core Web API controllers.
- The specification clearly bounds endpoint scope, authentication and
  authorization behavior, error handling, and API-level testing.
- The specification keeps business rules in the existing Application layer and
  does not shift them into controllers.
- The specification does not expand into frontend work, new business features,
  advanced identity flows, or disallowed libraries.
- No clarification markers remain and the feature is ready for planning.
