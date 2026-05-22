# Post Search API Contract

## Route Contract

- Reuse `GET /api/posts`
- Add optional query parameter:
  - `q`: search term

## Request Behavior

- Anonymous request:
  - no bearer token required
  - returns only matching public and available posts
- Authenticated request:
  - bearer token may be supplied
  - returns matching public and available posts plus matching owned private
    available posts when allowed by the domain rules
- Blank `q`:
  - behaves the same as the current default listing

## Response Contract

Response shape remains the existing public list DTO:

```json
[
  {
    "id": 1,
    "title": "Post title",
    "summary": "Optional summary"
  }
]
```

## Visibility Contract

- Public and available posts may appear for any requester.
- Owned non-public matches may appear only for the authenticated owner.
- Private or non-public posts owned by someone else must never appear.
- Unavailable posts remain excluded unless an already-existing owner rule
  explicitly includes them.

## Frontend Consumption Contract

- The frontend continues to control search state through `?q=...`.
- The public posts page must request backend results whenever `q` changes.
- The frontend must stop filtering the full post list locally.
- If a session token exists, the request should include it so the backend can
  apply owner-inclusive search rules.

## Failure Contract

- Blank `q` is not an error.
- Malformed requests continue to use ProblemDetails-style API failures if any
  validation is triggered.
- Search does not introduce new authentication requirements for public browsing.
