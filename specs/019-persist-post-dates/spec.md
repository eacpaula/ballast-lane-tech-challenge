# Feature Specification: Persist Post Publish and Expiration Dates

**Feature Branch**: `019-persist-post-dates`

**Created**: 2026-05-22

**Status**: Draft

---

## 1. Feature Summary

Blog posts currently support a publish date and an expiration date in the
create/edit UI, but these fields are not persisted or evaluated at any
layer. This feature closes that gap: it wires the fields through the full
stack — Application layer commands and queries, raw-SQL repositories,
ASP.NET Core API contracts, and the React frontend — so that the dates are
saved, returned, and used to gate public visibility of posts.

---

## 2. Goal

Persist `PublishDate` and `ExpirationDate` for blog posts and apply those
values to all public-facing post surfaces (listing, detail, and search) so
that only posts within their active window are visible to anonymous users.

---

## 3. Functional Requirements

- **FR-001**: The system MUST accept an optional `PublishDate` and an
  optional `ExpirationDate` when a post is created.
- **FR-002**: The system MUST accept an optional `PublishDate` and an
  optional `ExpirationDate` when a post is updated.
- **FR-003**: When both dates are provided, the system MUST reject requests
  where `ExpirationDate` is earlier than or equal to `PublishDate`.
- **FR-004**: The system MUST persist `PublishDate` and `ExpirationDate`
  alongside the post record using the existing raw-SQL repository pattern.
- **FR-005**: The system MUST return `PublishDate` and `ExpirationDate` in
  every API response that includes post data (create, update, get, list,
  search).
- **FR-006**: Public post listing MUST exclude posts whose `PublishDate`
  is in the future or whose `ExpirationDate` is in the past.
- **FR-007**: Public post detail MUST refuse to return a post that does not
  satisfy the public visibility window.
- **FR-008**: Anonymous post search MUST exclude posts outside the active
  publish/expiration window.
- **FR-009**: Authenticated search MUST include the requesting user's own
  posts regardless of their publish/expiration window, while still excluding
  other users' posts that do not satisfy the window.
- **FR-010**: Business-rule validation (date consistency, visibility logic)
  MUST live entirely in the Application layer and MUST NOT be duplicated in
  API controllers or Infrastructure.
- **FR-011**: The database schema MUST include `publish_date` and
  `expiration_date` columns on the posts table (nullable `timestamptz`).
- **FR-012**: The frontend create-post form MUST transmit `publishDate` and
  `expirationDate` to the API when the user provides them.
- **FR-013**: The frontend edit-post form MUST pre-populate `publishDate`
  and `expirationDate` from the existing post data returned by the API.

---

## 4. Publish/Expiration Date Rules

| Rule | Description |
|------|-------------|
| Both dates are optional | A post without either date is always within its window. |
| Publish date only | Post becomes visible when `PublishDate <= now`. |
| Expiration date only | Post is visible until `ExpirationDate` passes. |
| Both dates provided | Both constraints apply simultaneously. |
| Date consistency | `ExpirationDate` MUST be strictly greater than `PublishDate` when both are set. Validated in the Application layer before any persistence. |
| Null semantics | A null `PublishDate` means no scheduled start. A null `ExpirationDate` means no scheduled end. |
| Timestamps | All dates are stored as UTC `timestamptz` in PostgreSQL. |

---

## 5. Visibility Rules

A post is **publicly visible** when ALL of the following are true:

1. `IsPublic = true`
2. `Available = true`
3. `PublishDate IS NULL OR PublishDate <= NOW()`
4. `ExpirationDate IS NULL OR ExpirationDate > NOW()`

**Anonymous users** (listing, detail, search): full visibility rule applies.

**Authenticated users — own posts**: all own posts are accessible regardless
of the visibility rule; existing ownership rules continue to govern access.

**Authenticated users — other users' posts**: the full visibility rule
applies. Authenticated search MUST NOT expose another user's scheduled,
expired, or private post.

---

## 6. Backend Scope

### Database
- Verify `posts` table has `publish_date TIMESTAMPTZ NULL` and
  `expiration_date TIMESTAMPTZ NULL` columns.
