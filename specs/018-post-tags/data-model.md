# Phase 1 Data Model: Add Tags Support to Blog Posts

## Entities

### BlogPost (Domain aggregate — modified)

Existing aggregate in `BlogPlatform.Domain/Posts/BlogPost.cs`. One field is
added; all current fields and behavior are unchanged.

| Field | Type | Notes |
|-------|------|-------|
| Id | int | Existing |
| AuthorUserId | int | Existing |
| CategoryId | int | Existing |
| Title | string | Existing |
| Summary | string? | Existing |
| Content | string | Existing |
| IsPublic | bool | Existing |
| IsAvailable | bool | Existing |
| **Tags** | **IReadOnlyList\<string\>** | **NEW** — never null; empty when the post has no tags |

- `Create`, `Rehydrate`, and `Update` factory methods accept a tags collection.
- The aggregate stores a defensive copy and treats `null` input as empty.
- The aggregate does **not** normalize tags; it trusts an already-normalized
  collection (normalization is owned by `TagNormalizer`).

### Tag (persistence entity — existing, unchanged)

Existing `tags` table; no schema change. Listed for reference.

| Column | Type | Notes |
|--------|------|-------|
| id | INTEGER identity | Primary key |
| title | VARCHAR(100) | `UNIQUE`, case-sensitive constraint |
| created_by_user_id | INTEGER NOT NULL | FK → `users(id)` |
| creation_date / update_date | TIMESTAMPTZ | Audit |
| creation_user_id / update_user_id | INTEGER NULL | Audit FKs |

There is no `Tag` domain entity — tags are exposed only as string values on
`BlogPost`, per the spec.

### PostTag link (persistence — existing, unchanged)

Existing `post_tags` table; no schema change.

| Column | Type | Notes |
|--------|------|-------|
| post_id | INTEGER | FK → `posts(id)` `ON DELETE CASCADE` |
| tag_id | INTEGER | FK → `tags(id)` `ON DELETE CASCADE` |
| creation_date / update_date | TIMESTAMPTZ | Audit |
| creation_user_id / update_user_id | INTEGER NULL | Audit FKs |
| (post_id, tag_id) | composite | Primary key — prevents duplicate links |

## Relationships

- `BlogPost` 1 — N `post_tags` N — 1 `tags` (many-to-many between posts and
  tags through `post_tags`).
- A tag may be referenced by many posts; a post may reference many tags.
- Deleting a post cascades its `post_tags` rows; `tags` rows persist (shared).

## Validation & Normalization Rules

Applied by `TagNormalizer` (Application layer) before a `BlogPost` is built:

| Rule | Source | Behavior |
|------|--------|----------|
| Trim | TN-001 | Leading/trailing whitespace removed from each tag |
| Drop empties | TN-002, TN-006 | Empty/whitespace-only tags excluded; all-empty input → empty list |
| De-duplicate | TN-003, TN-004 | Duplicates removed using case-insensitive comparison |
| Casing kept | TN-005 | First occurrence's trimmed value retained |
| Order | TN-007 | Submitted order preserved for surviving tags |

Persistence-level constraints: `tags.title` is `UNIQUE`; the `post_tags`
composite primary key prevents a tag being linked to the same post twice.

## State / Flow Notes

- **Create**: normalized tags → `BlogPost.Create` → repository inserts post,
  upserts each tag, inserts `post_tags` links (single transaction).
- **Edit**: normalized tags fully replace the prior set → repository updates
  post, deletes all `post_tags` for the post, re-inserts the new links (single
  transaction). An empty set clears all tags.
- **Read**: repository selects the post and an aggregated tag array in one
  query; an empty array is returned for posts with no tags.

## Application & API Carriers (modified to carry Tags)

| Type | Layer | Change |
|------|-------|--------|
| CreateBlogPostCommand / EditBlogPostCommand | Application | `+ IReadOnlyList<string>? Tags` |
| CreateBlogPostResult / EditBlogPostResult | Application | `+ Tags` |
| GetPublicPostByIdResult / GetOwnedPostByIdResult | Application | `+ Tags` |
| PublicPostListItem / OwnedPostListItem | Application | `+ Tags` |
| CreatePostRequest / UpdatePostRequest | API | `+ IReadOnlyList<string>? Tags` (optional) |
| PostMutationResponse | API | `+ IReadOnlyList<string> Tags` |
| PublicPostSummaryResponse / PublicPostDetailResponse | API | `+ IReadOnlyList<string> Tags` |
| OwnedPostSummaryResponse / OwnedPostDetailResponse | API | `+ IReadOnlyList<string> Tags` |
