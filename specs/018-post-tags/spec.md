# Feature Specification: Add Tags Support to Blog Posts

**Feature Branch**: `018-post-tags`

**Created**: 2026-05-22

**Status**: Draft

## 1. Feature Summary

Add tag support to blog posts so authors can label a post with zero or more
short, free-text tags. Tags travel through the full stack: the domain model,
the application create and edit use cases, the raw SQL persistence layer, the
API request and response contracts, and the frontend create, edit, and public
reading flows. The frontend post editor already exposes a comma-separated tags
input that is currently ignored; this feature makes that input meaningful end
to end.

## 2. Goal

Allow blog post authors to attach descriptive tags to a post when creating or
editing it, and allow all readers to see those tags on public post listings and
post details, without changing any existing visibility, ownership, or
authorization behavior.

## 3. Functional Requirements

- **FR-001**: A blog post MUST support zero or more tags.
- **FR-002**: Tags MUST be represented as a collection of string values in the
  domain model, the application layer, and the API contracts.
- **FR-003**: The create post flow MUST accept tags and persist them with the
  new post.
- **FR-004**: The edit post flow MUST accept tags and replace the post's
  existing tags with the submitted set.
- **FR-005**: Editing a post with an empty tag collection MUST result in the
  post having no tags.
- **FR-006**: Post API responses MUST include the post's tags.
- **FR-007**: Public post listing responses MUST include each post's tags when
  tags are available.
- **FR-008**: Public post detail responses MUST include the post's tags when
  tags are available.
- **FR-009**: All tag values MUST be normalized according to the rules in
  Section 4 before being persisted.
- **FR-010**: Tag normalization MUST be enforced in the application layer, not
  in API controllers, so the rules apply regardless of the entry point.
- **FR-011**: The frontend create and edit flows MUST convert the
  comma-separated tags input into a string collection before sending it to the
  API.
- **FR-012**: The frontend public post UI MUST display a post's tags when the
  post has tags.
- **FR-013**: Existing post visibility, ownership, and authorization rules MUST
  remain unchanged.
- **FR-014**: A post with no tags MUST continue to behave exactly as it does
  today across create, edit, listing, and detail flows.

## 4. Tag Normalization Rules

The following rules MUST be applied to a submitted set of tags before
persistence, and MUST produce the same result regardless of caller:

- **TN-001**: Each tag value MUST be trimmed of leading and trailing
  whitespace.
- **TN-002**: A tag that is empty or whitespace-only after trimming MUST be
  ignored and excluded from the persisted set.
- **TN-003**: Duplicate tags within the same post MUST be removed so that each
  tag appears at most once on a post.
- **TN-004**: Duplicate detection MUST be case-insensitive; tags that differ
  only by letter casing MUST be treated as the same tag.
- **TN-005**: When duplicates are collapsed, the first occurrence's trimmed
  value MUST be the value retained for the post.
- **TN-006**: A submitted set consisting only of empty or whitespace tags MUST
  result in a post with no tags rather than an error.
- **TN-007**: Tag order as submitted SHOULD be preserved after normalization,
  excluding removed empties and duplicates.

## 5. Backend Scope

- Add a tags collection to the `BlogPost` domain/entity model.
- Update the create post command/input model to accept tags.
- Update the edit/update post command/input model to accept tags.
- Apply the tag normalization rules from Section 4 inside the application
  layer so they are shared by create and edit.
- Update the post repository abstraction so tags can be persisted and
  retrieved alongside the post.
- Update the raw SQL repository implementation to write tags on create,
  replace tags on edit, and read tags back for single-post and listing queries.
- Update the PostgreSQL database scripts to add or complete the tag table(s)
  and post-to-tag relationship if they are missing or incomplete.
- Keep the application layer independent from the infrastructure layer; tag
  persistence details MUST stay behind the repository abstraction.
- Do not introduce Entity Framework, Dapper, Mediator, or MediatR.

## 6. API Contract Scope

- Update the create post request DTO to accept a tags collection of strings.
- Update the edit/update post request DTO to accept a tags collection of
  strings.
