---
description: "Task list for Add Tags Support to Blog Posts"
---

# Tasks: Add Tags Support to Blog Posts

**Input**: Design documents from `/specs/018-post-tags/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md,
contracts/post-tags-api.md

**Tests**: Backend tests are mandatory. Write failing tests before production
code for every backend layer (Application, Infrastructure, API). Frontend tests
cover `parseTags` and tag display where practical; final frontend validation
is done through lint, build, and Docker Compose.

**Organization**: Tasks are grouped by story to enable independent
implementation. US1 = create tags, US2 = edit tags, US3 = read tags,
US4 = frontend integration.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no shared dependencies)
- **[Story]**: Which user story this task belongs to (US1–US4)
- Exact file paths are given in every task description

## Path Conventions

- **Backend src**: `src/backend/BlogPlatform.{Domain,Application,Infrastructure,Api}/`
- **Backend tests**: `tests/backend/BlogPlatform.{Application,Infrastructure,Api}.Tests/`
- **Frontend**: `src/frontend/blog-web/src/`

---

## Phase 1: Setup — Verify Foundation

**Purpose**: Confirm the existing schema and running toolchain match the plan
before writing any production code. No source files change in this phase.

- [ ] T001 Confirm `database/scripts/006-create-tags.sql` and
      `007-create-post-tags.sql` define `tags(id, title UNIQUE,
      created_by_user_id NOT NULL)` and `post_tags(post_id, tag_id, PK,
      ON DELETE CASCADE)` — document any gap; no source change expected.
- [ ] T002 Run `dotnet test` from repo root and confirm the full existing
      backend suite passes before any changes are made.
- [ ] T003 [P] Run `cd src/frontend/blog-web && npm run lint && npm run build`
      and confirm the frontend builds cleanly before any changes.

**Checkpoint**: Existing tests pass; schema is confirmed; no change needed to
database scripts.

---

## Phase 2: Foundational — Tag Normalizer (Shared)

**Purpose**: Implement `TagNormalizer` once before any handler or domain work
so create and edit both consume the same rule.

**CRITICAL**: This phase MUST complete before any US1/US2 work begins.

### Failing Tests First ⚠️

- [ ] T004 Create
      `tests/backend/BlogPlatform.Application.Tests/Posts/TagNormalizerTests.cs`
      with failing tests for: trim leading/trailing whitespace (TN-001); drop
      empty or whitespace-only values (TN-002, TN-006); case-insensitive
      de-duplication — first occurrence retained (TN-003–TN-005); order
      preserved for surviving tags (TN-007); null input → empty list; all-empty
      input → empty list. Verify all tests fail.

### Implementation

- [ ] T005 Create
      `src/backend/BlogPlatform.Application/Posts/TagNormalizer.cs` —
      a `static` internal helper with
      `Normalize(IEnumerable<string>? tags) : IReadOnlyList<string>` that
      applies the rules in the order documented by spec Section 4. Run T004
      tests and confirm they all pass.

**Checkpoint**: `TagNormalizer` is tested and passing; no other code changed.

---

## Phase 3: US1 — Tags on Create Post

**Goal**: A user who creates a post can include tags; the normalized tags are
persisted with the post and returned in the create response.

**Independent Test**: POST `/api/posts` with `"tags": [" rust ", "cloud-native",
"rust", ""]` returns `201` with `"tags": ["rust", "cloud-native"]`; the post
is readable with those tags.

### Failing Tests First ⚠️

- [ ] T006 [P] [US1] Add tests to
      `tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerTagTests.cs`
      (new file) for: handler forwards normalized tags to the repository;
      null tags → empty tag list on result; tags are present on
      `CreateBlogPostResult`; ownership/category rules still block when
      violated. Verify tests fail.
- [ ] T007 [P] [US1] Add tag-persistence tests to
      `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryCreateTagTests.cs`
      (new file): creating a post with tags writes the expected rows to
      `post_tags` and reads them back; creating with no tags leaves `post_tags`
      empty for that post; tags with the same `lower(title)` reuse the existing
      `tags` row. Verify tests fail.
- [ ] T008 [P] [US1] Add test to
      `tests/backend/BlogPlatform.Api.Tests/Posts/CreatePostApiTests.cs`
      (existing): POST with `tags` array returns `201` with `tags` in the body;
      POST without `tags` returns `tags: []`. Verify new assertions fail.

### Backend Implementation

- [ ] T009 [US1] Add `IReadOnlyList<string> Tags { get; }` to
      `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`; extend the private
      constructor, `Create`, `Rehydrate`, and `Update` factory methods to
      accept and store a tags collection (defensive copy; `null` → empty list).
- [ ] T010 [US1] Add `IReadOnlyList<string>? Tags` to
      `src/backend/BlogPlatform.Application/Posts/CreateBlogPostCommand.cs`
      and add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Application/Posts/CreateBlogPostResult.cs`.
      Update `CreateBlogPostResult.Success(BlogPost)` to map `post.Tags`.
