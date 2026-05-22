# Tasks: Persist Post Publish and Expiration Dates

**Input**: Design documents from `specs/019-persist-post-dates/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/api-contracts.md

**Tests**: Backend tests are mandatory (test-first). Each story starts with
failing tests before any production code is written.

**Organization**: Tasks are grouped by user story to enable independent
implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no shared dependencies)
- **[Story]**: User story this task belongs to (US1, US2, US3)
- Exact file paths are included in every description

## Path Conventions

- **Domain**: `src/backend/BlogPlatform.Domain/Posts/`
- **Application**: `src/backend/BlogPlatform.Application/Posts/`
- **Infrastructure**: `src/backend/BlogPlatform.Infrastructure/Posts/`
- **API**: `src/backend/BlogPlatform.Api/`
- **Application tests**: `src/backend/BlogPlatform.Application.Tests/Posts/`
- **Infrastructure tests**: `src/backend/BlogPlatform.Infrastructure.Tests/Posts/`
- **API tests**: `src/backend/BlogPlatform.Api.Tests/Posts/`
- **Frontend**: `src/frontend/blog-web/src/features/posts/`
- **Database**: `database/scripts/`

---

## User Stories

| ID | Story | Priority |
|----|-------|----------|
| US1 | As a blog author, I can set a publish date and expiration date when creating or editing a post, so that I can schedule or expire content and the dates are persisted and returned by the API | P1 — MVP |
| US2 | As an anonymous visitor, I can only see posts that are currently within their publish/expiration window on the public listing and post detail pages | P2 |
| US3 | As an anonymous or authenticated visitor, search results respect the publish/expiration window (own posts are always visible to authenticated authors) | P3 |

---

## Phase 1: Setup & Verification

**Purpose**: Confirm the database baseline, update seed data, and verify that
all existing tests pass before any changes are made.

- [X] T001 Confirm `database/scripts/005-create-posts.sql` already declares
      `publish_date TIMESTAMPTZ NULL` and `expire_date TIMESTAMPTZ NULL` on the
      `posts` table — no DDL change is needed; document the confirmation
- [X] T002 Update seed data script(s) under `database/scripts/` to include
      representative posts: one with no dates (always visible), one with a past
      `publish_date` and null `expire_date` (active), one with a future
      `publish_date` (scheduled), and one with a past `expire_date` (expired);
      ensure scripts remain idempotent for Docker Compose restart

**Checkpoint**: DB schema confirmed; seed data covers all visibility scenarios

---

## Phase 2: Foundational Backend (Blocking for all stories)

**Purpose**: Extend the `BlogPost` domain entity with the two new date
properties and update all factory method signatures. This is a prerequisite for
every story because Application commands, Infrastructure mapping, and
Application handlers all depend on these changes.

**CRITICAL**: No story-level implementation begins until this phase is complete.

- [X] T003 Add private `DateTimeOffset? _publishDate` and
      `DateTimeOffset? _expirationDate` fields and corresponding public
      `PublishDate` and `ExpirationDate` properties to the private constructor in
      `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`; update `Create()` and
      `Rehydrate()` factory methods to accept `DateTimeOffset? publishDate = null`
      and `DateTimeOffset? expirationDate = null` optional parameters and assign
      them; leave `IsPubliclyReadable` unchanged for now (date awareness is added
      in US2)
- [X] T004 Update `BlogPost.Update()` in
      `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs` to accept
      `DateTimeOffset? publishDate = null` and `DateTimeOffset? expirationDate =
      null` optional parameters; return a new `BlogPost` instance carrying the
      caller-supplied date values (replacing the existing dates on the post)

**Checkpoint**: `BlogPost` carries date properties; all existing tests still
compile and pass; `IsPubliclyReadable` is unchanged (date filter not yet active)

---

## Phase 3: US1 — Persist Dates on Create and Edit (Priority: P1) 🎯 MVP

**Goal**: Authors can supply a publish date and expiration date when creating or
editing a post; both values are persisted to `publish_date`/`expire_date`
columns and returned in all API responses for that post.

**Independent Test**: POST `/api/posts` with `"publishDate"` and
`"expirationDate"` fields returns 201 with those values in the response; GET
`/api/posts/mine/{id}` returns the same values; PUT `/api/posts/{id}` with
updated dates returns the new values; PUT with `expirationDate <= publishDate`
returns 400 ProblemDetails.

### Tests for US1 (write first — verify they fail before implementing) ⚠️

- [X] T005 [P] [US1] Create
      `src/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerDateTests.cs`
      with test cases: (a) create with only `PublishDate` set — result carries
      correct `PublishDate`, (b) create with only `ExpirationDate` set — result
      carries correct `ExpirationDate`, (c) create with both dates where
      `ExpirationDate > PublishDate` — success, (d) create with both dates where
      `ExpirationDate <= PublishDate` — returns validation error, (e) create with
      neither date — both null in result
- [X] T006 [P] [US1] Create
      `src/backend/BlogPlatform.Application.Tests/Posts/EditBlogPostHandlerDateTests.cs`
      with test cases: (a) edit updating both dates to valid values — result
      carries new dates, (b) edit with `expirationDate <= publishDate` — returns
      validation error, (c) edit clearing both dates to null — result has null
      dates
- [X] T007 [P] [US1] Create
      `src/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryDatePersistenceTests.cs`
      with test cases: (a) `CreateAsync` with `PublishDate` and `ExpirationDate`
      set — `SELECT` returns matching values, (b) `CreateAsync` with null dates —
      `SELECT` returns nulls, (c) `UpdateAsync` with new date values — `SELECT`
      returns updated values, (d) `UpdateAsync` clearing dates to null — `SELECT`
      returns nulls

### Application Implementation for US1

- [X] T008 [P] [US1] Add `DateTimeOffset? PublishDate = null` and
      `DateTimeOffset? ExpirationDate = null` optional record parameters to
      `src/backend/BlogPlatform.Application/Posts/CreateBlogPostCommand.cs`
- [X] T009 [P] [US1] Add `DateTimeOffset? PublishDate = null` and
      `DateTimeOffset? ExpirationDate = null` optional record parameters to
      `src/backend/BlogPlatform.Application/Posts/EditBlogPostCommand.cs`
- [X] T010 [US1] In
      `src/backend/BlogPlatform.Application/Posts/CreateBlogPostHandler.cs`, add
      date consistency validation before calling `BlogPost.Create()`: if both
      `command.PublishDate` and `command.ExpirationDate` are non-null and
      `ExpirationDate <= PublishDate`, return a validation failure result with
      error key `"expirationDate"` and message `"Expiration date must be after
      publish date."`; pass `publishDate: command.PublishDate` and
      `expirationDate: command.ExpirationDate` to `BlogPost.Create()`
