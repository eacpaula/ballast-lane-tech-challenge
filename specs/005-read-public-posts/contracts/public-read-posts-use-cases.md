# Public Read Posts Use Cases Contract

## Purpose

This contract defines the Application-layer boundaries for the first
implementation increment. It stays independent of HTTP so public list and
detail read rules can be implemented and tested before API wiring.

## Public Listing Contract

### Output

The public listing use case returns a collection of `PublicPostListItem`
entries containing:

- `PostId`
- `Title`
- `Summary`

The listing includes only posts available to public visitors and uses a simple
default ordering.

## Public Detail Contract

### Input

- `PostId`: required target post id

### Success Contract

On success, the use case returns a `GetPublicPostByIdResult` containing:

- `PostId`
- `Title`
- `Summary`
- `Content`

### Failure Contract

The use case can fail with one of these business outcomes:

- `PostNotAvailable`: the requested post does not exist, is not public, or is
  not available to the public

## Deferred Outer-Boundary Behavior

The following behavior is required by the overall feature but is intentionally
deferred to a later API or persistence increment:

- HTTP endpoint handling for public listing and public detail reads
- Translating public read failures into HTTP responses
- Executing public-read queries with raw SQL and Npgsql
