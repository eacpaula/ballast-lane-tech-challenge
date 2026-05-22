# API Contracts: Persist Post Publish and Expiration Dates

**Feature**: 019-persist-post-dates
**Date**: 2026-05-22

All date/time fields are ISO 8601 UTC strings (e.g. `"2026-06-01T09:00:00Z"`).
Null values are represented as JSON `null`. Omitting a field is treated as null.

---

## POST /api/posts — Create Post

### Request body (additions)

```json
{
  "categoryId": 1,
  "title": "My Post",
  "summary": "Optional summary",
  "content": "Post body",
  "tags": ["tag1"],
  "publishDate": "2026-06-01T09:00:00Z",
  "expirationDate": "2026-12-31T23:59:59Z"
}
```

`publishDate` and `expirationDate` are optional. Omit or pass `null` for no
scheduling constraint.

### Validation error (400)

Returned when `expirationDate <= publishDate` (both provided):

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Validation Error",
  "status": 400,
  "detail": "Expiration date must be after publish date.",
  "errors": {
    "expirationDate": ["Expiration date must be after publish date."]
  }
}
```

### Response body (201 Created — additions)

```json
{
  "id": 42,
  "authorUserId": 7,
  "title": "My Post",
  "summary": "Optional summary",
  "content": "Post body",
  "tags": ["tag1"],
  "publishDate": "2026-06-01T09:00:00Z",
  "expirationDate": "2026-12-31T23:59:59Z"
}
```

`publishDate` and `expirationDate` reflect the persisted values (null if not
provided).

---

## PUT /api/posts/{postId} — Update Post

### Request body (additions)

```json
{
  "title": "Updated Title",
  "summary": "Updated summary",
  "content": "Updated body",
  "tags": [],
  "publishDate": null,
  "expirationDate": "2027-01-01T00:00:00Z"
}
```

Passing `null` explicitly clears the stored date.

### Validation error (400)

Same ProblemDetails shape as create when `expirationDate <= publishDate`.

### Response body (200 OK — additions)

Same shape as create response with updated values.

---

## GET /api/posts — Public Post List

### Query parameters

No change. Date filtering is server-side only.

### Response body (200 OK — additions)

```json
[
  {
    "id": 42,
    "title": "My Post",
    "summary": "Optional summary",
    "tags": ["tag1"],
    "publishDate": "2026-06-01T09:00:00Z",
    "expirationDate": null
  }
]
```

Posts outside the active publish/expiration window are **excluded** from the
response for anonymous callers. Authenticated callers receive their own posts
regardless of window (for search; see below).

---

## GET /api/posts/{postId} — Public Post Detail

### Response body (200 OK — additions)

```json
{
  "id": 42,
  "title": "My Post",
  "summary": "Optional summary",
  "content": "Post body",
  "tags": ["tag1"],
  "publishDate": "2026-06-01T09:00:00Z",
  "expirationDate": null
}
```

Returns **404** if the post is outside the active window (anonymous caller).

---

## GET /api/posts?q={query} — Public Post Search

### Query parameters

No change. Date filtering is server-side only.

### Response body (200 OK)

Same shape as public post list. Visibility window applied. Authenticated callers
receive their own posts regardless of window state.

---

## GET /api/posts/mine — Owned Post List

### Response body (200 OK — additions)

```json
[
  {
    "id": 42,
    "categoryId": 1,
    "title": "My Post",
    "summary": "Optional summary",
    "isPublic": true,
    "isAvailable": true,
    "tags": ["tag1"],
    "publishDate": "2026-06-01T09:00:00Z",
    "expirationDate": null
  }
]
```

No visibility window applied. Authenticated owner sees all their own posts.

---

## GET /api/posts/mine/{postId} — Owned Post Detail

### Response body (200 OK — additions)

```json
{
  "id": 42,
  "authorUserId": 7,
  "categoryId": 1,
  "title": "My Post",
  "summary": "Optional summary",
  "content": "Post body",
  "isPublic": true,
  "isAvailable": true,
  "tags": ["tag1"],
  "publishDate": "2026-06-01T09:00:00Z",
  "expirationDate": null
}
```

No visibility window applied.

---

## Summary of DTO changes

| DTO class | New fields |
|-----------|-----------|
| `CreatePostRequest` | `PublishDate?`, `ExpirationDate?` |
| `UpdatePostRequest` | `PublishDate?`, `ExpirationDate?` |
| `PostMutationResponse` | `PublishDate?`, `ExpirationDate?` |
| `PublicPostDetailResponse` | `PublishDate?`, `ExpirationDate?` |
| `PublicPostSummaryResponse` | `PublishDate?`, `ExpirationDate?` |
| `OwnedPostDetailResponse` | `PublishDate?`, `ExpirationDate?` |
| `OwnedPostSummaryResponse` | `PublishDate?`, `ExpirationDate?` |