- Add columns via `ALTER TABLE` in the init SQL or a migration script if they
  do not exist.
- Update seed data to include representative rows: scheduled, active, and
  expired.

### Application Layer
- Extend `CreatePostCommand` / input to carry `PublishDate?` and
  `ExpirationDate?`.
- Extend `EditPostCommand` / input to carry `PublishDate?` and
  `ExpirationDate?`.
- Extend post result/output DTOs to expose `PublishDate?` and
  `ExpirationDate?`.
- Add validation rule: when both dates are provided,
  `ExpirationDate > PublishDate`.
- Update public listing use-case to apply the visibility window.
- Update public post-detail use-case to apply the visibility window.
- Update search use-case to apply the visibility window for anonymous callers
  and the ownership override for authenticated callers.

### Repository Abstractions
- Extend the post repository interface to accept and return `PublishDate?`
  and `ExpirationDate?` on create, update, and read operations.
- Extend public listing, detail, and search repository methods to filter by
  the visibility window.

### Infrastructure (Raw SQL)
- Update the `INSERT` statement in the create-post repository method.
- Update the `UPDATE` statement in the edit-post repository method.
- Update `SELECT` statements in all read, list, and search repository methods
  to project the two new columns.
- Add `WHERE` conditions implementing the visibility window for public queries.
- Implement the authenticated-search ownership bypass with a conditional
  clause: `(visibility_rule) OR author_id = @userId`.

---

## 7. API Contract Scope

### Request bodies (create and edit post)

```
publishDate?    : string (ISO 8601 UTC) | null
expirationDate? : string (ISO 8601 UTC) | null
```

### Response bodies (all post endpoints)

```
publishDate?    : string (ISO 8601 UTC) | null
expirationDate? : string (ISO 8601 UTC) | null
```

### Error responses

- `400 Bad Request` with a ProblemDetails body when
  `expirationDate <= publishDate`.
- Existing `404 Not Found` behavior for the detail endpoint continues to
  apply when a post is outside the visibility window for anonymous callers.

### Affected endpoints

| Method | Path | Change |
|--------|------|--------|
| `POST` | `/posts` | Accept and persist date fields |
| `PUT` | `/posts/{id}` | Accept and update date fields |
| `GET` | `/posts/{id}` | Return date fields; apply visibility window for anonymous callers |
| `GET` | `/posts` | Return date fields; apply visibility window for anonymous callers |
| `GET` | `/posts/search` | Return date fields; apply visibility window with auth ownership override |

---

## 8. Frontend Scope

### Create post form
- Map the existing `publishDate` and `expirationDate` input fields to the
  corresponding API request fields.
- Send the values (or `null`) in the POST body.
- Display an inline validation error if the API returns 400 for date
  inconsistency.

### Edit post form
- Populate `publishDate` and `expirationDate` inputs from the post data
  returned by the GET endpoint.
- Send updated values (or `null`) in the PUT body.

### Post cards / detail view
- Optionally display a "Scheduled" label or publish date when `publishDate`
  is in the future (visible to the authenticated owner only).
- Optionally display an expiration date label when `expirationDate` is set.
- No major redesign; additions must follow existing TailwindCSS theme tokens
  from `DESIGN.md`.

### Listing and search pages
- No behavioral change required on the frontend; filtering is enforced by
  the backend. The frontend renders what the API returns.

---

## 9. Cache Impact

- If a cached post listing or search result set exists, the visibility window
  is time-sensitive: a cached response may become stale as posts cross their
  publish or expiration boundaries.
- Cache TTL for public listing and search MUST be short enough that
  visibility changes propagate within an acceptable window. Use the existing
  TTL if it is already short; reduce it if not.
- Cache keys for public listing and search MUST NOT include
  `publishDate`/`expirationDate` as caller-supplied query parameters; they
  are server-side filters.
- If Redis does not already exist in the project, no new caching
  infrastructure is introduced by this feature.
- Post create and edit operations MUST invalidate any cached listing or
  search entries that include the modified post.

---

## 10. Test Scope

### Application layer unit tests
- Create post with publish date only → persisted correctly.
- Create post with expiration date only → persisted correctly.
- Create post with both dates where expiration > publish → accepted.
- Create post with both dates where expiration <= publish → validation error.
- Edit post to clear dates (set to null) → dates removed.
- Edit post to update dates → new dates reflected in output.