- [X] T011 [US1] In
      `src/backend/BlogPlatform.Application/Posts/EditBlogPostHandler.cs`, add
      the same date consistency validation before calling `post.Update()`; pass
      `publishDate: command.PublishDate` and `expirationDate:
      command.ExpirationDate` to `post.Update()`
- [X] T012 [P] [US1] Add `DateTimeOffset? PublishDate` and
      `DateTimeOffset? ExpirationDate` properties to
      `src/backend/BlogPlatform.Application/Posts/CreateBlogPostResult.cs` and
      `src/backend/BlogPlatform.Application/Posts/EditBlogPostResult.cs`; update
      the construction/mapping in each handler to populate these from the returned
      `BlogPost.PublishDate` and `BlogPost.ExpirationDate`
- [X] T013 [P] [US1] Add `DateTimeOffset? PublishDate` and
      `DateTimeOffset? ExpirationDate` properties to
      `src/backend/BlogPlatform.Application/Posts/GetOwnedPostByIdResult.cs` and
      `src/backend/BlogPlatform.Application/Posts/OwnedPostListItem.cs`; update
      `From()` factory methods to map from `BlogPost.PublishDate` and
      `BlogPost.ExpirationDate`

### Infrastructure (Raw SQL) Implementation for US1

- [X] T014 [US1] In `PostgreSqlPostRepository.CreateAsync()` in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`,
      replace the hardcoded expression `CASE WHEN @public_post THEN NOW() ELSE
      NULL END` for `publish_date` with a `@publish_date` parameter; add
      `expire_date` to the INSERT column list with parameter `@expire_date`;
      update the `RETURNING` clause to add `publish_date, expire_date`; update
      `BuildPostInsertCommand` to add `NpgsqlParameter<DateTimeOffset?>` entries
      for `@publish_date` and `@expire_date` using `NpgsqlDbType.TimestampTz`
- [X] T015 [US1] In `PostgreSqlPostRepository.UpdateAsync()` in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`,
      add `publish_date = @publish_date, expire_date = @expire_date` to the SET
      clause; update the `RETURNING` clause to add `publish_date, expire_date`;
      update `BuildPostUpdateCommand` to add the two `NpgsqlParameter<DateTimeOffset?>`
      entries