- [ ] T011 [US1] Update
      `src/backend/BlogPlatform.Application/Posts/CreateBlogPostHandler.cs` to
      call `TagNormalizer.Normalize(command.Tags)` and pass the result to
      `BlogPost.Create(...)`.
- [ ] T012 [US1] Update the `CreateAsync` method in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
      to open a `NpgsqlTransaction`; after inserting the post, for each
      normalized tag: look up the `tags` row by `lower(title) = lower(@title)`;
      if missing, `INSERT INTO tags ... ON CONFLICT (title) DO NOTHING` then
      re-select; then `INSERT INTO post_tags`. Commit transaction. All SQL must
      be parameterized.
- [ ] T013 [US1] Add `IReadOnlyList<string>? Tags` to
      `src/backend/BlogPlatform.Api/Contracts/Posts/CreatePostRequest.cs`
      and add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Api/Contracts/Posts/PostMutationResponse.cs`.
- [ ] T014 [US1] Update the `Create` action in
      `src/backend/BlogPlatform.Api/Controllers/PostsController.cs` to pass
      `request.Tags ?? []` to `CreateBlogPostCommand` and map `result.Tags` to
      `PostMutationResponse`. No normalization logic in the controller.
- [ ] T015 [US1] Run `dotnet test` and confirm T006–T008 tests pass and all
      pre-existing tests still pass.

**Checkpoint**: POST `/api/posts` with tags works end-to-end in tests.

---

## Phase 4: US2 — Tags on Edit Post

**Goal**: A user who edits an owned post can replace its tags; an empty tag set
clears all tags; existing ownership rules still block non-owners.

**Independent Test**: PUT `/api/posts/{id}` with new tags returns `200` with
the updated tag set; a second PUT with `"tags": []` returns `200` with
`"tags": []`; a PUT from a non-owner still returns `403`.

### Failing Tests First ⚠️

- [ ] T016 [P] [US2] Add tests to
      `tests/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerTagTests.cs`
      (new file): handler replaces the post's prior tags with the normalized
      set from the command; empty tags clears all tags; null tags → empty;
      ownership failure returns error regardless of tags. Verify tests fail.
- [ ] T017 [P] [US2] Add tag-update tests to
      `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryUpdateTagTests.cs`
      (new file): update replaces old tags with new ones; update with empty
      tags removes all `post_tags` rows for the post; transaction rolls back if
      the post does not exist. Verify tests fail.
- [ ] T018 [P] [US2] Add test to
      `tests/backend/BlogPlatform.Api.Tests/Posts/UpdatePostApiTests.cs`
      (existing): PUT with new tags returns updated tags; PUT with `"tags": []`
      returns empty tags; PUT from non-owner returns `403`. Verify new
      assertions fail.

### Backend Implementation

- [ ] T019 [US2] Add `IReadOnlyList<string>? Tags` to
      `src/backend/BlogPlatform.Application/Posts/EditBlogPostCommand.cs`
      and add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Application/Posts/EditBlogPostResult.cs`.
      Update `EditBlogPostResult.Success(BlogPost)` to map `post.Tags`.
