# Edit Owned Post Use Case Contract

## Purpose

This contract defines the Application-layer boundary for the first
implementation increment. It stays independent of HTTP so ownership and edit
rules can be implemented and tested before API wiring.

## Input Contract

### EditBlogPostCommand

- `AuthenticatedUserId`: required user id attempting the edit
- `PostId`: required target post id
- `Title`: required non-blank updated title
- `Summary`: optional updated summary
- `Content`: required non-blank updated content/body

## Success Contract

On success, the use case returns an `EditBlogPostResult` containing:

- `PostId`
- `AuthorUserId`
- `Title`
- `Summary`
- `Content`

## Failure Contract

The use case can fail with one of these business outcomes:

- `AuthenticationRequired`: no authenticated user is available
- `PostNotFound`: target post does not exist
- `ForbiddenPostEdit`: target post is owned by a different user
- `ValidationError`: title or content/body is blank

## Deferred Outer-Boundary Behavior

The following behavior is required by the overall feature but is intentionally
deferred to a later API or persistence increment:

- Rejecting unauthenticated HTTP requests before a command is dispatched
- Translating edit failures into HTTP responses
- Persisting the edited post with raw SQL and Npgsql
