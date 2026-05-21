# Feature Specification: Edit Owned Blog Post

**Feature Branch**: `002-edit-owned-post`

**Created**: 2026-05-20

**Status**: Draft

## Feature Summary

This feature allows an authenticated user to update an existing blog post only
when that post belongs to them. The feature is limited to editing the post’s
title, summary, and content/body for an existing post record. It assumes
authentication already exists and that persistence access is provided through
repository abstractions.

## User Story

As an authenticated user, I want to edit a blog post that I created so that I
can correct or improve my published content without changing posts owned by
other users.

## Functional Requirements

- **FR-001**: The system MUST allow an authenticated user to submit updated
  title, summary, and content/body values for an existing blog post.
- **FR-002**: The system MUST reject edit requests when the requester is not
  authenticated.
- **FR-003**: The system MUST load the existing post before applying updates.
- **FR-004**: The system MUST reject the update when the target post does not
  exist.
- **FR-005**: The system MUST reject the update when the authenticated user does
  not own the target post.
- **FR-006**: The system MUST validate that the updated title is present and not
  blank.
- **FR-007**: The system MUST validate that the updated content/body is present
  and not blank.
- **FR-008**: The system MUST persist valid updates through repository
  abstractions only.
- **FR-009**: The system MUST return a clear success result for a valid update
  and a clear failure result for unauthorized, invalid, or missing-post cases.

## Business Rules

- Only authenticated users may edit blog posts.
- A user may edit only a post that they own.
- Ownership validation MUST happen in the Application layer.
- An updated post requires a non-empty title.
- An updated post requires non-empty content/body.
- Business validation MUST stay outside API controllers.
- Application logic MUST remain independent from Infrastructure/Data details.
- Persistence access for this feature MUST occur through repository
  abstractions.

## Acceptance Criteria

1. **Given** an authenticated user who owns an existing post, **when** the user
   submits valid updated title and content/body values, **then** the system
   saves the changes and confirms success.
2. **Given** an unauthenticated requester, **when** an edit-post request is
   submitted, **then** the system rejects the request.
3. **Given** an authenticated user, **when** the target post does not exist,
   **then** the system rejects the request.
4. **Given** an authenticated user, **when** the target post belongs to another
   user, **then** the system rejects the request.
5. **Given** an authenticated user editing their own post, **when** the updated
   title is blank or the updated content/body is blank, **then** the system
   rejects the request with a validation error.

## Out of Scope

- User registration, login, JWT issuance, or authentication implementation
- Blog post creation
- Blog post deletion
- Post reactions or comment behavior
- Administrator category management
- Frontend forms, pages, or client-side workflows
- Full database migration implementation
- Full tag or category management
- Full role or permission management

## Test-First Notes

- The first implementation increment SHOULD start with failing Application and
  Domain tests for successful owned-post updates, non-owner rejection, missing
  authenticated user rejection, missing-post rejection, and invalid title/body
  validation.
- Tests SHOULD prove ownership enforcement before any outer-layer API wiring is
  added.
- Production code SHOULD be introduced only after the relevant failing tests are
  in place.
- Repository behavior for this slice SHOULD be expressed as abstractions first,
  with Infrastructure persistence deferred until the business behavior is stable.

## Definition of Done

- The feature remains limited to editing an existing owned post.
- Functional requirements and business rules are explicit enough to guide a
  TDD-first implementation.
- Acceptance criteria cover the happy path, unauthenticated access, missing
  post, non-owner access, and invalid title/content cases.
- Ownership validation is clearly identified as an Application-layer
  responsibility.
- The specification does not expand into creation, deletion, reactions, admin
  management, frontend work, or authentication implementation details.
- No open clarification markers remain in the specification.