- [ ] T020 [US2] Update
      `src/backend/BlogPlatform.Application/Posts/EditBlogPostHandler.cs` to
      call `TagNormalizer.Normalize(command.Tags)` and pass the result to
      `existingPost.Update(...)`.
- [ ] T021 [US2] Update the `UpdateAsync` method in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
      to open a `NpgsqlTransaction`; after updating the post, execute
      `DELETE FROM post_tags WHERE post_id = @id`; then for each tag in the
      normalized set upsert into `tags` and insert into `post_tags` (same
      pattern as T012). Commit transaction. An empty tag set results in no
      `post_tags` inserts. All SQL must be parameterized.
- [ ] T022 [US2] Add `IReadOnlyList<string>? Tags` to
      `src/backend/BlogPlatform.Api/Contracts/Posts/UpdatePostRequest.cs`.
      Update the `Update` action in
      `src/backend/BlogPlatform.Api/Controllers/PostsController.cs` to pass
      `request.Tags ?? []` to `EditBlogPostCommand`. No normalization logic in
      the controller.
- [ ] T023 [US2] Run `dotnet test` and confirm T016–T018 tests pass and all
      prior tests still pass.

**Checkpoint**: PUT `/api/posts/{id}` with tags works end-to-end in tests.

---

## Phase 5: US3 — Tags on Read Flows

**Goal**: All post-reading endpoints include the post's tags in their response;
a post with no tags returns `"tags": []`.

**Independent Test**: GET `/api/posts` and GET `/api/posts/{id}` both return
`tags` in every item. Owned-post endpoints likewise. A post created without
tags shows `"tags": []`.

### Failing Tests First ⚠️

- [ ] T024 [P] [US3] Add read-with-tags tests to
      `tests/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryReadTagTests.cs`
      (new file): `GetByIdAsync` returns tags on the post; `GetPublicReadByIdAsync`
      returns tags; `ListPublicReadAsync` returns tags for each post;
      `ListByAuthorAsync` returns tags; a post with no tags returns an empty
      list (not null). Verify tests fail.
- [ ] T025 [P] [US3] Add tests to
      `tests/backend/BlogPlatform.Api.Tests/Posts/PublicPostDetailTests.cs` and
      `PublicPostListTests.cs` (existing): response includes a `tags` array; a
      post with no tags has `"tags": []`. Verify new assertions fail.
- [ ] T026 [P] [US3] Add tests to
      `tests/backend/BlogPlatform.Api.Tests/Posts/OwnedPostDetailApiTests.cs`
      and `OwnedPostListApiTests.cs` (existing): response includes `tags`.
      Verify new assertions fail.

### Backend Implementation

- [ ] T027 [US3] Update `MapPost` in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
      to read an aggregated `tags` column (added via a correlated `array_agg`
      subquery in each SELECT; see research.md R5). Update `BlogPost.Rehydrate`
      calls to pass the tag array. Update `BuildPostCommand` if needed. Apply
      to all read methods: `GetByIdAsync`, `GetByIdForAuthorAsync`,
      `GetPublicReadByIdAsync`, `ListByAuthorAsync`, `ListPublicReadAsync`, and
      `SearchPublicReadAsync`. All SQL stays parameterized.
- [ ] T028 [US3] Add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Application/Posts/PublicPostListItem.cs` and
      update `PublicPostListItem.From(BlogPost)`.
- [ ] T029 [P] [US3] Add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Application/Posts/OwnedPostListItem.cs` and
      update `OwnedPostListItem.From(BlogPost)`.
- [ ] T030 [P] [US3] Add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdResult.cs`
      and update `GetPublicPostByIdResult.Success(BlogPost)`.
- [ ] T031 [P] [US3] Add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Application/Posts/GetOwnedPostByIdResult.cs`
      and update `GetOwnedPostByIdResult.Success(BlogPost)`.