- [X] T016 [US1] Update all `SELECT` statements in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
      (`GetByIdAsync`, `GetByIdForAuthorAsync`, `GetPublicReadByIdAsync`,
      `ListByAuthorAsync`, `ListPublicReadAsync`, `SearchPublicReadAsync`) to
      project `p.publish_date, p.expire_date` after the existing 8 columns;
      update `MapPost()` and `MapPostFromReader()` to read `publish_date` and
      `expire_date` via `reader.GetOrdinal("publish_date")` and
      `reader.GetOrdinal("expire_date")` (use named ordinal lookups to avoid
      fragile positional indexing) and pass them to `BlogPost.Rehydrate()`

### API Contract and Controller Implementation for US1

- [ ] T017 [P] [US1] Add `public DateTimeOffset? PublishDate { get; init; }` and
      `public DateTimeOffset? ExpirationDate { get; init; }` to
      `src/backend/BlogPlatform.Api/Contracts/Posts/CreatePostRequest.cs` and
      `src/backend/BlogPlatform.Api/Contracts/Posts/UpdatePostRequest.cs`
- [ ] T018 [P] [US1] Add `PublishDate?` and `ExpirationDate?` properties to
      `src/backend/BlogPlatform.Api/Contracts/Posts/PostMutationResponse.cs`,
      `src/backend/BlogPlatform.Api/Contracts/Posts/OwnedPostDetailResponse.cs`,
      and `src/backend/BlogPlatform.Api/Contracts/Posts/OwnedPostSummaryResponse.cs`
- [ ] T019 [US1] In
      `src/backend/BlogPlatform.Api/Controllers/PostsController.cs`, update the
      `Create` action to pass `PublishDate: request.PublishDate` and
      `ExpirationDate: request.ExpirationDate` to `CreateBlogPostCommand` and map
      `result.PublishDate` / `result.ExpirationDate` to `PostMutationResponse`;
      update the `Update` action similarly using `EditBlogPostCommand`

### API Tests for US1

- [ ] T020 [P] [US1] Update
      `src/backend/BlogPlatform.Api.Tests/Posts/CreatePostApiTests.cs`: add test
      cases for POST with valid `publishDate`/`expirationDate` → 201 with dates in
      response; POST with `expirationDate <= publishDate` → 400 ProblemDetails
      with `"expirationDate"` error key
- [ ] T021 [P] [US1] Update
      `src/backend/BlogPlatform.Api.Tests/Posts/UpdatePostApiTests.cs`: add test
      cases for PUT with valid dates → 200 with updated dates; PUT with invalid
      date range → 400 ProblemDetails

### Backend test gate for US1

- [ ] T022 [US1] Run the full backend test suite (`dotnet test` from repo root or
      `src/backend`) and confirm all US1 Application.Tests,
      Infrastructure.Tests, and Api.Tests cases pass; confirm no regressions in
      existing create, edit, owned read, tag, and authorization tests

### Frontend Implementation for US1

- [ ] T023 [US1] In
      `src/frontend/blog-web/src/features/posts/post.types.ts`, add
      `publishDate?: string | null` and `expirationDate?: string | null` to the
      `PostMutationRequest` type; rename `expireDate` to `expirationDate` in
      `PostEditorDraft` and update all references in the same file
- [ ] T024 [P] [US1] In
      `src/frontend/blog-web/src/features/posts/CreatePostPage.tsx`, update the
      initial draft object to use `expirationDate: ''` (was `expireDate`); in the
      `createPost(...)` call include `publishDate: draft.publishDate || null` and
      `expirationDate: draft.expirationDate || null`; convert non-empty date
      strings to ISO UTC format before submission (e.g.
      `new Date(draft.publishDate).toISOString()` if the value is non-empty)
- [ ] T025 [P] [US1] In
      `src/frontend/blog-web/src/features/posts/EditPostPage.tsx`, populate the
      form draft with `publishDate` and `expirationDate` from the loaded post
      response (format the API value for `<input type="datetime-local">` by
      trimming the `Z` suffix to produce `YYYY-MM-DDTHH:mm`); include both fields
      in the `updatePost(...)` call with null normalization as in T024
- [ ] T026 [US1] In
      `src/frontend/blog-web/src/features/posts/PostForm.tsx`, rename `expireDate`
      field references to `expirationDate` to match the updated `PostEditorDraft`
      type; remove the "Metadata fields are ready in the UI and awaiting backend
      support" status note (currently around lines 165-173); add an inline
      validation error display below the expiration date input that renders an
      error message when the API returns a 400 response with an `"expirationDate"`
      error; follow existing `DESIGN.md` theme tokens for error text styling

