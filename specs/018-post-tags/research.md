# Phase 0 Research: Add Tags Support to Blog Posts

The feature spec contained no `[NEEDS CLARIFICATION]` markers. This document
records the design decisions that resolve the open implementation choices
identified while reading the existing codebase.

## R1: Where tag normalization lives

- **Decision**: A dedicated `TagNormalizer` static helper in
  `BlogPlatform.Application/Posts`, called by both `CreateBlogPostHandler` and
  `EditBlogPostHandler`.
- **Rationale**: The spec (Section 4) and the constitution's Lightweight Clean
  Architecture principle require business rules outside controllers and outside
  persistence. A single helper guarantees create and edit apply identical
  rules. The domain `BlogPost` already normalizes title/content, but tag
  normalization is multi-value list logic that the spec explicitly assigns to
  the Application layer.
- **Alternatives considered**: (a) Normalize inside `BlogPost` — rejected
  because the spec assigns the rule to the Application layer and it would mix
  list-collapsing logic into the aggregate. (b) Normalize in the controller —
  rejected; controllers must stay HTTP-only. (c) Normalize in the repository —
  rejected; repositories must stay persistence-only.

## R2: Tag identity and storage shape

- **Decision**: Tags are stored in the existing `tags` table keyed by `title`;
  the `post_tags` join table links posts to tags. No schema change.
- **Rationale**: `database/scripts/006-create-tags.sql` and
  `007-create-post-tags.sql` already define both tables, with `post_tags`
  cascading on post and tag deletion. Reusing them keeps the change minimal and
  matches the existing seed data and the existing search query, which already
  joins `post_tags`/`tags`.
- **Alternatives considered**: A denormalized `text[]` column on `posts` —
  rejected because it would diverge from the existing normalized schema and the
  search feature that already joins the tag tables.

## R3: Matching existing tag rows when persisting

- **Decision**: When persisting a post's tags, resolve each normalized tag to a
  `tags` row using a case-insensitive match (`lower(title) = lower(@title)`),
  and create the row with `INSERT ... ON CONFLICT (title) DO NOTHING` when it
  does not exist.
- **Rationale**: De-duplication within a post is case-insensitive (TN-004).
  Matching existing tag rows case-insensitively prevents the `tags` table from
  accumulating near-duplicate rows like `Rust` and `rust`. `ON CONFLICT` makes
  concurrent inserts of the same new tag idempotent and avoids unique-violation
  errors. New tag rows take `created_by_user_id`/`creation_user_id`/
  `update_user_id` from the post's author id, which is always available on the
  `BlogPost` aggregate.
- **Alternatives considered**: (a) Exact-case match only — rejected; it would
  let case variants pile up. (b) Adding a functional unique index on
  `lower(title)` — rejected as scope creep; the residual risk of a stray case
  variant is acceptable for an interview MVP and is recorded as a trade-off in
  plan.md.

## R4: Atomicity of post + tag writes

- **Decision**: `CreateAsync` and `UpdateAsync` wrap the post write and the tag
  writes in a single `NpgsqlTransaction`.
- **Rationale**: Each method now issues several statements (post write, tag
  upserts, `post_tags` writes). A transaction ensures a post is never persisted
  with a partially written tag set. The repository already owns its connection
  lifecycle, so adding a transaction is contained.
- **Alternatives considered**: Non-transactional sequential writes — rejected;
  a mid-sequence failure would leave inconsistent tag data.

## R5: Reading tags without N+1 queries

- **Decision**: Each post-reading SQL statement gains an aggregated tag column
  produced by a correlated subquery, e.g.
  `COALESCE((SELECT array_agg(t.title ORDER BY pt.creation_date, t.id)
  FROM post_tags pt JOIN tags t ON t.id = pt.tag_id
  WHERE pt.post_id = p.id), ARRAY[]::varchar[])`.
- **Rationale**: Tags load in the same single round-trip as the post, so list
  endpoints do not regress into N+1 queries. Npgsql maps a SQL text/varchar
  array directly to a .NET `string[]`, which `MapPost` exposes as
  `IReadOnlyList<string>`. `COALESCE` guarantees an empty array (never `NULL`)
  for posts with no tags.
- **Alternatives considered**: (a) A second query per post — rejected for N+1
  cost on list endpoints. (b) One batched second query using
  `post_id = ANY(@ids)` then grouping in memory — workable but adds a second
  round-trip and grouping code; the aggregated-column approach is simpler.

## R6: Read order of a post's tags

- **Decision**: Tags are read ordered by `post_tags.creation_date` then
  `tags.id`.
- **Rationale**: `post_tags` has no dedicated ordering column. This ordering is
  stable and approximates author-typed order for newly created tags. The spec
  marks order preservation (TN-007) as a SHOULD, and read-side order is not a
  hard requirement.
- **Alternatives considered**: Adding a `sort_order` column to `post_tags` —
  rejected as a schema change beyond the minimal scope; recorded as a trade-off
  in plan.md.

## R7: Frontend comma-separated input handling

- **Decision**: A small `parseTags` helper in `features/posts/tags.ts` converts
  the existing comma-separated `PostEditorDraft.tags` string into `string[]`
  (split on comma, trim, drop empties, case-insensitive de-dup) before the API
  call. `EditPostPage` prefills the input with `post.tags.join(', ')`.
- **Rationale**: The `PostEditorDraft.tags` string field and the tags input in
  `PostForm.tsx` already exist but are ignored. A focused helper makes the
  input meaningful with minimal UI change. The backend `TagNormalizer` remains
  authoritative, so the frontend helper only needs to be lightweight.
- **Alternatives considered**: Replacing the text input with a tag-chip editor
  component — rejected; it exceeds the "minimal frontend changes" constraint
  and the editor design already specifies a comma-separated input.
