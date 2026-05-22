# Quickstart: Add Tags Support to Blog Posts

This feature extends the existing post create, edit, and read flows with tags.
There are no new endpoints, tables, or services.

## Prerequisites

- The stack runs via Docker Compose (database, API, frontend).
- The `tags` and `post_tags` tables already exist (`database/scripts/006`,
  `007`) — no migration is needed.

## Implementation order (backend-first, test-first)

1. **Application — `TagNormalizer`**: write failing unit tests for trim,
   empty removal, case-insensitive de-duplication, casing retention, order, and
   all-empty input; then implement the helper.
2. **Domain — `BlogPost.Tags`**: add the `Tags` collection and thread it
   through `Create`, `Rehydrate`, and `Update`.
3. **Application — commands/handlers/results**: add `Tags` to the create/edit
   commands, normalize in the handlers, and surface `Tags` on results and list
   items. Write handler tests first.
4. **Infrastructure — `PostgreSqlPostRepository`**: write PostgreSQL-backed
   repository tests, then implement transactional tag writes and aggregated
   tag reads in raw SQL.
5. **API — DTOs/controllers**: add `Tags` to request/response DTOs and map it
   in the controllers; write API integration tests first.
6. **Frontend**: add `tags` to the post types, add `parseTags`, wire create/
   edit submit and edit prefill, and render tag chips on the post card and
   detail page.

## Manual verification

1. `docker compose up` and open the frontend.
2. Create a post with `rust, cloud-native, rust,  ` in the Tags field; confirm
   the saved post shows `rust` and `cloud-native` only.
3. Edit that post, change the tags, save, and confirm the new set replaces the
   old one; clear the field and confirm the post ends up with no tags.
4. Open the public post list and a public post detail; confirm tags display
   for tagged posts and nothing extra renders for untagged posts.
5. Open Swagger and confirm `tags` appears on the post request/response
   schemas.

## Validation commands

```bash
# Backend tests
dotnet test

# Frontend checks
cd src/frontend/blog-web && npm run lint && npm run build && npm test
```

## Done when

- All new tag tests pass and existing post tests still pass.
- Tags survive a create → edit → read round trip through the running stack.
- Visibility, ownership, and authorization behavior is unchanged.
