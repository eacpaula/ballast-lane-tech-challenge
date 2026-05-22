# Implementation Plan: Paginate Categories and Support Category Descriptions

**Branch**: `main` | **Date**: 2026-05-22 | **Spec**:
`specs/017-category-pagination-description/spec.md`

**Input**: Feature specification from
`/specs/017-category-pagination-description/spec.md`

**Note**: This plan updates the existing category flow with pagination and
optional descriptions while keeping authorization, validation, and data access
patterns aligned with the current project.

## Summary

Add paginated category listing and optional category descriptions to the
existing category management flow, then update the current frontend category
screen and category consumers to use the revised API contracts without a UI
redesign.

- Reuse the project’s current page/pageSize pagination style and metadata
  envelope pattern for category list responses.
- Keep category validation in Application and raw SQL in Infrastructure.
- Wire the already-existing `post_categories.description` column through
  Domain, Application, Infrastructure, API, and frontend layers.
- Preserve administrator-only category writes and the existing public available
  category read behavior.
- Keep frontend changes limited to contract updates, description field support,
  and minimal paginated list handling.

## Implementation Design

**1. Technical approach**:

- Extend the existing category list endpoints rather than adding parallel
  routes.
- Apply pagination to:
  - `GET /api/categories` for administrator category management
  - `GET /api/categories/available` for current post-form category selection
- Reuse the existing page/pageSize behavior pattern already used by post
  listing:
  - default `page = 1`
  - small bounded `pageSize`
- Reuse the existing pagination envelope shape concept, but keep category DTOs
  category-specific rather than reusing post-specific response types.
- Treat category description as optional data:
  - persisted when provided
  - clearable when set to empty or whitespace

**2. Layers affected**:

- Domain:
  - `PostCategory` description support
  - description-preserving update behavior
- Application:
  - paginated category list request/result models
  - paginated admin and available category handlers
  - create/update commands extended with optional description
  - existing validation and duplicate-title checks retained
- Infrastructure:
  - category repository contract updates
  - raw SQL list pagination and count queries
  - description read/write mapping
- API:
  - category list query parameters
  - paginated category response DTOs
  - category create/update request and response DTOs with description
- Frontend:
  - admin category API contract changes
  - available category list consumption updates
  - enable description editing in the existing category form

**3. Database/schema impact**:

- The schema already includes `post_categories.description`, so no new table or
  migration framework is needed.
- Existing seed data already contains category descriptions.
- Implementation should verify all initialization scripts stay consistent with
  the current repository state.
- The main database impact is mapping and querying the existing column rather
  than altering schema shape.

**4. Application use case/query strategy**:

- Introduce category pagination request and result models that mirror the
  project’s current paginated response pattern.
- Update `ListAllPostCategoriesHandler` to:
  - require administrator context as it does today
  - accept `page` and `pageSize`
  - return paginated category data with metadata
- Update `ListAvailablePostCategoriesHandler` to:
  - accept `page` and `pageSize`
  - return only available categories in paginated form
- Extend create/update commands and results to carry optional description.
- Keep business validation in Application:
  - required title
  - normalized optional description
  - duplicate-title rejection
  - administrator-only write behavior

**5. Repository/raw SQL strategy**:

- Extend `ICategoryRepository` with paginated category read methods instead of
  returning only full arrays.
- Keep list ordering deterministic with one explicit sort.
- Add parameterized raw SQL for:
  - paginated available category reads
  - paginated all-category reads
  - total count retrieval for each list path
- Update create, update, get-by-id, and deactivate queries to include
  description mapping where applicable.
- Keep the repository responsible only for query execution and mapping, not
  authorization decisions.

**6. API endpoint/DTO strategy**:

- Preserve the existing routes:
  - `GET /api/categories`
  - `GET /api/categories/available`
  - existing create/update/deactivate routes
- Add `page` and `pageSize` query parameters to list endpoints.
- Return category-specific paginated response envelopes that match the existing
  pagination shape:
  - items
  - page
  - pageSize
  - totalCount
  - totalPages
  - hasNextPage
- Extend category request and response DTOs with optional description.
- Keep ProblemDetails-style failures and OpenAPI derived from the final DTOs.

**7. Frontend integration strategy**:

- Update the administrator category API helper to consume paginated list data
  and submit description on create/update.
- Update the admin category management page to:
  - render the existing description field as active instead of placeholder-only
  - consume paginated list responses
  - keep current table and form layout
- Update available-category consumers in post forms to handle the paginated
  response shape.
- Keep frontend adjustments narrow:
  - no category redesign
  - no new search/filter UI
  - no infinite scrolling requirement for categories

**8. Testing strategy**:

