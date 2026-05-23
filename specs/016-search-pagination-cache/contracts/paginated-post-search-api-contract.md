# Paginated Post Search API Contract

## Route Contract

- Reuse `GET /api/posts`
- Supported query parameters:
  - `q` optional search term
  - `page` optional page number
  - `pageSize` optional page size

## Request Behavior

- Anonymous requests remain allowed.
- Authenticated requests may include a bearer token.
- Authenticated default listing remains public-only when no search term is
  provided.
- Authenticated search requests can receive owned private or non-public matches
  when existing visibility rules allow them.
- Blank `q` behaves like the default paginated list.
- Invalid pagination inputs use the existing ProblemDetails-style validation
  behavior.

## Response Contract

The response becomes a paginated envelope rather than a bare array:

```json
{
  "items": [
    {
      "id": 1,
      "title": "Post title",
      "summary": "Optional summary",
      "publishDate": "2026-05-22T10:00:00Z",
      "expirationDate": null
    }
  ],
  "page": 1,
  "pageSize": 6,
  "totalCount": 12,
  "totalPages": 2,
  "hasNextPage": true
}
```

## Visibility Contract

- Anonymous users receive only public and available posts.
- Authenticated users receive the same public and available posts as anonymous
  users in the default listing.
- Authenticated users may also receive owned private or non-public matches in
  search results when allowed by the current business rules.
- Private posts owned by other users must never appear, whether the result is
  served live or from cache.

## Cache Contract

- Cached entries apply only to read/list/search operations.
- TTL is 30 seconds.
- Cache keys must include:
  - normalized search term or explicit empty-search marker
  - page
  - page size
  - viewer context

## Frontend Consumption Contract

- The frontend keeps using the existing search input and route query string.
- The frontend requests the first page on initial load or search reset.
- The frontend requests additional pages and appends items while `hasNextPage`
  remains true.
- The frontend shows an end-of-list state when `hasNextPage` becomes false.