### Repository integration tests (PostgreSQL)
- Insert post with dates → SELECT returns correct values.
- Public listing query excludes a post whose `publishDate` is in the future.
- Public listing query excludes a post whose `expirationDate` is in the past.
- Public listing query includes a post within its active window.
- Public detail query returns not-found for a post outside its window.
- Anonymous search excludes posts outside the window.
- Authenticated search includes the owner's own scheduled or expired posts.
- Authenticated search excludes another user's scheduled or expired posts.

### API integration tests
- `POST /posts` with valid dates → 201, response includes date fields.
- `POST /posts` with expiration <= publish → 400 ProblemDetails.
- `PUT /posts/{id}` with updated dates → 200, response reflects new values.
- `GET /posts` (anonymous) → scheduled and expired posts absent.
- `GET /posts/{id}` (anonymous) for an out-of-window post → 404.
- `GET /posts/search` (anonymous) → out-of-window posts absent.
- `GET /posts/search` (authenticated, own post, out-of-window) → own post present.
- `GET /posts/search` (authenticated, other user's out-of-window post) → absent.

### Frontend (manual or E2E)
- Create form: date fields submit correctly and API receives expected values.
- Edit form: existing dates pre-populate; updated dates persist on save.
- Inline error appears when expiration precedes publish date.

---

## 11. Out of Scope

- Background jobs to automatically publish or expire posts.
- Notification system for scheduled or expiring posts.
- Calendar UI or date-picker complexity beyond existing input fields.
- Timezone preference management (all dates treated as UTC).
- Admin moderation workflow.
- Visibility or availability redesign beyond applying the date window rules.
- Redis implementation if it does not already exist in the project.
- Tag-related changes.
- Major frontend redesign or new pages.
- Entity Framework, Dapper, Mediator, or MediatR.

---

## 12. Acceptance Criteria

- **AC-01**: Creating a post with a publish date and/or expiration date
  persists both values and returns them in the API response.
- **AC-02**: Editing a post updates publish date and expiration date to the
  submitted values, including `null` to clear them.
- **AC-03**: Submitting a create or edit request where `expirationDate <=
  publishDate` returns HTTP 400 with a ProblemDetails body; no post is
  created or modified.
- **AC-04**: Anonymous `GET /posts` does not include posts whose
  `publishDate` is in the future.
- **AC-05**: Anonymous `GET /posts` does not include posts whose
  `expirationDate` is in the past.
- **AC-06**: Anonymous `GET /posts/{id}` for a scheduled or expired post
  returns HTTP 404.
- **AC-07**: Anonymous `GET /posts/search` excludes posts outside their
  active window.
- **AC-08**: Authenticated `GET /posts/search` includes the requesting
  user's own scheduled or expired posts.
- **AC-09**: Authenticated `GET /posts/search` does not include another
  user's scheduled or expired posts.
- **AC-10**: The frontend edit form pre-populates existing publish and
  expiration date values.
- **AC-11**: All new behavior is covered by failing tests written before
  implementation.
- **AC-12**: No business rules are implemented in API controllers or
  Infrastructure.

---

## 13. Definition of Done

- [ ] Database schema includes `publish_date` and `expiration_date` columns.
- [ ] Application commands and queries carry the new date fields.
- [ ] Application validation rejects inconsistent date pairs before any
      persistence.
- [ ] Raw-SQL repository persists and retrieves both date columns.
- [ ] Raw-SQL queries for public listing, detail, and search apply the
      visibility window.
- [ ] Authenticated search query applies the ownership override correctly.
- [ ] API request and response DTOs include `publishDate` and
      `expirationDate`.
- [ ] All acceptance criteria (AC-01 through AC-12) have passing tests.
- [ ] Frontend create form sends date values to the API.
- [ ] Frontend edit form pre-populates and sends date values.
- [ ] Cache TTL and invalidation reviewed and adjusted if needed.
- [ ] No regressions in existing post create, edit, listing, detail, or
      search behavior.
- [ ] All CI checks pass.
