# Data Model: Public Visitor Reads Posts

## Overview

The first increment needs only the Domain and Application data required to list
public, available posts and read the details of one public, available post.
Persistence-specific details and HTTP-specific contracts stay out of this
model.

## Entities

### BlogPost

- **Purpose**: Represents a blog post that may be visible in public read flows
  only when it is public and available.
- **Core fields**:
  - `Id`: existing persisted identifier
  - `AuthorUserId`: identifies the post owner; not the focus of this slice
  - `CategoryId`: existing category reference
  - `Title`: post title shown to public visitors
  - `Summary`: optional preview content
  - `Content`: full post body for detail reads
  - `IsPublic`: indicates whether the post is visible to the public
  - `IsAvailable`: indicates whether the post is available for public reading
- **Relationships**:
  - One post belongs to one user
- **Validation rules**:
  - Public listing includes only posts where `IsPublic` and `IsAvailable` are
    both true
  - Public detail reads succeed only when `IsPublic` and `IsAvailable` are both
    true
- **State transitions**:
  - `Persisted post` -> `Publicly readable post`
  - `Persisted post` -> `Not available to public`

## Application Boundary Models

### PublicPostListItem

- **Purpose**: Represents one post entry returned in the public listing.
- **Fields**:
  - `PostId`
  - `Title`
  - `Summary`

### GetPublicPostByIdResult

- **Purpose**: Returns either the public post detail outcome or a public
  not-available outcome without leaking persistence details.
- **Fields**:
  - `PostId`
  - `Title`
  - `Summary`
  - `Content`
  - `ErrorCode`
  - `ErrorMessage`

## Repository Abstractions

### IPostRepository

- **Purpose**: Supports public listing and public detail reads.
- **Minimum behavior needed**:
  - Read public, available posts using the simple default ordering
  - Read one public, available post by id

## Invariants to Prove with Tests

- Public listing returns only posts that are public and available.
- Non-public posts do not appear in the public listing.
- Unavailable posts do not appear in the public listing.
- Public detail reads succeed only for a public, available post.
- Missing, non-public, or unavailable detail reads return a not-available
  public outcome.
- Public read use cases do not require authentication.
