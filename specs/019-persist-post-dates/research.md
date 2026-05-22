# Research: Persist Post Publish and Expiration Dates

**Feature**: 019-persist-post-dates
**Date**: 2026-05-22

---

## Finding 1: Database Columns Already Exist

**Decision**: No schema migration is required.

**Rationale**: `005-create-posts.sql` already declares:
```sql
publish_date  TIMESTAMPTZ NULL
expire_date   TIMESTAMPTZ NULL
```
Both columns are nullable, which matches the optional semantics in the spec.

**What must change**: The existing `INSERT` statement uses a server-side
expression `CASE WHEN @public_post THEN NOW() ELSE NULL END` to auto-populate
`publish_date`. That expression must be replaced with a caller-supplied
parameter so the actual user-provided date (or null) is persisted. The
`expire_date` column is not set at all today.

---

## Finding 2: No Caching Layer Exists

**Decision**: Cache impact is not applicable for this feature.

**Rationale**: A thorough search of `backend/src` and `backend/tests` found no
`IMemoryCache`, `IDistributedCache`, or Redis usage. No cache key or TTL
management is needed.

---

## Finding 3: Domain Entity Strategy — Carry Dates, Update `IsPubliclyReadable`

**Decision**: Add `PublishDate?` (DateTimeOffset?) and `ExpirationDate?`
(DateTimeOffset?) to the `BlogPost` domain entity. Update `IsPubliclyReadable`
to evaluate the date window using `DateTimeOffset.UtcNow`.

**Rationale**: The domain entity is already the authority for `IsPublic`,
`IsAvailable`, and `IsPubliclyReadable`. Extending `IsPubliclyReadable` to also
check the date window keeps all visibility logic in one place and requires zero
changes in the Application handlers that already call it.

**Alternatives considered**:
- *Application-layer date check without domain change*: Requires every handler
  to re-implement date window logic and keep it in sync. Rejected as fragile.
- *IDateTimeProvider injection*: Provides deterministic time for unit tests but
  adds infrastructure dependency to the domain. Rejected for simplicity; the
  feature's date tests can control the window by choosing dates well in the
  future or past relative to test execution.

---

## Finding 4: Visibility Rule Coordination — Domain Filter + SQL WHERE Optimization

**Decision**: The domain `IsPubliclyReadable` property is the authoritative
in-memory filter. SQL `WHERE` clauses in the repository mirror the same
conditions as a performance optimization so rows outside the window are not
loaded.

**Rationale**: `ListPublicReadAsync` currently returns all posts and relies on
the Application handler to filter in memory via `IsPubliclyReadable`. Keeping
the Application-layer filter as the authority maintains the Clean Architecture
boundary. Adding the matching SQL conditions prevents large table scans as data
grows.

**Search differs**: `SearchPublicReadAsync` already filters in SQL
(including the ownership bypass). The date conditions must be added to that
`WHERE` clause consistently.

---

## Finding 5: `Update()` Method Must Accept Dates

**Decision**: Extend `BlogPost.Update()` to accept `publishDate?` and
`expirationDate?`. The returned new `BlogPost` instance carries the updated
dates.

**Rationale**: `Update()` currently copies `IsPublic` and `IsAvailable` from
the existing post without allowing changes. Dates must be similarly updatable on
edit. The `EditBlogPostHandler` hydrates a new `BlogPost` via `Update()` and
passes it to `IPostRepository.UpdateAsync()`.

---

## Finding 6: Date Field Name Standardization

**Decision**: Use `PublishDate` / `ExpirationDate` throughout the backend (.NET
properties, API JSON fields). Frontend draft type renames `expireDate` to
`expirationDate` for consistency; the existing `PostEditorDraft` shape in
`post.types.ts` is updated accordingly.

**Rationale**: The spec uses "expiration date" throughout. The database column
`expire_date` is an abbreviation; the API contract should use the full word to
match the spec and be self-documenting.

---

## Finding 7: Frontend Already Has UI Fields

**Decision**: No new UI components are needed. The existing `publishDate` and
`expireDate` inputs in `PostForm.tsx` are wired through by:
1. Adding `publishDate?` and `expirationDate?` to `PostMutationRequest`.
2. Updating `CreatePostPage` and `EditPostPage` to include these fields in the
   API call.
3. Updating `EditPostPage` to populate `publishDate` and `expirationDate` from
   the loaded post response.
4. Adding inline error display for the 400 date-inconsistency case.

**Date format note**: `<input type="datetime-local">` produces
`YYYY-MM-DDTHH:mm` (no timezone suffix). When passed to the API the value must
be sent as-is and the backend treats it as UTC, OR the frontend appends `Z`
before submission. Plan recommends appending `Z` in the API call helper for
clarity, or normalizing to empty string → null before sending.

---

## Finding 8: Repository `MapPost` Ordinal Shift

**Decision**: After adding `publish_date` and `expire_date` to all `SELECT`
column lists, the column ordinals in `MapPost` and `MapPostFromReader` shift.
The tags subquery becomes ordinal 10 (was 8). Both mapping functions must be
updated to use the new ordinals (or named column access via `reader.GetOrdinal`
for maintainability).

**Recommendation**: Use `reader.GetOrdinal("column_name")` lookups in the
mapping functions to avoid fragile positional index coupling.

---

## Finding 9: `RETURNING` Clause Must Include Date Columns

**Decision**: Both the `INSERT ... RETURNING` and `UPDATE ... RETURNING`
statements must add `publish_date, expire_date` to the returned column list so
`MapPostFromReader` can read them without a second SELECT.

---

## Finding 10: Seed Data Update

**Decision**: Update seed data (if any SQL seed scripts exist) to include
representative posts: one scheduled (future `publish_date`), one active (past
`publish_date`, null `expire_date`), one expired (past `expire_date`), one
with no dates.

**Rationale**: This allows manual demo and integration-test setup to exercise
all visibility branches without creating fixtures from scratch in each test.
