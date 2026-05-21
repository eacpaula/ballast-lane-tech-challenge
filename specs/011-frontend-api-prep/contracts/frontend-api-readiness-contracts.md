# Contract: Frontend API Readiness

## Purpose

Define the backend-facing contracts required to make the API easier for the
frontend to consume locally without expanding business scope.

## Local Runtime Contract

### API service in Docker Compose

- **Access**: Local development only
- **Behavior**:
  - API starts as a Docker Compose service
  - API can connect to PostgreSQL through the Compose network
  - API exposes its HTTP port for local browser and tool access
  - Swagger remains reachable through the local API URL

## Local CORS Contract

- **Access**: Local frontend origins only
- **Behavior**:
  - Browser requests from the configured local frontend origin are allowed
  - Unrelated origins are not implicitly allowed by this feature

## Category Read Endpoint

### `GET /api/categories/available`

- **Access**: Public
- **Purpose**: Return category options for post create and edit forms
- **Success**:
  - Returns a list of available categories only
  - Each item contains only category selection data needed by the frontend
- **Failure coverage**:
  - unexpected server failure still uses ProblemDetails-style behavior

## Category Write Boundary

- Existing category management operations remain unchanged:
  - `POST /api/categories`
  - `PUT /api/categories/{categoryId}`
  - `DELETE /api/categories/{categoryId}`
- **Access**: Administrator only
- **Contract rule**: The new read endpoint must not weaken or replace the
  authorization model for category write operations.

## Test Contract

- Application test proves available-category filtering behavior.
- Repository integration test proves raw SQL filtering for available
  categories.
- API integration tests prove:
  - anonymous category listing success
  - exclusion of unavailable categories
  - no regression of admin-only category management protection
- Manual verification proves:
  - Compose-based API startup
  - Swagger availability through the containerized API
