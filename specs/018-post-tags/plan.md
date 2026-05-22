# Implementation Plan: Add Tags Support to Blog Posts

**Branch**: `018-post-tags` | **Date**: 2026-05-22 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/018-post-tags/spec.md`

## Summary

Add tag support to the existing blog post create, edit, and read flows. Posts
gain a string-collection `Tags` value that flows through the `BlogPost` domain
model, the `CreateBlogPost`/`EditBlogPost` use cases, the raw SQL post
repository, the API request/response DTOs, and the React create/edit/read UI.
Tag normalization (trim, drop empties, case-insensitive de-duplication) is owned
by a dedicated Application-layer helper so the rule is enforced once for both
create and edit. The `tags` and `post_tags` tables already exist in the
database schema, so no new tables are required. Existing visibility, ownership,
and authorization behavior is unchanged.

## Technical Context

**Language/Version**: .NET 10 LTS (backend), TypeScript (frontend)

**Primary Dependencies**: ASP.NET Core Web API, Npgsql, xUnit, React, Vite,
TailwindCSS

**Storage**: PostgreSQL — existing `posts`, `tags`, and `post_tags` tables

**Testing**: xUnit unit tests (Application), PostgreSQL-backed repository tests
(Infrastructure), API integration tests, Vitest/RTL frontend tests where
practical

**Target Platform**: Web application with ASP.NET Core backend and browser
frontend

**Project Type**: Full-stack web application

**Performance Goals**: Interview-scale demo; tag reads must not introduce N+1
query patterns into existing post list/detail queries

**Constraints**: Raw SQL with Npgsql only; ProblemDetails-style errors;
business rules outside controllers; explicit DTOs; no Entity Framework, Dapper,
Mediator, or MediatR

**Scale/Scope**: Small interview MVP slice limited to tag support for post
create, edit, and read

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Test-first**: PASS — each layer below names the failing tests written first
  (`TagNormalizer` unit tests, handler tag tests, repository persistence tests,
  API integration tests) before production code.
- **Backend-first**: PASS — domain, Application, persistence, and API contract
  changes are sequenced ahead of all frontend work.
- **Architecture**: PASS — normalization lives in an Application helper, the
  repository stays persistence-only, and the domain holds tags without taking
  on the normalization rule.
- **Data access**: PASS — all tag persistence is parameterized raw SQL with
  Npgsql inside `PostgreSqlPostRepository`; Application/Domain never reference
  Npgsql types or table layout.
- **Security**: PASS — no authentication, authorization, or ownership change;
  the edit-flow ownership rule is re-asserted by a regression test that edits
  tags on a non-owned post.
- **Scope**: PASS — tag creation is explicitly inside the approved MVP
  (Principle VI); no tag management, search, or analytics is added.
- **API consistency**: PASS — DTOs stay explicit, validation/error responses
  remain ProblemDetails, and OpenAPI updates flow automatically from DTO
  changes.
- **Frontend governance**: PASS — tag input and tag display reuse existing
  `DESIGN.md` tokens and shared primitives; no new UI framework, no Ballast
  Lane branding.

**Result**: All gates pass. No constitution violations — Complexity Tracking
is intentionally empty.

## Project Structure

### Documentation (this feature)

```text
specs/018-post-tags/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── post-tags-api.md
├── checklists/
│   └── requirements.md
└── tasks.md          # created later by /speckit-tasks
```

### Source Code (repository root)

```text
src/backend/
├── BlogPlatform.Domain/Posts/BlogPost.cs                 # add Tags
├── BlogPlatform.Application/
│   ├── Posts/TagNormalizer.cs                            # NEW helper
│   ├── Posts/CreateBlogPostCommand.cs / Handler / Result
│   ├── Posts/EditBlogPostCommand.cs / Handler / Result
│   ├── Posts/PublicPostListItem.cs / OwnedPostListItem.cs
│   ├── Posts/GetPublicPostByIdResult.cs / GetOwnedPostByIdResult.cs
│   └── Abstractions/IPostRepository.cs                    # tags via BlogPost
├── BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs
└── BlogPlatform.Api/Contracts/Posts/*.cs                  # request/response DTOs

src/frontend/blog-web/src/features/posts/
├── post.types.ts            # add tags fields
├── tags.ts                  # NEW parseTags helper
├── PostForm.tsx             # tags input already present; update note text
├── CreatePostPage.tsx       # send parsed tags
├── EditPostPage.tsx         # send parsed tags + prefill from post.tags
├── PostCard.tsx             # display tags
└── PublicPostDetailPage.tsx # display tags

database/
└── scripts/006-create-tags.sql, 007-create-post-tags.sql # already present
```

**Structure Decision**: Reuse the existing layered backend and the existing
`features/posts` frontend module. No new projects, tables, endpoints, or routes
are introduced; only existing files are extended plus two small new helpers
(`TagNormalizer`, `tags.ts`).

## 1. Technical Approach

Thread a `Tags` string collection through every existing post layer rather than
adding a parallel tag subsystem. Reuse the `CreateBlogPostHandler` and
`EditBlogPostHandler` use cases — they gain a normalization call and pass tags
to the domain object. The repository persists tags inside the same unit of work
as the post write (one transaction) and reads tags back in the same query as
the post via an aggregated subquery, so no new endpoints or round-trip patterns
appear. The frontend wires its already-present comma-separated tags input to
the existing create/edit API calls and renders tag chips on read surfaces.

## 2. Layers Affected

| Layer | Change |
|-------|--------|
| Domain | `BlogPost` carries a read-only `Tags` collection |
| Application | New `TagNormalizer`; commands, handlers, results, list items carry tags |
| Infrastructure | `PostgreSqlPostRepository` persists/reads tags in raw SQL within a transaction |
| Database | No schema change — `tags` and `post_tags` already exist |
| API | Request DTOs accept tags; response DTOs return tags; controllers only pass through |
| Frontend | Types, `parseTags` helper, create/edit submit, edit prefill, tag display |
| Tests | Application, Infrastructure, API integration, and frontend coverage |

## 3. Domain Model Strategy

- Add `IReadOnlyList<string> Tags` to `BlogPost`.
- Extend the private constructor and the `Create`, `Rehydrate`, and `Update`
  factory methods to accept tags.
- The domain stores a defensive copy (`null` → empty list) and does **not**
  re-run normalization — normalization is an Application responsibility. The
  domain trusts that callers pass an already-normalized collection.
- A post with no tags exposes an empty collection, never `null`, so all
  downstream layers can treat tags uniformly.
- No change to existing title/summary/content normalization or to
  visibility/availability fields.

## 4. Application Command/Use Case Strategy

- `CreateBlogPostCommand` and `EditBlogPostCommand` each gain an optional
  `IReadOnlyList<string>? Tags` member (`null` is treated as "no tags").
- `CreateBlogPostHandler` and `EditBlogPostHandler` are reused as-is: each
  calls `TagNormalizer.Normalize(command.Tags)` and passes the normalized
  result into `BlogPost.Create(...)` / `existingPost.Update(...)`.
- Edit replaces the full tag set: the normalized collection from the command
  becomes the post's complete tag set, and an empty collection clears all tags.
- `CreateBlogPostResult`, `EditBlogPostResult`, `GetPublicPostByIdResult`,
  `GetOwnedPostByIdResult`, `PublicPostListItem`, and `OwnedPostListItem` each
  expose the post's `Tags` so the API can return them.
- Existing failure paths (authentication, category availability, ownership,
  validation) are unchanged.

## 5. Tag Normalization Strategy

- Add a dedicated `TagNormalizer` static helper in
  `BlogPlatform.Application/Posts`.
- `Normalize(IEnumerable<string>? tags)` returns `IReadOnlyList<string>` and
  applies, in order: trim each value (TN-001); drop empty/whitespace-only
  values (TN-002, TN-006); collapse duplicates using a case-insensitive
  comparison (TN-003, TN-004), keeping the first occurrence's trimmed casing
  (TN-005); preserve submitted order otherwise (TN-007).
- The helper is the single source of truth, shared by create and edit, so the
  rule cannot drift between the two flows and is never duplicated in
  controllers or the repository.
- The frontend has a separate lightweight `parseTags` for input convenience,
  but the backend `TagNormalizer` result is authoritative.

## 6. Database / Schema Impact

- **No schema change required.** `database/scripts/006-create-tags.sql` already
  defines `tags(id, title UNIQUE, created_by_user_id, audit columns)` and
  `007-create-post-tags.sql` already defines `post_tags(post_id, tag_id, PK,
  audit columns)` with `ON DELETE CASCADE` from both `posts` and `tags`.
- Tag identity is the tag `title`. Persisting a post tag means: find an existing
  `tags` row (matched case-insensitively, see research.md) or insert a new one,
  then insert the `(post_id, tag_id)` link.
- New `tags` rows use the post's author user id for `created_by_user_id`,
  `creation_user_id`, and `update_user_id`.
- Deleting a post already cascades `post_tags` rows; tag rows are intentionally
  left in place (tags are shared and tag cleanup is out of scope).
- Seed data (`006-seed-tags.sql`, `007-seed-post-tags.sql`) already exercises
  the tables and needs no change.

## 7. Repository / Raw SQL Strategy

- Keep `IPostRepository` shaped as-is: tags travel inside the `BlogPost`
  aggregate, so no new repository methods or parameters are needed.
- **Write path** (`CreateAsync`, `UpdateAsync`): wrap the work in a single
  `NpgsqlTransaction` so the post row and its tags commit atomically.
  - `CreateAsync`: insert the post, then for each normalized tag upsert into
    `tags` and insert into `post_tags`.
  - `UpdateAsync`: update the post, `DELETE FROM post_tags WHERE post_id = @id`,
    then re-insert the new tag links (replace semantics; empty set clears all).
- **Read path** (`GetByIdAsync`, `GetByIdForAuthorAsync`,
  `GetPublicReadByIdAsync`, `ListByAuthorAsync`, `ListPublicReadAsync`,
  `SearchPublicReadAsync`): add an aggregated tag column to each SELECT via a
  correlated `array_agg` subquery so tags load in the existing single query
  with no N+1 fan-out. `MapPost` reads the tag array into `IReadOnlyList<string>`.
- All SQL stays parameterized; the repository performs no trimming,
  de-duplication, or other business logic — it persists exactly what the
  normalized `BlogPost.Tags` collection contains.

## 8. API Contract Strategy

- **Requests**: `CreatePostRequest` and `UpdatePostRequest` gain an optional
  `IReadOnlyList<string>? Tags`. Omitting the field is equivalent to an empty
  collection.
- **Responses**: `PostMutationResponse`, `PublicPostSummaryResponse`,
  `PublicPostDetailResponse`, `OwnedPostSummaryResponse`, and
  `OwnedPostDetailResponse` gain `IReadOnlyList<string> Tags` (always present,
  empty when the post has no tags).
- `PostsController` and `PublicPostsController` only map tags between DTOs and
  commands/results — no normalization or validation logic moves into
  controllers.
- Swagger/OpenAPI updates automatically from the changed DTO shapes; no manual
  documentation edits are needed.
- Error contracts (`ProblemDetails`) and status codes are unchanged.

## 9. Frontend Integration Strategy

- `post.types.ts`: add `tags: string[]` to `PublicPostSummary`,
  `PublicPostDetail`, `OwnedPostSummary`, `OwnedPostDetail`, and
  `PostMutationResponse`; add `tags: string[]` to `PostMutationRequest`.
- Add a small `parseTags` helper (`features/posts/tags.ts`) that splits the
  comma-separated `PostEditorDraft.tags` string, trims, drops empties, and
  de-duplicates case-insensitively before the API call.
- `CreatePostPage` and `EditPostPage`: include `tags: parseTags(value.tags)` in
  the create/update request bodies.
- `EditPostPage`: prefill the draft `tags` string from the loaded post via
  `post.tags.join(', ')` so existing tags are editable.
- `PostForm.tsx`: the comma-separated tags input already exists — update the
  stale metadata helper note so it no longer claims tags are ignored.
- Display: render tag chips on `PostCard` and `PublicPostDetailPage` using
  existing `DESIGN.md` badge tokens; a post with no tags renders no tag area.
- Changes stay minimal and visually consistent; no editor or page redesign.

## 10. Testing Strategy

Tests are written first per the Test-First principle.

- **Application unit tests**:
  - `TagNormalizer`: trimming, empty/whitespace removal, case-insensitive
    de-duplication, first-occurrence casing retained, order preserved,
    all-empty input → empty list.
  - `CreateBlogPostHandler`: tags are normalized and forwarded to the saved
    post; no tags / null tags → empty.
  - `EditBlogPostHandler`: tags replace the prior set; empty set clears tags;
    ownership failure still blocks the edit (regression).
- **Infrastructure repository tests** (PostgreSQL-backed): create persists
  tags; update replaces tags; update with empty set clears tags; single-post
  and list reads return tags; public-read query returns tags.
- **API integration tests**: create with tags returns tags; create with
  tags omitted returns empty tags; update replaces tags; public list and
  public detail include tags; owned list/detail include tags.
- **Frontend tests** (where practical): `parseTags` unit coverage; tag chips
  render on a post with tags and are absent for a post without tags. Lint,
  build, and full-stack manual verification back the final check.

## 11. Validation Strategy

- Run the backend test suite (Domain/Application/Infrastructure/API) and
  confirm new tag tests pass and existing post tests still pass.
- Run frontend lint, type-check, build, and available unit tests.
- Bring the stack up via Docker Compose and manually verify: create a post with
  tags, edit it to change/clear tags, and confirm tags appear on the public
  list and detail pages.
- Confirm Swagger reflects the new tag fields on request/response schemas.
- Re-confirm an unrelated user cannot edit another user's post tags.

## 12. Documentation Update Strategy

- Keep `specs/018-post-tags/` artifacts (`spec.md`, `plan.md`, `research.md`,
  `data-model.md`, `contracts/`, `quickstart.md`) as the feature record.
- OpenAPI/Swagger documentation updates automatically from the DTO changes.
- Update the `CLAUDE.md` SPECKIT plan reference to point at this plan.
- No README or DESIGN.md change is required; tag chips reuse existing tokens.

## 13. Risks and Trade-offs

- **Tag casing across posts**: `tags.title` has an exact-case UNIQUE
  constraint. Matching existing tags case-insensitively (see research.md)
  avoids near-duplicate rows; the small residual risk of case variants is
  accepted for interview scope rather than adding a functional unique index.
- **Read order of tags**: `post_tags` has no explicit order column. Reads order
  tags by `creation_date` then `tag_id`, which approximates author-typed order
  but does not perfectly preserve it when existing tags are reused. Accepted as
  a trade-off; adding a `sort_order` column would exceed the minimal scope.
- **Transactional writes**: create/update now run multiple statements in a
  transaction. This is a deliberate, contained change to the two write methods
  and is covered by repository tests.
- **Concurrent tag creation**: two posts created simultaneously with the same
  new tag could race on the `tags` unique constraint; an `INSERT ... ON
  CONFLICT` upsert (see research.md) makes the write idempotent and safe.
- **Frontend/back-end normalization duplication**: `parseTags` and
  `TagNormalizer` apply similar rules. The backend remains authoritative; the
  frontend copy is only for input ergonomics and is intentionally lightweight.

## Complexity Tracking

> No constitution violations identified. This section is intentionally empty.
