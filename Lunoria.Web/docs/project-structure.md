# Lunoria.Web Project Structure

This application uses a feature-based structure. Code should remain close to
the feature that owns it unless it is genuinely shared by unrelated features.

## Source Layout

```text
src/
├── app/
│   ├── layouts/
│   └── router.tsx
├── assets/
├── components/
│   ├── layout/
│   └── ui/
├── features/
│   ├── auth/
│   ├── characters/
│   ├── equipment/
│   ├── items/
│   ├── journeys/
│   ├── playthroughs/
│   ├── scenes/
│   └── spells/
├── hooks/
├── lib/
├── pages/
├── styles/
├── main.tsx
└── vite-env.d.ts
```

## `app`

Application-wide composition and configuration.

- `router.tsx` defines routes and connects them to page components.
- `layouts/` contains route layouts such as `AppLayout` and `AuthLayout`.
- `providers/` contains application-wide providers and hooks such as auth,
  confirmation dialogs, and the modal stack.

Do not place feature business logic here.

## `pages`

Top-level route components.

Pages should be thin. They should assemble layouts and feature components rather
than contain API calls, large forms, or substantial business logic.

Examples:

- `HomePage.tsx`
- `JourneysPage.tsx`
- `CharacterCatalogPage.tsx`
- `SceneDashboardPage.tsx`

## `features`

Business functionality organized by domain.

Each feature may contain:

```text
features/characters/
├── api/
├── components/
├── hooks/
├── schemas/
└── types.ts
```

### `api`

API requests and feature-specific query functions.

Examples:

- `getCharacters.ts`
- `createCharacter.ts`
- `updateCharacter.ts`

### `components`

Components owned by one feature, including its cards, lists, forms, and modals.

Examples:

- `CharacterCard.tsx`
- `CharacterList.tsx`
- `CharacterForm.tsx`
- `CharacterFormModal.tsx`

A modal should remain in its feature instead of going into a global `modals`
directory.

Feature-owned modal content can still be displayed by the shared modal stack
when the workflow needs nesting. See `docs/dialogs-and-modals.md` for when to
use page-owned drawers, confirmation dialogs, and stacked modals.

### `hooks`

Hooks that coordinate feature behavior.

Examples:

- `useCharacters.ts`
- `useCreateCharacter.ts`
- `useCharacterForm.ts`

### `schemas`

Form and runtime validation schemas.

Examples:

- `characterSchema.ts`
- `journeySchema.ts`

### `types.ts`

Types used primarily by that feature. Avoid creating one global type directory
for every API response.

## `components/ui`

Small, reusable UI primitives with no domain knowledge.

Examples:

- `Button.tsx`
- `Input.tsx`
- `Select.tsx`
- `Card.tsx`
- `Modal.tsx`
- `Spinner.tsx`

These components should not know about characters, journeys, scenes, or API
endpoints.

## `components/layout`

Shared visual structure used across multiple pages.

Examples:

- `Navigation.tsx`
- `PageContainer.tsx`
- `PageHeader.tsx`
- `Sidebar.tsx`

Feature-specific layouts should remain inside their feature.

## `hooks`

Hooks shared by multiple unrelated features.

Examples:

- `useDebounce.ts`
- `useLocalStorage.ts`
- `useMediaQuery.ts`

Start a hook inside its feature. Move it here only when another feature needs
the same behavior.

## `lib`

Framework-independent utilities and application infrastructure.

Examples:

- `apiClient.ts`
- `cn.ts`
- `queryClient.ts`
- `formatDate.ts`

Do not place React components here.

## `styles`

Global styles, Tailwind configuration layers, animations, and shared CSS.

Component-specific styling should normally stay with the component through
Tailwind classes or a colocated stylesheet.

## `assets`

Static assets imported by source code, such as images, icons, and fonts.

Files that must be served unchanged from a fixed URL should instead go in the
project-level `public/` directory.

## Placement Rules

1. Start domain code inside the feature that owns it.
2. Keep route pages focused on composition.
3. Move a component to `components/ui` only when it is domain-independent.
4. Move code to a shared directory only after multiple unrelated features need
   it.
5. Keep API functions, types, hooks, and components for a feature close
   together.
6. Use the `@/` alias for all imports from `src`.
7. Avoid duplicate directory trees outside `src`.

## Naming

- Components and pages use PascalCase: `JourneyCard.tsx`.
- Hooks use camelCase beginning with `use`: `useJourneys.ts`.
- Utilities use camelCase: `formatDate.ts`.
- Prefer domain names over location-based names. Use `JourneyCharacterCard`
  instead of `DashboardPlayerCard`.
- Use descriptive modal names such as `CharacterFormModal`, not
  `AddEditModal`.

## Promoting Shared Code

Do not make a component shared preemptively.

For example, `features/journeys/components/JourneyCard.tsx` should remain in the
journeys feature. If its visual shell is later needed by unrelated features,
extract only the generic shell to `components/ui/Card.tsx` and keep the
journey-specific content in `JourneyCard.tsx`.
