# Data Model: Add Paginated Post Search With Redis Caching

## PostListPageRequest

- **Purpose**: Represents one request for a paginated post list or paginated
  search result.
- **Fields**:
  - `query`
  - `page`
  - `pageSize`
  - `requestingUserId` (optional)
- **Rules**:
  - `query` is normalized before execution and before cache-key construction
  - `page` must be a positive number
  - `pageSize` must be a positive number within the chosen maximum bound
  - empty or whitespace-only `query` represents default listing behavior

## PaginatedPostResult

- **Purpose**: Application and API representation of one page of post results.
- **Fields**:
  - `items`
  - `page`
  - `pageSize`
  - `totalCount`
  - `totalPages`
  - `hasNextPage`
- **Rules**:
  - `items` preserve the existing public post summary payload shape
  - page metadata must remain self-consistent even when the page is empty
  - empty page results are valid when the requested page is beyond the last page

## ViewerContext

- **Purpose**: Distinguishes who is allowed to see which list or search results.
- **Fields**:
  - `isAnonymous`
  - `requestingUserId` (optional)
- **Rules**:
  - anonymous viewers may see only public and available posts
  - authenticated viewers may also see their own matching private or non-public
    available posts
  - viewer context must be included in the cache-key strategy

## PostSearchCacheEntry

- **Purpose**: Cached representation of one paginated result page.
- **Fields**:
  - `cacheKey`
  - `payload`
  - `expiresInSeconds`
- **Rules**:
  - payload must correspond to exactly one query, page, page size, and viewer
    context combination
  - TTL is fixed at 30 seconds
  - entries are read-only and not used for writes or mutation workflows

## PostSearchCacheKey

- **Purpose**: Prevents cached page reuse across incompatible search contexts.
- **Fields**:
  - `queryMarker`
  - `page`
  - `pageSize`
  - `viewerMarker`
- **Rules**:
  - `queryMarker` must distinguish empty search from non-empty terms
  - `viewerMarker` must distinguish anonymous from each authenticated user
  - two different users must never share the same private-inclusive cache entry

## FrontendFeedPageState

- **Purpose**: Represents the browser-side state of the incremental public feed.
- **Fields**:
  - `query`
  - `pagesLoaded`
  - `items`
  - `isInitialLoading`
  - `isLoadingNextPage`
  - `hasNextPage`
  - `error`
- **Rules**:
  - new search terms reset the accumulated pages
  - next-page loading appends items rather than replacing the feed
  - end-of-list state appears when `hasNextPage` becomes false
