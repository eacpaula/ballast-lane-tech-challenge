# API Contract: Post Tags

This feature adds a `tags` field to existing post endpoints. No new endpoints,
routes, status codes, or error contracts are introduced. Authentication,
authorization, and ownership behavior is unchanged.

## Conventions

- `tags` in **requests** is an optional array of strings. Omitting it or
  sending `null` is equivalent to `[]`.
- `tags` in **responses** is always present and is an array of strings; it is
  `[]` when the post has no tags.
- Submitted tags are normalized server-side (trim, drop empties,
  case-insensitive de-duplication) before persistence; responses reflect the
  normalized set.

## POST /api/posts — create a post

**Auth**: required (bearer).

Request body:

```json
{
  "categoryId": 3,
  "title": "Architecting Scalable Microservices",
  "summary": "A short overview.",
  "content": "Full markdown content...",
  "tags": ["rust", "  cloud-native ", "rust", ""]
}
```

`201 Created` response body:

```json
{
  "id": 42,
  "authorUserId": 7,
  "title": "Architecting Scalable Microservices",
  "summary": "A short overview.",
  "content": "Full markdown content...",
  "tags": ["rust", "cloud-native"]
}
```

- The example shows normalization: whitespace trimmed, empty value dropped,
  case-insensitive duplicate collapsed.
- Failure responses (`400`, `401`, `404`) are unchanged `ProblemDetails`.

## PUT /api/posts/{postId} — edit an owned post

**Auth**: required (bearer); caller must own the post.

Request body:

```json
{
  "title": "Updated Title",
  "summary": "Updated summary.",
  "content": "Updated content...",
  "tags": ["architecture", "testing"]
}
```

`200 OK` response body:

```json
{
  "id": 42,
  "authorUserId": 7,
  "title": "Updated Title",
  "summary": "Updated summary.",
  "content": "Updated content...",
  "tags": ["architecture", "testing"]
}
```

- The submitted `tags` array fully **replaces** the post's existing tags.
- Sending `"tags": []` (or omitting `tags`) clears all tags on the post.
- Ownership failure still returns `403` `ProblemDetails`; not-found returns
  `404`; validation failure returns `400`.

## GET /api/posts — public post list

`200 OK` response body (array); each item gains `tags`:

```json
[
  { "id": 42, "title": "Architecting Scalable Microservices", "summary": "A short overview.", "tags": ["rust", "cloud-native"] },
  { "id": 43, "title": "A Post With No Tags", "summary": null, "tags": [] }
]
```

## GET /api/posts/{postId} — public post detail

`200 OK` response body:

```json
{
  "id": 42,
  "title": "Architecting Scalable Microservices",
  "summary": "A short overview.",
  "content": "Full markdown content...",
  "tags": ["rust", "cloud-native"]
}
```

## GET /api/posts/mine and /api/posts/mine/{postId} — owned posts

Both owned-post responses (`OwnedPostSummaryResponse`,
`OwnedPostDetailResponse`) gain the same `tags` array, populated from the
post's persisted tags.

## OpenAPI / Swagger

Swagger documentation regenerates automatically from the updated request and
response DTOs; no manual specification edits are required.

## Contract Test Checklist

- Create with a mixed `tags` array → response `tags` is trimmed, de-duplicated,
  empties removed.
- Create with `tags` omitted → response `tags` is `[]`.
- Edit with a new `tags` array → response `tags` equals the new set.
- Edit with `tags: []` → response `tags` is `[]` and the post has no tags.
- Public list and public detail include `tags` for tagged and untagged posts.
- Owned list and owned detail include `tags`.
- Editing a non-owned post still returns `403` regardless of the `tags` field.
