# Feature Specification: Search Posts Through Backend API

## 1. Feature Summary

Replace the current client-side-only post search behavior with a backend-powered
search flow so visitors and signed-in users can find posts from the same source
of truth used by the platform.

## 2. Goal

Provide a small, reliable search experience that keeps post visibility rules
intact, returns the same default listing when no search term is supplied, and
keeps the frontend aligned with the existing interface without introducing
cache or external search infrastructure.

## 3. Functional Requirements

- **FR-001**: The system MUST allow anonymous visitors to search public and
  available posts.
- **FR-002**: The system MUST allow authenticated users to search public and
  available posts.
- **FR-003**: The system MUST allow authenticated users to search their own
  posts, including non-public or private posts, when those posts are otherwise
  available to the owner in the current domain model.
- **FR-004**: The system MUST NOT return private or non-public posts owned by
  other users.
- **FR-005**: The system MUST treat an empty or whitespace-only search term as
  the same behavior as the current default post listing.
- **FR-006**: The system MUST perform case-insensitive matching where practical
  for searchable text fields.
- **FR-007**: The system MUST search post title, summary or description, and
  content.
- **FR-008**: The system MUST also search category title and tag values only if
  those relationships are already available in the current persistence model
  and repository scope without expanding the feature into unrelated data work.
- **FR-009**: The system MUST expose the backend-powered search behavior
  through the existing post-listing flow so the frontend can request filtered
  results without duplicating search logic in the browser.
- **FR-010**: The system MUST keep visibility, ownership, and authorization
  rules in backend business logic rather than in controllers or frontend code.
- **FR-011**: The system MUST return clear failure behavior for invalid request
  shapes while treating an empty search value as a valid non-error request.
- **FR-012**: The frontend MUST update the existing search input to call the
  backend search flow while preserving the current design direction,
  accessibility baseline, and responsive behavior.

## 4. Search Visibility Rules

- Anonymous visitors can only see posts that are both public and available.
- Authenticated users can see the same public and available posts as anonymous
  visitors.
- Authenticated users can also see their own matching posts when those posts
  are not public, as long as the current domain model still considers them
  available to their owner.
- Authenticated users cannot see private or non-public posts owned by other
  users.
- If a post is unavailable, it is excluded from search results unless an
  existing domain rule already permits that post to be shown in an owner-only
  management flow. This feature must follow the existing rule rather than
  redefine visibility.

## 5. Backend Scope

- Add or update one application-level post search use case that applies the
  visibility rules before returning results.
- Add or update the post repository abstraction only as needed to support
  backend-powered search.
- Implement repository-backed search behavior using the existing persistence
  approach and current post-related data model.
- Add or update the existing post-listing API behavior so the frontend can send
  a search term through query parameters or the current listing contract.
- Keep the feature small by avoiding new infrastructure concerns such as cache,
  ranking, pagination expansion, or external search engines.

## 6. Frontend Scope

- Replace the current client-side-only filtering behavior with calls to the
  backend search flow.
- Reuse the existing search input and public post listing experience rather than
  redesigning the page.
- Preserve loading, empty, and error states in a way that remains consistent
  with the current UI patterns and `DESIGN.md`.
- Keep the search interaction simple and suitable for the technical interview
  demo; no advanced filtering, suggestions, or large client-side state layer is
  added.

## 7. Test Scope

- Add or update application tests for search visibility, empty search handling,
  and owner-only private result inclusion.
- Add or update repository tests for case-insensitive search behavior and
  visibility-constrained result sets against the current database setup.
- Add or update API tests for anonymous and authenticated search behavior,
  including exclusion of private posts owned by other users.
- Add or update frontend tests where practical for search request triggering and
  search-result rendering, while relying on existing lint/build and manual flow
  validation for the final UI check.

## 8. Out of Scope

- Redis or any other caching layer
- External search infrastructure
- Full-text search engine behavior
- Advanced ranking or scoring
- Search suggestions or autocomplete
- Broader filtering or sorting redesign
- New pagination behavior unless already supported
- Frontend visual redesign
- Major domain or architecture refactoring

## 9. Acceptance Criteria

- A visitor can enter a search term and receive only matching public and
  available posts.
- A signed-in regular user can enter a search term and receive matching public
  posts plus matching owned private posts, but not private posts owned by
  someone else.
- A blank search returns the same result set and page behavior as the current
  default post listing.
- Search matching is case-insensitive for the supported text fields.
- The existing frontend search control uses backend results instead of filtering
  a previously loaded in-browser list.
- End-to-end validation shows the feature working through the existing local
  stack without adding cache or external search services.

## 10. Definition of Done

- The specification is approved for planning without open clarification markers.
- Backend search behavior is defined with clear visibility rules.
- Frontend impact is limited to wiring the existing search UI to backend
  results.
- Test scope covers application, repository, API, and practical frontend
  validation.
- The feature remains within the technical interview scope and does not expand
  into caching or external search infrastructure.
