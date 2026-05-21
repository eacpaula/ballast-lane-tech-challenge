# Data Model: Public Visitor Reacts To Posts

## Overview

The first increment needs only the Domain and Application data required to
validate a reaction request, confirm the target post is publicly readable, and
persist one accepted like or dislike reaction. Persistence-specific details and
HTTP-specific contracts stay out of this model.

## Entities

### BlogPost

- **Purpose**: Represents the target post for a public reaction and determines
  whether the post is eligible for public interaction.
- **Core fields reused in this slice**:
  - `Id`
  - `Title`
  - `IsPublic`
  - `IsAvailable`
  - `IsPubliclyReadable`
- **Validation rules**:
  - A reaction may be accepted only when `IsPubliclyReadable` is true.

### PostReaction

- **Purpose**: Represents one accepted reaction submitted for a blog post.
- **Core fields**:
  - `PostId`
  - `ReactionType`
  - `UserId`
  - `VisitorIdentifier`
- **Relationships**:
  - One reaction belongs to one post.
  - One reaction is associated with exactly one acting identity source in this
    slice: either `UserId` or `VisitorIdentifier`.
- **Validation rules**:
  - `PostId` must refer to an existing target post provided by Application.
  - `ReactionType` must be a valid supported value.
  - At least one valid actor identity must be present.

## Domain Value Concepts

### ReactionType

- **Purpose**: Restricts reaction input to the supported values for this slice.
- **Allowed values**:
  - `Like`
  - `Dislike`
- **Validation rules**:
  - Any other value is invalid and must be rejected before persistence.

### ReactionActor

- **Purpose**: Represents the actor identity associated with a reaction without
  coupling to HTTP or authentication middleware.
- **Fields**:
  - `UserId`
  - `VisitorIdentifier`
- **Validation rules**:
  - At least one valid identity must be present.
  - If both are supplied, authenticated `UserId` is treated as the primary
    identity for this slice.

## Application Boundary Models

### ReactToPostCommand

- **Purpose**: Represents the input required to submit a public post reaction.
- **Fields**:
  - `PostId`
  - `ReactionType`
  - `AuthenticatedUserId`
  - `VisitorIdentifier`

### ReactToPostResult

- **Purpose**: Represents either a successful accepted reaction or a business
  failure outcome without leaking persistence details.
- **Fields**:
  - `IsSuccess`
  - `PostId`
  - `ReactionType`
  - `UserId`
  - `VisitorIdentifier`
  - `ErrorCode`
  - `ErrorMessage`

## Repository Abstractions

### IPostRepository

- **Purpose**: Loads the target post so Application can enforce public
  eligibility.
- **Minimum behavior reused**:
  - Load a post by id for public interaction checks

### IPostReactionRepository

- **Purpose**: Persists one accepted reaction record.
- **Minimum behavior needed**:
  - Create a reaction after Domain and Application validation succeeds

## Invariants to Prove with Tests

- A like reaction is accepted for a public, available post.
- A dislike reaction is accepted for a public, available post.
- Missing, non-public, or unavailable posts are rejected for reaction
  submission.
- Invalid reaction values are rejected.
- Invalid actor identity input is rejected.
- Accepted reactions are associated with either a user identifier or a visitor
  identifier.
- Accepted reactions are persisted only through repository abstractions.