**Checkpoint**: POST, PUT, GET `/api/posts/mine/*` all include date fields;
frontend create/edit forms send and pre-populate dates; invalid date range
returns 400 inline error

---

## Phase 4: US2 — Public Listing and Detail Visibility (Priority: P2)

**Goal**: Anonymous visitors only see posts within their active
publish/expiration window on the public listing (`GET /api/posts`) and post
detail (`GET /api/posts/{id}`) endpoints.

**Independent Test**: Create a scheduled post (future `publishDate`) and an
expired post (past `expirationDate`) directly in the DB (or via API as
authenticated author); confirm `GET /api/posts` (anonymous) excludes both;
confirm `GET /api/posts/{id}` (anonymous) for each returns 404.

### Tests for US2 (write first — verify they fail before implementing) ⚠️

- [ ] T027 [P] [US2] Create
      `src/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerDateTests.cs`
      with test cases: (a) post with `PublishDate` in the future → handler
      returns null/not-found, (b) post with `ExpirationDate` in the past → handler
      returns null/not-found, (c) post with `PublishDate` in the past and null
      `ExpirationDate` → handler returns result, (d) post with both dates in valid
      window → handler returns result, (e) post with null dates → handler returns
      result
- [ ] T028 [P] [US2] Create
      `src/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerDateTests.cs`
      with test cases: (a) scheduled post excluded from anonymous listing, (b)
      expired post excluded from anonymous listing, (c) post with no dates
      included, (d) post with active window included; use the existing stub
      pattern from `SearchPostRepositoryStub.cs` to control the test data
- [ ] T029 [P] [US2] Create or extend
      `src/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositoryVisibilityTests.cs`
      with test cases for `ListPublicReadAsync`: (a) scheduled post absent, (b)
      expired post absent, (c) active dated post present, (d) no-date post
      present; and for `GetPublicReadByIdAsync`: (e) scheduled post returns null,
      (f) expired post returns null
- [ ] T030 [P] [US2] Add visibility test cases to
      `src/backend/BlogPlatform.Api.Tests/Posts/PublicPostListTests.cs`: scheduled
      and expired posts absent from `GET /api/posts` response
- [ ] T031 [P] [US2] Create or extend
      `src/backend/BlogPlatform.Api.Tests/Posts/PublicPostDetailTests.cs`: add
      test cases for `GET /api/posts/{id}` returning 404 for a scheduled post and
      404 for an expired post

### Application Implementation for US2

- [ ] T032 [US2] Update `IsPubliclyReadable` in
      `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs` to include date window
      conditions: `IsPublic && IsAvailable && (PublishDate is null || PublishDate
      <= DateTimeOffset.UtcNow) && (ExpirationDate is null || ExpirationDate >
      DateTimeOffset.UtcNow)`; no changes to `GetPublicPostByIdHandler.cs` or
      `ListPublicPostsHandler.cs` are needed — they already call `IsPubliclyReadable`
- [ ] T033 [P] [US2] Add `DateTimeOffset? PublishDate` and
      `DateTimeOffset? ExpirationDate` to
      `src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdResult.cs` and
      `src/backend/BlogPlatform.Application/Posts/PublicPostListItem.cs`; update
      `From()` factory methods to map from `BlogPost.PublishDate` and
      `BlogPost.ExpirationDate`

### Infrastructure (Raw SQL) Implementation for US2

- [ ] T034 [US2] Update the `ListPublicReadAsync` SQL in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
      to add a `WHERE` clause (or extend the existing one) with date window
      conditions: `AND (p.publish_date IS NULL OR p.publish_date <= NOW()) AND
      (p.expire_date IS NULL OR p.expire_date > NOW())`; also add
      `p.public_post = TRUE AND p.available = TRUE` if not already present
- [ ] T035 [US2] Update the `GetPublicReadByIdAsync` SQL query in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
      to add the same date window conditions to its `WHERE id = @id` clause:
      `AND (p.publish_date IS NULL OR p.publish_date <= NOW()) AND
      (p.expire_date IS NULL OR p.expire_date > NOW()) AND p.public_post = TRUE
      AND p.available = TRUE`

### API Contract Implementation for US2

