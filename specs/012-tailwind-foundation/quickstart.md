# Quickstart: Tailwind Frontend Foundation

## Goal

Verify that the React + Vite frontend runs with the new Tailwind foundation and
that the minimal validation shell reflects the approved design direction from
`DESIGN.md`.

## Preconditions

- Use the existing frontend app in `src/frontend/blog-web`
- Node and npm are available locally

## Steps

1. Install frontend dependencies from `src/frontend/blog-web`.
2. Start the local development server.
3. Open the local frontend URL shown by the dev server.
4. Confirm the starter Vite screen has been replaced by a minimal styling proof
   shell.
5. Confirm the shell shows:
   - a centered content frame
   - clean headline and body typography
   - a surface/card example
   - a button-like action with visible focus treatment
   - a metadata/tag treatment
6. Resize to a mobile-width viewport and confirm spacing and layout still read
   cleanly.
7. Run the frontend build and confirm it succeeds.

## Expected Outcome

- TailwindCSS is active in the frontend app
- Shared tokens drive the first-pass visual system
- The frontend is ready for later UI slices without relying on starter CSS
