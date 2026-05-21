# Remove Owned Post Use Case Contract

## Purpose

This contract defines the Application-layer boundary for the first
implementation increment. It stays independent of HTTP so ownership and removal
rules can be implemented and tested before API wiring.

## Input Contract

### RemoveBlogPostCommand

- `AuthenticatedUserId`: required user id attempting the removal
- `PostId`: required target post id

## Success Contract

On success, the use case returns a `RemoveBlogPostResult` containing:

- `PostId`
- `AuthorUserId`

## Failure Contract

The use case can fail with one of these business outcomes:

- `AuthenticationRequired`: no authenticated user is available
- `PostNotFound`: target post does not exist
- `ForbiddenPostRemoval`: target post is owned by a different user

## Deferred Outer-Boundary Behavior

The following behavior is required by the overall feature but is intentionally
deferred to a later API or persistence increment:

- Rejecting unauthenticated HTTP requests before a command is dispatched
- Translating removal failures into HTTP responses
- Executing the actual delete with raw SQL and Npgsql
- Choosing the final persistence mechanics; if needed later, hard delete is the
  recommended default for this feature because it is the simplest viable option
