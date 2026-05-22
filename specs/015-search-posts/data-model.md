# Data Model: Search Posts Through Backend API

## PostSearchRequest

- **Purpose**: Represents a post-list or post-search request sent through the
  existing listing flow.
- **Fields**:
  - `query`
  - `requestingUserId` (optional)
- **Rules**:
  - `query` is trimmed before use.
  - empty or whitespace-only `query` means default listing behavior.
  - `requestingUserId` is present only when a valid authenticated context is
    available.

## SearchablePost

- **Purpose**: Repository-level read model used to evaluate whether a post
  matches search criteria and visibility rules.
- **Fields**:
  - `postId`
  - `authorUserId`
  - `categoryId`
  - `title`
  - `summary`
  - `content`
  - `isPublic`
  - `isAvailable`
  - `categoryTitle` (matching only, optional in result shaping)
  - `tagTitles` (matching only, optional in result shaping)
- **Rules**:
  - must map back into the existing `BlogPost` or list-item model without
    leaking persistence concerns to Application
  - may be matched on category or tags without exposing those fields in the
    existing list response

## PostSearchResultItem

- **Purpose**: Application/API list item returned to the frontend search view.
- **Fields**:
  - `postId`
  - `title`
  - `summary`
- **Rules**:
  - keeps the existing public listing payload stable
  - contains only posts visible to the requester under the feature rules

## SearchVisibilityRule

- **Purpose**: Encodes the allowed result-set scope for the requester.
- **Fields**:
  - `isAnonymous`
  - `requestingUserId` (optional)
- **Rules**:
  - anonymous users may see only public and available posts
  - authenticated users may see public and available posts plus their own
    matching non-public available posts
  - authenticated users may not see private or non-public posts owned by other
    users

## SearchPageState

- **Purpose**: Frontend page-state model for the search-enabled public listing.
- **Fields**:
  - `query`
  - `status` (`loading`, `success`, `empty`, `error`)
  - `results`
  - `message` (optional)
- **Rules**:
  - blank query uses the same screen structure as the default listing
  - non-empty query with zero results shows the no-results state
  - existing search field remains the route-driving control
