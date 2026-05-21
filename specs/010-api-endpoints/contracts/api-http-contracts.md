# Contract: API HTTP Surface

## Purpose

This contract defines the HTTP surface that the API layer must expose for the
existing backend use cases. It is intentionally high-level and focused on route
shape, access level, and expected response behavior.

## Auth Endpoints

### `POST /api/auth/register`

- **Access**: Public
- **Input**: Registration request DTO
- **Success**: Returns created user data
- **Failure coverage**:
  - validation failure
  - duplicate email conflict

### `POST /api/auth/login`

- **Access**: Public
- **Input**: Login request DTO
- **Success**: Returns user data plus authentication payload
- **Failure coverage**:
  - validation failure
  - invalid credentials

## Public Post Endpoints

### `GET /api/posts`

- **Access**: Public
- **Success**: Returns public post list DTOs only

### `GET /api/posts/{postId}`

- **Access**: Public
- **Success**: Returns the public post detail DTO
- **Failure coverage**:
  - missing post
  - unavailable or non-public post

## Public Reaction Endpoint

### `POST /api/posts/{postId}/reactions`

- **Access**: Public
- **Input**: Reaction request DTO
- **Success**: Returns stored reaction outcome
- **Failure coverage**:
  - invalid reaction type
  - invalid actor identity
  - unavailable or non-public post

## Protected Post Endpoints

### `POST /api/posts`

- **Access**: Authenticated user
- **Input**: Create post request DTO
- **Success**: Returns created post DTO
- **Failure coverage**:
  - unauthenticated request
  - validation failure
  - unavailable category

### `PUT /api/posts/{postId}`

- **Access**: Authenticated user
- **Input**: Update post request DTO
- **Success**: Returns updated post DTO
- **Failure coverage**:
  - unauthenticated request
  - post not found
  - non-owner forbidden outcome
  - validation failure

### `DELETE /api/posts/{postId}`

- **Access**: Authenticated user
- **Success**: Returns successful removal outcome or no-content style success
- **Failure coverage**:
  - unauthenticated request
  - post not found
  - non-owner forbidden outcome

## Administrator Category Endpoints

### `POST /api/categories`

- **Access**: Administrator only
- **Input**: Create category request DTO
- **Success**: Returns created category DTO
- **Failure coverage**:
  - unauthenticated request
  - non-admin forbidden outcome
  - validation failure
  - duplicate title conflict

### `PUT /api/categories/{categoryId}`

- **Access**: Administrator only
- **Input**: Update category request DTO
- **Success**: Returns updated category DTO
- **Failure coverage**:
  - unauthenticated request
  - non-admin forbidden outcome
  - category not found
  - validation failure
  - duplicate title conflict

### `DELETE /api/categories/{categoryId}`

- **Access**: Administrator only
- **Success**: Returns deactivated category outcome or no-content style success
- **Failure coverage**:
  - unauthenticated request
  - non-admin forbidden outcome
  - category not found

## Error Contract Requirements

- Failures must use ProblemDetails-style responses.
- Validation, conflict, unauthorized, forbidden, and not-found outcomes must
  remain consistent across controllers.
- Unexpected failures must not expose SQL, token, or connection internals.
