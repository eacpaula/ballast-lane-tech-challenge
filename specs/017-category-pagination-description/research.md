# Research: Paginate Categories and Support Category Descriptions

## Decision 1: Paginate both current category read endpoints

- **Decision**: Apply pagination to both `GET /api/categories` and
  `GET /api/categories/available`.
- **Rationale**: The feature goal is category listing pagination, and the
  project currently has two active category list contracts. Updating both keeps
  the category read model consistent and avoids one paginated contract plus one
  array-based exception.
- **Alternatives considered**:
  - Paginate only the administrator endpoint: simpler in the short term, but
    leaves the current frontend category consumer on a different contract.
  - Add a new paginated route and keep the old routes: expands contract surface
    unnecessarily.

## Decision 2: Reuse the existing pagination pattern, not the post-specific DTOs

- **Decision**: Reuse the project’s current page/pageSize plus metadata envelope
  shape, but define category-specific request and response models.
- **Rationale**: The post pagination work already established the project
  contract pattern. Reusing the shape keeps the API consistent, while reusing
  post-specific types would leak unrelated semantics into category flows.
- **Alternatives considered**:
  - Reuse `PaginatedPublicPostResponse`: too post-specific.
  - Create a generic shared pagination framework: more abstraction than the
    feature needs.

## Decision 3: Treat description as an existing data field to wire through the stack

- **Decision**: Use the already-present `post_categories.description` column and
  existing seed descriptions rather than introduce a new schema change.
- **Rationale**: The database scripts already support descriptions, so the
  actual gap is missing Domain/Application/API/repository mapping rather than
  schema design.
- **Alternatives considered**:
  - Add a new migration or table change: unnecessary because the column already
    exists.

## Decision 4: Keep description validation intentionally simple

- **Decision**: Description is optional, trimmed, and clearable; title remains
  the only required category field.
- **Rationale**: The feature is about enabling useful context, not adding a new
  validation-heavy content model.
- **Alternatives considered**:
  - Add description length or formatting rules beyond basic normalization: not
    required by the feature scope.

## Decision 5: Keep administrator writes unchanged apart from description support

- **Decision**: Create, update, and deactivate category authorization behavior
  remains exactly administrator-only, with description added only as extra
  category data.
- **Rationale**: The feature is not a role or permission redesign; it only
  expands category payloads and read behavior.
- **Alternatives considered**:
  - Loosen category write access for authors: out of scope and conflicts with
    existing rules.

## Decision 6: Keep frontend adjustments narrow and reuse the current admin screen

- **Decision**: Enable the already-present description field in the category
  form and update current category list consumers to the paginated contract.
- **Rationale**: The frontend already contains the intended form affordance, so
  the smallest change is to wire it up rather than redesign the page.
- **Alternatives considered**:
  - Introduce a new category detail view or search UI: outside the approved
    scope.
