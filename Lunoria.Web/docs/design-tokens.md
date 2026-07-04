# Design Tokens & Component Migration Plan

This document captures the plan for centralizing colors/design tokens while
migrating components from the old app (`Eldoria.Web`) into `Lunoria.Web`.
It is a planning reference — none of this has been implemented yet.

## Problem

`Eldoria.Web` (Tailwind v3) has no custom color palette. Components use raw
Tailwind utilities directly (`bg-gray-700`, `text-red-400`, `border-blue-400`,
`bg-stone-800`, etc.), chosen ad-hoc per component. The same intent (e.g.
"danger" or "success") ends up expressed with different shades in different
places, and there's no single place to see or change the palette.

`Lunoria.Web` (Tailwind v4) currently defines styling via a CSS-first
`@theme` block in `src/styles/index.css`, which today only holds a font
token (`--font-cinzel`). `components/ui` and `components/layout` are empty
scaffolding, and `ComponentShowCasePage.tsx` is a blank placeholder intended
to become a style guide page. This is a clean slate — the goal is to avoid
re-importing the old scattering when components are migrated over.

## Recommendation: two-tier token system, defined once in CSS

**Tier 1 — primitives:** raw values (a small brand palette + grays), rarely
referenced directly by components.

**Tier 2 — semantic tokens:** named by *purpose*, not color — e.g. `brand`,
`surface`, `surface-muted`, `border`, `danger`, `success`, `warning`, `info`,
`text-muted`. Components only ever use Tier 2 names.

Because the project is on Tailwind v4, this is defined as CSS custom
properties inside the `@theme` block in `index.css` (same place
`--font-cinzel` already lives) instead of a JS config. Tailwind
auto-generates utilities from these variables (`bg-danger`, `text-brand`,
`border-surface`, ...), so there's one file and one source of truth.

Benefits:

- Rename/retheme by changing one variable instead of hunting through
  components.
- Dark/light or theme variants become trivial — override the semantic
  variables under `.dark` (the `dark` custom-variant is already set up in
  `index.css`) instead of touching component markup.
- Prevents having several slightly different "danger" reds scattered around.

## Build a thin primitives layer before migrating features

`clsx`, `tailwind-merge`, and Radix are already installed, but there is no
`cn()` helper or actual UI components yet. Before migrating feature
components, stand up a handful of primitives in `components/ui` (Button,
Badge, Card, Alert/Toast, Input) built with `cva` (worth adding as a
dependency), a `cn()` helper, and the semantic tokens. Feature components
should compose these primitives instead of hardcoding utility classes again.

## Use the showcase page as a living guard-rail

Turn `ComponentShowCasePage` into a page that renders every token (swatches)
and every primitive/variant. As each old component is migrated, drop it in
to visually confirm it's using tokens/primitives instead of one-off colors,
catching inconsistencies immediately instead of much later.

## Suggested migration workflow (per component)

1. Pull the old component over from `Eldoria.Web`.
2. Don't copy raw color classes verbatim — map each one to the nearest
   semantic token (this is where duplicates get consolidated).
3. Replace any hand-rolled button/badge/card markup with the new primitives.
4. Drop it into the showcase page to sanity-check against everything else.

## Optional guard-rail

Once the token set stabilizes, an ESLint rule (e.g. restricting raw Tailwind
palette classes like `bg-red-400` outside of `styles/` and `components/ui`)
can enforce the discipline automatically instead of relying on code review
memory.

## Next step

Inventory the actual colors/purposes used across `Eldoria.Web` components
(grouped by apparent intent) to arrive at a concrete semantic token list
before naming and implementing anything.