- Application tests first:
  - paginated available category listing
  - paginated admin category listing
  - required title validation
  - optional description create/update behavior
  - duplicate-title rejection remains intact
- Repository tests:
  - available category page slicing and counts
  - admin category page slicing and counts
  - description persistence and mapping
- API integration tests:
  - paginated `GET /api/categories`
  - paginated `GET /api/categories/available`
  - create/update DTOs with description
  - non-admin write rejection remains unchanged
- Frontend validation:
  - keep automated checks light
  - use lint/build plus full-stack manual verification for category management
    and post-form category loading

**9. Validation strategy**:

- Run Application, Infrastructure, and API test projects after backend changes.
- Run frontend lint/build after the contract updates.
- Validate through Docker Compose with PostgreSQL, API, and frontend running:
  - admin category list pagination
  - create/update category with description
  - post-form category loading against the paginated available-category API
- Confirm Swagger exposes category pagination parameters and updated DTOs.

**10. Documentation update strategy**:

- Update README only if the category API contract or demo flow needs explicit
  pagination/description notes.
- Update frontend README if category-management behavior or available-category
  consumption changes need explanation.
- Update feature quickstart with:
  - paginated category API checks
  - admin description create/update checks
  - post-form category-load verification

**11. Risks and trade-offs**:

- Changing category list responses from arrays to paginated envelopes is a real
  contract change and must be updated consistently across admin and post-form
  consumers.
- Reusing the pagination pattern rather than a generic shared pagination model
  keeps scope small, but may duplicate some response-shape code; that is
  acceptable at this project scale.
- Paginating `available` categories may be more than the current post form
  strictly needs, but it keeps category list behavior consistent across the
  feature and avoids maintaining two read contracts.
- Description support stays intentionally simple and informational; no search,
  filtering, or display expansion is added in this slice.

## Technical Context

**Language/Version**: .NET 10 LTS (backend), TypeScript (frontend)

**Primary Dependencies**: ASP.NET Core Web API, Npgsql, xUnit, React, Vite,
TailwindCSS

**Storage**: PostgreSQL

**Testing**: xUnit unit tests, API integration tests, PostgreSQL-backed
repository tests where practical, frontend lint/build and manual full-stack
validation

**Target Platform**: Web application with ASP.NET Core backend and browser-based
frontend

**Project Type**: Full-stack web application

**Performance Goals**: Keep category list responses suitable for interview-scale
demo usage while aligning with the project’s existing pagination behavior

**Constraints**: Raw SQL with Npgsql only; ProblemDetails-style errors;
administrator-only category writes; explicit DTOs; no Redis for categories; no
Entity Framework, Dapper, MediatR, or unnecessary abstractions

**Scale/Scope**: Small category-flow enhancement for the current blog platform

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: backend category pagination and description behavior will
  begin with failing Application, repository, and API tests before production
  code.
- PASS Backend-first: category list contracts, validation, repository mapping,
  and API DTO updates are planned ahead of the dependent frontend changes.
- PASS Architecture: controllers remain thin, Application owns validation and
  authorization decisions, Infrastructure owns raw SQL and mapping, and the
  frontend consumes the API contract only.
- PASS Data access: all persistence work remains parameterized raw SQL in the
  category repository.
- PASS Security: administrator-only category writes remain enforced and covered
  by tests; public available-category reads remain read-only.
- PASS Scope: the plan stays within pagination plus optional descriptions and
  avoids caching, search, hierarchy, or broader category redesign.
- PASS API consistency: ProblemDetails-style failures, explicit DTOs, and
  OpenAPI alignment are preserved while evolving the category contract.
- PASS Frontend governance: frontend changes reuse the existing category screen,
  existing shared form patterns, and current `DESIGN.md`-aligned styling.

## Project Structure

### Documentation (this feature)

```text
specs/017-category-pagination-description/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── backend/
│   ├── BlogPlatform.Api/
│   ├── BlogPlatform.Application/
│   ├── BlogPlatform.Domain/
│   └── BlogPlatform.Infrastructure/
└── frontend/
    └── blog-web/
        └── src/
            ├── app/
            ├── components/
            ├── features/
            ├── lib/
            └── styles/

tests/
├── backend/
│   ├── BlogPlatform.Api.Tests/
│   ├── BlogPlatform.Application.Tests/
│   ├── BlogPlatform.Domain.Tests/
│   └── BlogPlatform.Infrastructure.Tests/
```

**Structure Decision**: Reuse the current layered backend and feature-based
frontend structure. Keep category pagination and description updates localized
to the existing category flow and related post-form consumers.

## Complexity Tracking

No constitution violations are required for this plan.
