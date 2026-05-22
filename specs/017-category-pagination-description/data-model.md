# Data Model: Paginate Categories and Support Category Descriptions

## PostCategory

- **Purpose**: Represents a blog post category that can be assigned to posts
  and managed by administrators.
- **Fields**:
  - `id`
  - `title`
  - `description` (optional)
  - `isAvailable`
- **Rules**:
  - `title` is required and normalized
  - `description` is optional and normalized when present
  - duplicate titles are rejected externally through Application validation
  - categories can be active or deactivated without deleting history

## CategoryPageRequest

- **Purpose**: Represents one paginated category list request.
- **Fields**:
  - `page`
  - `pageSize`
  - `isAdministrator` or endpoint access context, depending on handler path
- **Rules**:
  - `page` must be a positive number
  - `pageSize` must be a positive number within the chosen maximum bound
  - available-category and admin-category handlers apply different visibility
    scopes but use the same pagination semantics

## PaginatedCategoryResult

- **Purpose**: Represents one page of category results plus pagination
  metadata.
- **Fields**:
  - `items`
  - `page`
  - `pageSize`
  - `totalCount`
  - `totalPages`
  - `hasNextPage`
- **Rules**:
  - empty page results are valid when the request is beyond the last page
  - metadata must stay self-consistent for empty and non-empty pages
  - item payloads differ slightly between admin and available-category flows,
    but share the same envelope shape

## CreateOrUpdateCategoryInput

- **Purpose**: Represents the normalized category payload used by create and
  update flows.
- **Fields**:
  - `title`
  - `description` (optional)
- **Rules**:
  - title is required
  - description may be absent, present, or cleared
  - title uniqueness is evaluated independently of description

## FrontendManagedCategoryState

- **Purpose**: Represents the administrator category management view state.
- **Fields**:
  - `items`
  - `page`
  - `pageSize`
  - `hasNextPage`
  - `editingCategory`
  - `isSubmitting`
  - `error`
- **Rules**:
  - the existing admin form can edit both title and optional description
  - paginated category data replaces the current full-array assumption

## FrontendAvailableCategoryState

- **Purpose**: Represents the category data consumed by post forms.
- **Fields**:
  - `items`
  - `page`
  - `pageSize`
  - `hasNextPage`
- **Rules**:
  - available categories remain read-only in this feature
  - post-form consumers adapt to the paginated response shape without requiring
    a broader workflow redesign
