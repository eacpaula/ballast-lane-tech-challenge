# Data Model: Persist Post Publish and Expiration Dates

**Feature**: 019-persist-post-dates
**Date**: 2026-05-22

---

## Affected Entity: BlogPost

### Current shape (Domain)

| Property | Type | Nullable |
|----------|------|----------|
| Id | int | no |
| AuthorUserId | int | no |
| CategoryId | int | no |
| Title | string | no |
| Summary | string | yes |
| Content | string | no |
| IsPublic | bool | no |
| IsAvailable | bool | no |
| Tags | IReadOnlyList\<string\> | no (empty list) |
| IsPubliclyReadable | bool (computed) | — |

### New shape (Domain)

All existing properties are preserved. The following are added:

| Property | Type | Nullable | Notes |
|----------|------|----------|-------|
| PublishDate | DateTimeOffset | yes | Null = no scheduled start |
| ExpirationDate | DateTimeOffset | yes | Null = no scheduled end |

`IsPubliclyReadable` is updated to evaluate:
```
IsPublic
  AND IsAvailable
  AND (PublishDate is null OR PublishDate <= DateTimeOffset.UtcNow)
  AND (ExpirationDate is null OR ExpirationDate > DateTimeOffset.UtcNow)
```

### Factory method signatures (updated)

**`BlogPost.Create()`** — gains two optional parameters:
```
publishDate:    DateTimeOffset? = null
expirationDate: DateTimeOffset? = null
```

**`BlogPost.Rehydrate()`** — gains two optional parameters:
```
publishDate:    DateTimeOffset? = null
expirationDate: DateTimeOffset? = null
```

**`BlogPost.Update()`** — gains two optional parameters:
```
publishDate:    DateTimeOffset? = null
expirationDate: DateTimeOffset? = null
```

No new validation lives in the domain; date consistency
(`expirationDate > publishDate`) is an Application-layer concern.

---

## Database: posts table

**File**: `database/scripts/005-create-posts.sql`

Both columns ALREADY EXIST. No DDL change required:

| Column | Type | Nullable |
|--------|------|----------|
| publish_date | TIMESTAMPTZ | yes |
| expire_date | TIMESTAMPTZ | yes |

**What changes in the repository**:
- INSERT: replace `CASE WHEN @public_post THEN NOW() ELSE NULL END` with
  `@publish_date` parameter; add `expire_date = @expire_date` parameter.
- UPDATE: add `publish_date = @publish_date` and `expire_date = @expire_date`
  to SET clause.
- All SELECT statements: add `p.publish_date` and `p.expire_date` to column
  list; update `RETURNING` clauses to include both columns.
- `MapPost` / `MapPostFromReader`: read `publish_date` and `expire_date` from
  reader and pass to `Rehydrate()`.

---

## Application Commands (updated fields)

### CreateBlogPostCommand

| Field | Type | Notes |
|-------|------|-------|
| PublishDate | DateTimeOffset? | Optional; null = no scheduled start |
| ExpirationDate | DateTimeOffset? | Optional; null = no scheduled end |

### EditBlogPostCommand

| Field | Type | Notes |
|-------|------|-------|
| PublishDate | DateTimeOffset? | Optional; null = clear publish date |
| ExpirationDate | DateTimeOffset? | Optional; null = clear expiration date |

### Date consistency validation (Application layer)

Applied in both `CreateBlogPostHandler` and `EditBlogPostHandler`:
```
if (PublishDate has value AND ExpirationDate has value)
    ExpirationDate MUST be > PublishDate
    otherwise → validation error (400)
```

---

## Application Results (updated fields)

All result/output types gain:

| Field | Type |
|-------|------|
| PublishDate | DateTimeOffset? |
| ExpirationDate | DateTimeOffset? |

Affected types:
- `CreateBlogPostResult`
- `EditBlogPostResult`
- `GetPublicPostByIdResult`
- `GetOwnedPostByIdResult`
- `PublicPostListItem`
- `OwnedPostListItem`

---

## Seed Data (if applicable)

Representative seed rows for `posts` table:

| Row | publish_date | expire_date | Purpose |
|-----|-------------|-------------|---------|
| A | NULL | NULL | Always-visible post (no window) |
| B | Past date (e.g. -7 days) | NULL | Active with explicit publish date |
| C | Future date (e.g. +7 days) | NULL | Scheduled — not yet visible |
| D | Past date | Future date | Active within window |
| E | Past date | Past date (-1 day) | Expired — no longer visible |