- [ ] T032 [US3] Add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Api/Contracts/Posts/PublicPostSummaryResponse.cs`
      and `PublicPostDetailResponse.cs`. Update `PublicPostsController.cs` to
      map `post.Tags` / `result.Tags` when building responses.
- [ ] T033 [US3] Add `IReadOnlyList<string> Tags` to
      `src/backend/BlogPlatform.Api/Contracts/Posts/OwnedPostSummaryResponse.cs`
      and `OwnedPostDetailResponse.cs`. Update `PostsController.cs` list and
      get-by-id actions to map tags.
- [ ] T034 [US3] Run `dotnet test` and confirm T024–T026 tests pass and all
      prior tests still pass.

**Checkpoint**: All read endpoints return tags; full backend test suite green.

---

## Phase 6: US4 — Frontend Integration

**Goal**: Authors can enter comma-separated tags when creating or editing a
post; the saved tags display on post cards and the public post detail page.

**Independent Test**: Create a post with `"rust, cloud-native, rust, "` in the
tags field; confirm the public list shows `rust` and `cloud-native` as chips;
edit the post to clear tags and confirm chips disappear.

**Prerequisite**: Phases 3, 4, and 5 are complete and the backend is running
via Docker Compose.

### Frontend Type and Helper Setup

- [ ] T035 [US4] Add `tags: string[]` to `PublicPostSummary`, `PublicPostDetail`,
      `OwnedPostSummary`, `OwnedPostDetail`, `PostMutationResponse`, and
      `PostMutationRequest` in
      `src/frontend/blog-web/src/features/posts/post.types.ts`.
- [ ] T036 [US4] Create
      `src/frontend/blog-web/src/features/posts/tags.ts` with
      `parseTags(raw: string): string[]` — split on comma, trim each value,
      drop empties, de-duplicate case-insensitively (preserving first-seen
      casing). Add a corresponding Vitest unit test file if the project already
      has a frontend test setup; otherwise test manually via the running stack.

### Create and Edit Flows

- [ ] T037 [US4] Update
      `src/frontend/blog-web/src/features/posts/owned-posts.api.ts` so
      `createPost` passes `tags: string[]` in the request body (already typed
      through `PostMutationRequest`). No additional change needed if the type
      propagates correctly.
- [ ] T038 [US4] Update
      `src/frontend/blog-web/src/features/posts/CreatePostPage.tsx` to include
      `tags: parseTags(value.tags)` in the `createPost` call.
- [ ] T039 [US4] Update
      `src/frontend/blog-web/src/features/posts/EditPostPage.tsx` to:
      (a) prefill the `PostEditorDraft.tags` string with
      `post.tags.join(', ')` when the post loads; (b) include
      `tags: parseTags(value.tags)` in the `updatePost` call.
- [ ] T040 [US4] Update the stale metadata note in
      `src/frontend/blog-web/src/features/posts/PostForm.tsx` — replace the
      message that says tags are ignored with one that accurately describes what
      tags fields do and don't do (e.g., publish/expire dates are still
      pending; tags now work).

### Tag Display

- [ ] T041 [US4] Add a tag chip list to
      `src/frontend/blog-web/src/features/posts/PostCard.tsx` — render each
      tag as a small badge using the approved `DESIGN.md` badge token
      (e.g., `badge` or `tag` component class); render nothing if `tags` is
      empty.
- [ ] T042 [US4] Add a tag chip list to
      `src/frontend/blog-web/src/features/posts/PublicPostDetailPage.tsx` —
      same token as T041; render nothing if tags are empty.
- [ ] T043 [US4] Run `cd src/frontend/blog-web && npm run lint && npm run build`
      and confirm no type errors or warnings.

**Checkpoint**: Frontend builds cleanly and tags are wired to the API.

---

## Final Phase: Polish & Full-Stack Validation

**Purpose**: End-to-end validation through the running stack and minor
documentation tidying.

- [ ] T044 Bring the full stack up with `docker compose up` and manually
      verify: create a post with mixed/duplicate/whitespace tags → only
      normalized tags appear; edit the post to change tags → new tags replace
      old ones; clear the tags field → post shows no tags.
- [ ] T045 [P] Open Swagger at the API's `/swagger` URL and confirm `tags`
      appears in `CreatePostRequest`, `UpdatePostRequest`,
      `PostMutationResponse`, `PublicPostSummaryResponse`,
      `PublicPostDetailResponse`, `OwnedPostSummaryResponse`, and
      `OwnedPostDetailResponse`.
- [ ] T046 [P] Open the public post listing and post detail in the browser;
      confirm tag chips display for tagged posts and no empty space appears for
      untagged posts; check browser console for errors or warnings.
- [ ] T047 [P] Verify that a different user's post cannot have its tags edited
      by making a PUT request as another user and confirming `403 Forbidden`.
- [ ] T048 Run `dotnet test` one final time to confirm the complete backend
      suite is green.
- [ ] T049 [P] Review `specs/018-post-tags/quickstart.md` for accuracy; update
      any validation steps that no longer match the implementation. No README or
      API documentation changes are required (Swagger is auto-generated).
- [ ] T050 Confirm no out-of-scope work was introduced: no tag management UI,
      no tag search, no caching change, no visibility/availability/publish-date
      change, no Entity Framework/Dapper/MediatR.

---

## Dependencies & Execution Order

### Phase Dependencies

```text
Phase 1 (Setup)
  └── Phase 2 (TagNormalizer) — blocks all story work
        ├── Phase 3 (US1 Create Tags)
        │     └── Phase 4 (US2 Edit Tags)  ← reuses BlogPost.Tags from Phase 3
        ├── Phase 5 (US3 Read Tags)  ← can start after Phase 3 domain/infra work
        └── Phase 6 (US4 Frontend)  ← depends on Phases 3, 4, and 5
              └── Final Phase (Polish)
