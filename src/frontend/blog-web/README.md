# Blog Web Frontend

## Purpose

This app is the React + Vite frontend for the blog platform technical challenge.
The current slice establishes the TailwindCSS styling foundation that future UI
features will build on.

## Run Locally

From this directory:

```bash
npm install
npm run dev
```

Open the local URL printed by Vite, typically `http://localhost:5173`.

## Validation Commands

```bash
npm run lint
npm run build
```

## Tailwind Foundation Notes

- The frontend uses TailwindCSS with the Vite integration plugin.
- Shared design tokens are centralized in
  [tailwind.config.ts](./tailwind.config.ts).
- Global base styles are defined in [src/index.css](./src/index.css).
- Reusable shell primitives for the first UI layer are defined in
  [src/styles/components.css](./src/styles/components.css).
- Visual direction comes from the repository-level `DESIGN.md`.

## Current Scope

- Tailwind configuration
- Shared design tokens
- Accessible base styles
- Minimal validation shell

The app intentionally does not include full pages, backend API integration,
authentication UI, or admin workflows yet.
