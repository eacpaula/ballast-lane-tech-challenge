# Feature Specification: Public Visitor Reacts To Posts

**Feature Branch**: `006-post-reactions`

**Created**: 2026-05-21

**Status**: Draft

## Feature Summary

This feature allows a visitor to react to a public blog post with either a like
or a dislike. The slice is limited to submitting a single reaction against a
post that exists and is available to the public. The behavior must validate the
target post, validate the reaction type, associate the reaction with the acting
visitor or user identity, and keep persistence behind repository abstractions.

## User Story

As a public visitor, I want to like or dislike a public blog post so that I can
express a simple reaction without needing full account-management behavior in
this feature slice.

## Functional Requirements

- **FR-001**: The system MUST allow an anonymous visitor to submit a like for a
  public and available blog post.
- **FR-002**: The system MUST allow an anonymous visitor to submit a dislike
  for a public and available blog post.
- **FR-003**: The system MUST reject a reaction when the target post does not
  exist.
- **FR-004**: The system MUST reject a reaction when the target post is not
  public.
- **FR-005**: The system MUST reject a reaction when the target post is not
  available.
- **FR-006**: The system MUST reject any reaction type other than like or
  dislike.
- **FR-007**: The system MUST associate each accepted reaction with either a
  visitor identifier or a user identifier, depending on the actor context
  provided to the use case.
- **FR-008**: For anonymous reactions in this slice, the system MUST accept a
  visitor identifier through an abstraction rather than relying on transport
  details.
- **FR-009**: The Application layer MUST expose the post-reaction use case
  independent from API controllers.
- **FR-010**: Post and reaction data access MUST occur through repository
  abstractions only.

## Business Rules

- Reactions are public-facing behavior and must be available without requiring
  login.
- A reaction may be recorded only for a post that exists, is public, and is
  available.
- The only valid reaction values in this slice are like and dislike.
- Reaction validation and public-post eligibility checks MUST happen outside
  API controllers.
- Accepted reactions MUST carry actor identity in one of two forms:
  visitor identifier for anonymous use, or user identifier when authenticated
  context is later supplied.
- The Application layer MUST remain independent from Infrastructure/Data
  details.
- The feature is limited to submitting reactions and must not expand into
  analytics, moderation, or reaction history behavior.

## Acceptance Criteria

1. **Given** a public and available blog post, **when** an anonymous visitor
   submits a like with a valid visitor identifier, **then** the system accepts
   the reaction.
2. **Given** a public and available blog post, **when** an anonymous visitor
   submits a dislike with a valid visitor identifier, **then** the system
   accepts the reaction.
3. **Given** a post that does not exist, is not public, or is unavailable,
   **when** a visitor submits a reaction, **then** the system rejects the
   reaction as not allowed for public interaction.
4. **Given** a reaction value other than like or dislike, **when** a visitor
   submits the reaction, **then** the system rejects it as invalid.
5. **Given** a valid reaction request, **when** the system accepts it, **then**
   the recorded reaction is associated with the provided visitor identifier or
   user identifier.

## Out of Scope

- API controller endpoints
- Frontend reaction buttons or UI state
- Authentication or JWT implementation changes
- Public post reading implementation
- Reaction analytics or reporting
- Rate limiting or anti-spam protection
- IP address, device fingerprinting, or tracking implementation
- Advanced reaction history or undo behavior
- Full database migration implementation
- Comment system

## Test-First Notes

- The first implementation increment SHOULD start with failing Application unit
  tests for valid like acceptance, valid dislike acceptance, invalid reaction
  rejection, and rejection of missing, non-public, or unavailable posts.
- Tests SHOULD prove that anonymous reactions can be submitted without
  authentication while still requiring a valid visitor-identity input.
- Tests SHOULD prove that accepted reactions are persisted through repository
  abstractions only.
- Tests SHOULD prove that the Application layer handles actor association rules
  before any API wiring is introduced.
- Production code SHOULD be added only after the relevant failing tests exist.

## Definition of Done

- The feature remains limited to reacting to public posts with like or dislike.
- Functional requirements and business rules are explicit enough to support a
  TDD-first implementation.
- Acceptance criteria cover valid reactions, invalid reaction types, invalid
  target posts, and actor association behavior.
- Repository-based reaction and post access responsibilities are clear and stay
  outside API controllers.
- The specification does not expand into reading, analytics, spam protection,
  frontend behavior, or full identity-management changes.
- No open clarification markers remain in the specification.
