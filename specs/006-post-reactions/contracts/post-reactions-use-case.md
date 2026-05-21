# Post Reactions Use Case Contract

## Purpose

This contract defines the Application-layer boundary for the first
implementation increment. It stays independent of HTTP so public reaction rules
can be implemented and tested before API wiring.

## React To Post Contract

### Input

- `PostId`: required target post id
- `ReactionType`: required requested reaction value
- `AuthenticatedUserId`: optional authenticated actor id
- `VisitorIdentifier`: optional anonymous actor id

### Success Contract

On success, the use case returns a `ReactToPostResult` containing:

- `PostId`
- `ReactionType`
- `UserId` when authenticated context is used
- `VisitorIdentifier` when anonymous context is used

### Failure Contract

The use case can fail with one of these business outcomes:

- `PostNotAvailable`: the requested post does not exist, is not public, or is
  not available to the public
- `InvalidReactionType`: the submitted reaction value is not supported
- `InvalidReactionActor`: neither a valid user identifier nor a valid visitor
  identifier was supplied

## Deferred Outer-Boundary Behavior

The following behavior is required by the overall feature but is intentionally
deferred to a later API or persistence increment:

- HTTP endpoint handling for anonymous and authenticated reaction submission
- Translating reaction failures into HTTP responses
- Executing reaction writes with raw SQL and Npgsql
- Rate limiting, spam detection, deduplication, analytics, and reaction history
