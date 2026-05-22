# Category Pagination API Contract

## Route Contract

Reuse the existing category routes:

- `GET /api/categories`
- `GET /api/categories/available`
- existing create, update, and deactivate category routes

## List Request Behavior

- Supported query parameters:
  - `page` optional page number
  - `pageSize` optional page size
- `GET /api/categories` remains administrator-only.
- `GET /api/categories/available` remains available for the current post-form
  frontend flow.
- Invalid pagination inputs use the existing ProblemDetails-style validation
  behavior.

## Paginated List Response Contract

Category list routes return a paginated envelope:

```json
{
  "items": [
    {
      "id": 1,
      "title": "Architecture",
      "description": "Posts about software architecture.",
      "isAvailable": true
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1,
  "hasNextPage": false
}
```

## Write Request Contract

- Category create and update requests include:
  - `title`
  - `description` optional
- Title remains required.
- Description may be omitted or set to an empty value to clear it.

## Write Response Contract

Successful category create, update, and deactivate responses include:

```json
{
  "id": 1,
  "title": "Architecture",
  "description": "Posts about software architecture.",
  "isAvailable": true
}
```

## Authorization Contract

- Only administrator users may create, update, or deactivate categories.
- Non-admin and unauthenticated users must continue to receive the existing
  authorization failures for category writes.
- Public available-category reads do not enable category management operations.

## Frontend Consumption Contract

- The administrator category page consumes paginated admin category responses
  and submits the optional description field.
- Post-form category consumers adapt to the paginated available-category
  response shape without assuming the old array-only contract.
