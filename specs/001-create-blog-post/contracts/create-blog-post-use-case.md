# Create Blog Post Use Case Contract

## Purpose

This contract defines the Application-layer boundary for the first implementation
increment. It is intentionally independent of HTTP so the create-post behavior
can be implemented and tested before API wiring.

## Input Contract

### CreateBlogPostCommand

- `AuthenticatedUserId`: required owner id for the post being created
- `CategoryId`: required target category id
- `Title`: required non-blank text
- `Summary`: optional descriptive text for the first increment
- `Content`: required non-blank text

## Success Contract

On success, the use case returns a `CreateBlogPostResult` containing:

- `PostId`
- `AuthorUserId`
- `CategoryId`
- `Title`
- `Summary`
- `Content`

## Failure Contract

The use case can fail with one of these business outcomes:

- `ValidationError`: title or content is blank
- `CategoryNotFoundOrUnavailable`: selected category cannot be used

## Deferred Outer-Boundary Behavior

The following behavior is required by the overall feature but is intentionally
deferred to a later API increment:

- Rejecting unauthenticated HTTP requests before a command is dispatched
- Translating business failures into HTTP responses
- Persisting the post with raw SQL and Npgsql
