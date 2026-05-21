# Data Model: Authenticated User Creates a Blog Post

## Overview

The first increment needs only the data required to validate and create a post
inside the Domain and Application layers. Persistence-specific fields and HTTP
shapes stay out of this model unless they are necessary to express business
rules.

## Entities

### BlogPost

- **Purpose**: Represents a post created by an authenticated user.
- **Core fields**:
  - `Id`: assigned after persistence saves the post
  - `AuthorUserId`: identifies the user who owns the post
  - `CategoryId`: identifies the category selected for the post
  - `Title`: required, non-blank
  - `Summary`: included in the feature input; treated as optional descriptive
    text in the first increment
  - `Content`: required, non-blank
- **Relationships**:
  - Many posts can belong to one user
  - Many posts can belong to one category
- **Validation rules**:
  - `AuthorUserId` must be present
  - `CategoryId` must be present
  - `Title` must contain non-whitespace content
  - `Content` must contain non-whitespace content
- **State transitions**:
  - `Draft input` -> `Validated post` -> `Persisted post`

### CategoryReference

- **Purpose**: Represents the minimal category information needed by the
  create-post use case.
- **Core fields**:
  - `CategoryId`
  - `IsAvailableForPosts`
- **Relationships**:
  - One category can be referenced by many posts
- **Validation rules**:
  - The category must exist
  - The category must be allowed for post assignment

## Application Boundary Models

### CreateBlogPostCommand

- **Purpose**: Carries the use case input from an outer layer into the
  Application layer.
- **Fields**:
  - `AuthenticatedUserId`
  - `CategoryId`
  - `Title`
  - `Summary`
  - `Content`

### CreateBlogPostResult

- **Purpose**: Returns the outcome of successful post creation without exposing
  persistence implementation details.
- **Fields**:
  - `PostId`
  - `AuthorUserId`
  - `CategoryId`
  - `Title`
  - `Summary`
  - `Content`

## Repository Abstractions

### ICategoryRepository

- **Purpose**: Allows Application to verify that the selected category exists
  and can be used for posts.
- **Minimum behavior needed**:
  - Read category availability by id

### IPostRepository

- **Purpose**: Allows Application to persist a validated `BlogPost`.
- **Minimum behavior needed**:
  - Save a post and return the persisted post or assigned id

## Invariants to Prove with Tests

- A post cannot be created with a blank title.
- A post cannot be created with blank content.
- A post cannot be created for a missing or unavailable category.
- A successfully created post keeps the authenticated user as owner.
- Application returns a success result shaped from the saved post.