- [ ] T036 [P] [US2] Add `PublishDate?` and `ExpirationDate?` properties to
      `src/backend/BlogPlatform.Api/Contracts/Posts/PublicPostDetailResponse.cs`
      and
      `src/backend/BlogPlatform.Api/Contracts/Posts/PublicPostSummaryResponse.cs`;
      update mapping from Application result types in `PublicPostsController.cs`
      if any explicit mapping exists there

### Backend test gate for US2

- [ ] T037 [US2] Run the full backend test suite and confirm all US2 Application,
      Infrastructure, and API visibility tests pass; confirm `IsPubliclyReadable`
      change causes no regressions in ownership, listing, or detail handler tests

### Frontend for US2 (optional display)

- [ ] T038 [US2] In post detail view and owned post list/detail components, add
      optional display of `publishDate` and `expirationDate` labels where non-null
      (e.g. "Scheduled: {date}" or "Expires: {date}"); use existing theme tokens
      from `DESIGN.md` — no new CSS classes; authenticated owner view may show a
      "Scheduled" badge for future-published posts

**Checkpoint**: Anonymous listing and detail honor the publish/expiration window;
scheduled and expired posts return 404 on detail and are absent from listing

---

## Phase 5: US3 — Search Visibility (Priority: P3)

**Goal**: Anonymous search (`GET /api/posts?q=...`) excludes scheduled and
expired posts. Authenticated search includes the requesting user's own posts
regardless of their window but still excludes other users' out-of-window posts.

**Independent Test**: Create a scheduled post for user A; search anonymously for
a term in its title → result is empty; search as user A (authenticated) → post
appears; search as user B (authenticated) → post absent.

### Tests for US3 (write first — verify they fail before implementing) ⚠️

- [ ] T039 [P] [US3] Add date visibility test cases to
      `src/backend/BlogPlatform.Application.Tests/Posts/SearchPostsAnonymousTests.cs`:
      (a) scheduled post excluded from anonymous search result, (b) expired post
      excluded from anonymous search result
- [ ] T040 [P] [US3] Add date visibility test cases to
      `src/backend/BlogPlatform.Application.Tests/Posts/SearchPostsAuthenticatedTests.cs`:
      (a) authenticated user's own scheduled post is present in search result, (b)
      authenticated user's own expired post is present in search result, (c)
      another user's scheduled post is absent from authenticated search result
- [ ] T041 [P] [US3] Add date visibility test cases to
      `src/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositorySearchAnonymousTests.cs`
      and
      `src/backend/BlogPlatform.Infrastructure.Tests/Posts/PostRepositorySearchAuthenticatedTests.cs`:
      verify SQL `SearchPublicReadAsync` excludes out-of-window posts for
      anonymous callers and preserves the ownership bypass for authenticated callers
- [ ] T042 [P] [US3] Add date visibility test cases to
      `src/backend/BlogPlatform.Api.Tests/Posts/PublicPostSearchAnonymousTests.cs`:
      scheduled/expired posts absent from anonymous `GET /api/posts?q=...`
      response
- [ ] T043 [P] [US3] Add date visibility test cases to
      `src/backend/BlogPlatform.Api.Tests/Posts/PublicPostSearchAuthenticatedTests.cs`:
      own scheduled/expired post present; other user's scheduled/expired post absent

### Infrastructure (Raw SQL) Implementation for US3

- [ ] T044 [US3] Update the `SearchPublicReadAsync` SQL `WHERE` clause in
      `src/backend/BlogPlatform.Infrastructure/Posts/PostgreSqlPostRepository.cs`
      to wrap the public-post condition with date window conditions while
      preserving the ownership bypass:
      ```
      WHERE p.available = TRUE
        AND (
          (   p.public_post = TRUE
              AND (p.publish_date IS NULL OR p.publish_date <= NOW())
              AND (p.expire_date IS NULL OR p.expire_date > NOW())
          )
          OR (@requesting_user_id IS NOT NULL AND p.user_id = @requesting_user_id)
        )
        AND (text search conditions unchanged)
      ```
      No changes to `ListPublicPostsHandler.cs` or `SearchPostsHandler.cs` are
      needed — application-layer visibility checks already use `IsPubliclyReadable`

### Backend test gate for US3

- [ ] T045 [US3] Run the full backend test suite and confirm all US3 Application,
      Infrastructure, and API search visibility tests pass; confirm no regressions
      in existing anonymous search, authenticated search, or tags search tests

**Checkpoint**: Search respects publish/expiration window; authenticated search
ownership bypass is preserved

---

## Final Phase: Polish & Validation