- Update the post response DTO to return the post's tags as a collection of
  strings.
- Ensure public post listing and post detail response DTOs include tags.
- Tags MUST be optional in request DTOs; omitting tags MUST be equivalent to
  submitting an empty collection.
- Swagger/OpenAPI documentation MUST reflect the tag fields automatically
  through the updated DTOs; no separate documentation work is required.
- API controllers MUST NOT contain tag business rules; they only pass tags to
  the application layer and return the application result.

## 7. Frontend Scope

- Update the post editor so the existing comma-separated tags string field is
  parsed into a string collection on submit.
- Update the create post flow to send the parsed tags to the API.
- Update the edit post flow to send the parsed tags to the API, and to
  pre-populate the tags input from the post's existing tags.
- Update the public post UI (listing and/or detail) to display a post's tags
  when present, keeping styling aligned with `DESIGN.md`, centralized
  TailwindCSS theme tokens, and existing shared component patterns.
- A post with no tags MUST render cleanly with no empty tag area.
- Do not redesign the post editor or public post UI beyond the changes needed
  to capture and display tags.

## 8. Test Scope

- Add or update Application tests for create post tag handling, including
  trimming, empty removal, and case-insensitive de-duplication.
- Add or update Application tests for edit post tag handling, including
  replacing existing tags and clearing tags with an empty set.
- Add or update repository tests for persisting tags on create, replacing tags
  on edit, and reading tags back for single-post and listing queries.
- Add or update API tests confirming request DTOs accept tags and response
  DTOs return tags for create, edit, listing, and detail.
- Add or update frontend tests where practical for comma-separated parsing and
  tag display, relying on lint, build, and full-stack manual verification for
  final validation.

## 9. Out of Scope

- Tag management screens
- Tag autocomplete
- Tag search or filtering
- Tag analytics
- Tag color customization
- Post visibility changes
- Post availability changes
- Publish date behavior
- Expire date behavior
- Redis caching changes
- Major frontend redesign
- Entity Framework
- Dapper
- Mediator or MediatR

## 10. Acceptance Criteria

- An author can create a post with zero, one, or many tags, and the tags are
  persisted with the post.
- An author can edit a post to add, change, or remove tags, and the saved set
  replaces the prior tags.
- Submitting tags with surrounding whitespace, empty values, or
  case-insensitive duplicates results in a normalized, trimmed, de-duplicated
  set on the post.
- Creating or editing a post with no tags produces a post with no tags and no
  errors.
- Post API responses for create, edit, public listing, and public detail
  include the post's tags.
- The frontend converts the comma-separated tags input into a string
  collection before calling the API, and displays a post's tags in the public
  UI when present.
- Existing post visibility, ownership, and authorization behavior is unchanged.

## 11. Definition of Done

- Tag support is fully specified for the create, edit, and read flows with no
  open clarification markers.
- Tag normalization rules are defined once and enforced in the application
  layer for both create and edit.
- Backend, API, and frontend scope remain limited to tag support for create,
  edit, and read; no tag management, search, or analytics is introduced.
- The application layer stays independent of infrastructure, and no Entity
  Framework, Dapper, Mediator, or MediatR is introduced.
- Existing visibility, ownership, and authorization behavior is preserved.
- The resulting slice remains suitable for a technical interview demo.

## Assumptions

- Tags are short free-text labels with no separate lifecycle, ownership, or
  identity beyond their string value on a post.
- Duplicate detection is case-insensitive, while the displayed tag keeps the
  casing the author first typed (TN-004, TN-005).
- Omitting the tags field in an API request is equivalent to submitting an
  empty collection (FR-014, API Contract Scope).
- The frontend `PostEditorDraft` tags field remains a single comma-separated
  string for input; conversion to a collection happens at submit time.
- The existing database may already have partial tag-related schema; database
  scripts are updated only to the extent needed to support this feature.
- No upper limit on tag count or tag length is enforced unless an existing
  database constraint already imposes one.