```

### Within Each Phase

1. Failing tests are written first and verified to fail.
2. Implementation follows until tests pass.
3. Full suite is re-run before the checkpoint is declared.
4. Frontend tasks wait for the stable backend contract.

### Parallel Opportunities

- T006, T007, T008 (US1 tests) can be written in parallel.
- T016, T017, T018 (US2 tests) can be written in parallel.
- T024, T025, T026 (US3 tests) can be written in parallel.
- T028–T031 (Application read carriers) can be updated in parallel.
- T037–T040 (frontend submit logic and form copy) can be done in parallel.
- T041, T042 (tag chip display) can be done in parallel.
- T044–T049 (final validation steps) can run in parallel after T043.

---

## Implementation Strategy

### Backend-First Delivery

1. Phase 1: verify schema and baseline tests.
2. Phase 2: `TagNormalizer` — the shared normalization helper.
3. Phase 3: create-post tags — first end-to-end tag slice (domain → app →
   infra → API).
4. Phase 4: edit-post tags — reuses the domain and infrastructure patterns
   from Phase 3.
5. Phase 5: read tags — wire aggregated SQL reads, result types, and API
   response DTOs.
6. Phase 6: frontend after backend is stable and tested.
7. Final Phase: full-stack validation through Docker Compose.

### What Not To Do

- Do not skip to frontend until the backend tests pass.
- Do not add tag management, search, autocomplete, analytics, or
  color customization.
- Do not change visibility, availability, publish date, or expire date behavior.
- Do not introduce Entity Framework, Dapper, Mediator, or MediatR.
- Do not scatter arbitrary Tailwind styles — use existing `DESIGN.md` badge
  tokens.

## Notes

- `[P]` tasks are parallelizable across different files.
- Story labels (US1–US4) trace to the feature plan and spec.
- Every backend layer leaves a failing test before production code is written.
- Frontend tag display uses existing design tokens; no new component library or
  design pattern is introduced.