**Purpose**: End-to-end verification, documentation, and scope confirmation.

- [ ] T046 [P] Run the complete backend test suite from repo root:
      `dotnet test src/backend/BlogPlatform.sln` (or equivalent) and confirm all
      tests pass with no skipped or errored tests
- [ ] T047 [P] Run the frontend TypeScript build to confirm no type errors:
      `cd src/frontend/blog-web && npm run build`
- [ ] T048 Bring up the full stack with Docker Compose; navigate to the create
      post page as an authenticated user; create a post with a publish date 1 hour
      in the future; verify the post does not appear in the anonymous public
      listing; advance the `publish_date` in the DB to a past time; refresh and
      confirm the post now appears
- [ ] T049 From the edit post page, load an existing dated post; verify
      `publishDate` and `expirationDate` fields are pre-populated; update the
      expiration date to a date before the publish date; verify a validation error
      appears inline and no update is saved; fix the dates; verify the post saves
      correctly and the updated dates appear in the response
- [ ] T050 [P] Verify Swagger UI (e.g. at `/swagger`) shows `publishDate` and
      `expirationDate` fields in the `CreatePostRequest`, `UpdatePostRequest`, and
      all post response schemas; confirm the 400 validation error for invalid date
      range is documented via ProblemDetails
- [ ] T051 [P] Check the browser console for JavaScript errors or warnings during
      create, edit, listing, and search flows; confirm the "awaiting backend
      support" note is gone from the post form
- [ ] T052 Confirm scope: no background jobs, no notification system, no timezone
      UI, no tag changes, no major frontend redesign, and no Entity Framework or
      Dapper were introduced; confirm all existing ownership, authorization, tag,
      category, and search tests still pass without modification

---

## Dependencies & Execution Order

### Phase Dependencies

```
Phase 1 (Setup/Verification)
  └─→ Phase 2 (Foundational: BlogPost domain entity)
        └─→ Phase 3 (US1: Persistence — commands, repository, API, frontend)
              └─→ Phase 4 (US2: Listing/detail visibility)
                    └─→ Phase 5 (US3: Search visibility)
                          └─→ Final Phase (Validation)
```

### Within Each Phase

1. Tests are written and confirmed failing before production code
2. Domain/Application changes precede Infrastructure SQL changes
3. Infrastructure SQL changes precede API DTO changes
4. API DTO changes precede controller mapping changes
5. Frontend tasks start only after backend tests pass (T022, T037, T045)

### Parallel Opportunities

**Phase 2**: T003 and T004 are sequential (T004 extends what T003 creates)

**Phase 3 tests** (T005, T006, T007): All three can be written in parallel

**Phase 3 Application commands** (T008, T009): Parallel — different files

**Phase 3 Application results** (T012, T013): Parallel — different files

**Phase 3 API DTOs** (T017, T018): Parallel — different files

**Phase 3 Frontend** (T024, T025): Parallel — different files

**Phase 4 tests** (T027–T031): All can be written in parallel

**Phase 5 tests** (T039–T043): All can be written in parallel

**Final Phase** (T046, T047, T050, T051): Can run in parallel after T045

---

## Implementation Strategy

### MVP First (Phase 3 — US1)

1. Confirm DB schema (T001) and update seed data (T002)
2. Extend domain entity (T003–T004)
3. Write failing Application and Infrastructure tests (T005–T007)
4. Implement Application commands, handlers, results (T008–T013)
5. Implement Infrastructure SQL (T014–T016)
6. Implement API DTOs and controller mapping (T017–T019)
7. Add/update API tests (T020–T021)
8. Run test gate (T022)
9. Wire frontend (T023–T026)

### Incremental Delivery

After US1 is passing and frontend is wired:
- Deliver US2 (public visibility filtering) — pure backend SQL + domain change
- Deliver US3 (search visibility) — single SQL WHERE update

### Notes

- `[P]` tasks indicate parallelizable work; assign to separate agents or
  developers without conflicts
- Every backend story leaves behind automated tests that can be run independently
- Frontend tasks reference existing `DESIGN.md` tokens; no new styles should be
  introduced
- The `expireDate` → `expirationDate` rename in `post.types.ts` and
  `PostForm.tsx` must be done in a single pass (T023 + T026) to avoid TypeScript
  compile errors mid-implementation
- Existing tests for ownership, authorization, reactions, tags, and categories
  must not be modified unless they fail due to the BlogPost factory method
  signature change (optional params with defaults should prevent breakage)
