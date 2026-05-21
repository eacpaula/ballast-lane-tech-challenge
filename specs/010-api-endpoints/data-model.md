# Data Model: Expose Backend API Endpoints for Existing Application Use Cases

## Overview

This feature does not add new business entities. It introduces API-facing DTO
and configuration shapes that translate HTTP requests and responses to the
existing Application use cases.

## API Request Models

### RegisterUserRequest

- **Purpose**: Accept registration input for the auth API.
- **Fields**:
  - `nameOrUsername`
  - `email`
  - `password`
- **Maps to**: `RegisterUserCommand`

### LoginUserRequest

- **Purpose**: Accept login credentials for the auth API.
- **Fields**:
  - `email`
  - `password`
- **Maps to**: `LoginUserCommand`

### CreatePostRequest

- **Purpose**: Accept protected post creation input.
- **Fields**:
  - `categoryId`
  - `title`
  - `summary`
  - `content`
- **Maps to**: `CreateBlogPostCommand` plus authenticated user context

### UpdatePostRequest

- **Purpose**: Accept protected post edit input.
- **Fields**:
  - `title`
  - `summary`
  - `content`
- **Maps to**: `EditBlogPostCommand` plus route post id and authenticated user
    context

### ReactToPostRequest

- **Purpose**: Accept public post reaction input.
- **Fields**:
  - `reactionType`
  - optional `visitorIdentifier` when no authenticated user is present
- **Maps to**: `ReactToPostCommand` plus route post id and optional auth
    context

### CreateCategoryRequest

- **Purpose**: Accept admin category creation input.
- **Fields**:
  - `title`
- **Maps to**: `CreatePostCategoryCommand` plus authenticated user/admin
    context

### UpdateCategoryRequest

- **Purpose**: Accept admin category update input.
- **Fields**:
  - `title`
- **Maps to**: `UpdatePostCategoryCommand` plus route id and auth context

## API Response Models

### AuthResponse

- **Purpose**: Return successful registration or login data.
- **Fields**:
  - `userId`
  - `nameOrUsername`
  - `email`
  - optional `authenticationPayload`

### PublicPostSummaryResponse

- **Purpose**: Return list-item data for public posts.
- **Fields**:
  - `id`
  - `title`
  - `summary`
  - `categoryId`
  - `authorUserId`

### PublicPostDetailResponse

- **Purpose**: Return the public detail view for a single post.
- **Fields**:
  - `id`
  - `title`
  - `summary`
  - `content`
  - `categoryId`
  - `authorUserId`

### PostMutationResponse

- **Purpose**: Return successful create, edit, or remove outcomes for posts.
- **Fields**:
  - `id`
  - `title`
  - `summary`
  - `content`
  - `categoryId`
  - `authorUserId`

### ReactionResponse

- **Purpose**: Return successful reaction submission data.
- **Fields**:
  - `postId`
  - `reactionType`
  - optional `userId`
  - optional `visitorIdentifier`

### CategoryResponse

- **Purpose**: Return successful category management outcomes.
- **Fields**:
  - `id`
  - `title`
  - `isAvailable`

## Configuration Models

### JwtAuthenticationSettings

- **Purpose**: Represent API-owned JWT configuration values.
- **Fields**:
  - `issuer`
  - `audience`
  - `signingKey`
  - `expirationMinutes`

### ApiTestIdentitySeed

- **Purpose**: Represent the seeded identities required by API integration
  tests.
- **Fields**:
  - admin credentials
  - regular-user credentials
  - any test-only token or claim assumptions

## Error Contract Shape

### ProblemDetailsResponse

- **Purpose**: Represent consistent API error responses.
- **Fields**:
  - `type`
  - `title`
  - `status`
  - `detail`
  - optional validation or trace metadata as needed by the API

## Mapping Boundaries

- API request DTOs map into Application commands or direct handler parameters.
- API response DTOs map from Application success results only.
- Authentication and authorization claims map into primitive command input at
  the controller boundary.
- ProblemDetails mapping translates Application error codes into HTTP status
  codes and safe API-facing error details.
