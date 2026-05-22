# Blog Web Frontend

## Purpose

This app is the React + Vite frontend for the blog platform technical challenge.
The current slice provides the frontend MVP on top of the existing ASP.NET Core
API: public post browsing and reactions, user registration and login, author
post management, and administrator category management.

## Run Locally

From this directory:

```bash
npm install
npm run dev
```

Open the local URL printed by Vite, typically `http://localhost:5173`.

To run the full stack through Docker Compose from the repository root:

```bash
docker compose up -d postgres api frontend
```

## Validation Commands

```bash
npm run lint
npm run build
```

## Main Routes

- `/` public post listing
- `/posts/:postId` public post detail
- `/register` account registration
- `/login` account login
- `/my-posts` authenticated author workspace
- `/my-posts/new` create post
- `/my-posts/:postId/edit` edit owned post
- `/admin/categories` administrator category management

## Search Behavior

- The header search field keeps using the `?q=` route query string.
- Search results now come from the backend instead of client-side filtering.
- Logged-in users can receive their own matching private posts in search
  results; anonymous users only receive public and available matches.

## Styling Notes

- TailwindCSS with the Vite integration plugin is the styling foundation.
- Shared design tokens are centralized in [tailwind.config.ts](./tailwind.config.ts).
- Global base styles are defined in [src/index.css](./src/index.css).
- Reusable primitives are defined in [src/styles/components.css](./src/styles/components.css).
- Visual direction comes from the repository-level `DESIGN.md`.

## Demo Credentials

- Administrator: `admin@blogplatform.local` / `Admin123!`
- Regular user: `user@blogplatform.local` / `User123!`

## Known Limitations

- Post editing does not change category because the current backend update
  contract only updates title, summary, and content.
- Tags are not yet exposed in the frontend because a frontend tag workflow is
  out of scope for this MVP.
