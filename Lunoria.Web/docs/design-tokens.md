# Color Design Tokens

This project uses semantic color tokens. A semantic token is named after the
job a color performs in the interface, not after the color itself.

For example:

```tsx
<button className="bg-brand text-on-brand hover:bg-brand-hover">
  Continue
</button>
```

The component communicates its intent without knowing that the current brand
color is amber. The application can later change from amber to gold, support
another theme, or adjust contrast by editing the tokens in one place.

The tokens are defined in `src/styles/index.css` inside Tailwind's `@theme`
block. Tailwind generates utilities such as `bg-brand`, `text-content-muted`,
and `border-border` from variables whose names start with `--color-`.

## Current tokens

### Brand

| Token          | Current color | Purpose                                                                                        |
| -------------- | ------------- | ---------------------------------------------------------------------------------------------- |
| `brand`        | `amber-500`   | Primary brand and interactive accent. Use for primary buttons and other high-emphasis actions. |
| `brand-hover`  | `amber-400`   | Hover or focus treatment for a brand-colored element.                                          |
| `brand-subtle` | `amber-300`   | Lower-intensity brand treatment, usually with opacity for decorative borders or highlights.    |

Examples:

```tsx
className = "bg-brand hover:bg-brand-hover";
className = "text-brand-hover hover:text-brand-subtle";
className = "border-brand-subtle/20";
```

`brand-subtle` means lower visual emphasis, not necessarily a numerically
darker color. In the current dark theme, a lighter amber with low opacity
creates a subtle border.

### Backgrounds and surfaces

| Token            | Current color | Purpose                                                                                                  |
| ---------------- | ------------- | -------------------------------------------------------------------------------------------------------- |
| `canvas`         | `slate-950`   | The application's lowest, page-level background.                                                         |
| `surface`        | `slate-950`   | A contained region placed on the canvas, such as a card, panel, or dialog.                               |
| `surface-raised` | `slate-900`   | A surface that should appear one level above or distinct from its surrounding surface, such as an input. |

`canvas` and `surface` currently resolve to the same color, but they represent
different roles. Keeping both names allows their colors to diverge later
without changing component markup.

Examples:

```tsx
<body className="bg-canvas">
<section className="bg-surface/90">
<input className="bg-surface-raised">
```

### Content

| Token                 | Current color | Purpose                                                         |
| --------------------- | ------------- | --------------------------------------------------------------- |
| `content`             | `slate-100`   | Primary text and icons that need the strongest normal emphasis. |
| `content-secondary`   | `slate-300`   | Supporting text and labels that remain easy to read.            |
| `content-muted`       | `slate-400`   | Metadata, descriptions, hints, and lower-priority copy.         |
| `content-placeholder` | `slate-600`   | Placeholder text inside form controls.                          |
| `on-brand`            | `slate-950`   | Content displayed on a brand-colored background.                |

The `on-*` convention describes a foreground chosen specifically for contrast
against a background:

```tsx
<button className="bg-brand text-on-brand">Sign in</button>
```

Do not use `on-brand` for ordinary dark text elsewhere. Its contract is
specifically "readable content on the brand background."

### Structure and feedback

| Token    | Current color | Purpose                                             |
| -------- | ------------- | --------------------------------------------------- |
| `border` | `slate-700`   | Default border for controls, cards, and separators. |
| `danger` | `red-400`     | Errors, destructive actions, or harmful states.     |

`danger` describes meaning rather than a component. It can therefore be used
for error text, an invalid border, a destructive button, or an alert icon.
Those usages may eventually need separate variants such as
`danger-surface` and `on-danger`.

## Naming convention

Use this general pattern:

```text
role[-variant][-state]
```

- **Role** answers: "What job does this perform?" Examples: `brand`,
  `surface`, `content`, `border`, `danger`.
- **Variant** answers: "Which level or context?" Examples: `raised`,
  `secondary`, `muted`, `placeholder`, `on-brand`.
- **State** answers: "Which interaction state?" Examples: `hover`, `focus`,
  `disabled`, `selected`.

Prefer the shortest name that still communicates a stable purpose.

Good names:

```text
brand
brand-hover
surface-raised
content-muted
on-brand
danger
danger-surface
on-danger
```

Avoid names tied to:

- A literal color: `gold`, `dark-gray`, `red-text`
- A particular page: `login-background`, `landing-border`
- A particular element when the role is reusable: `submit-button-yellow`
- Unclear numbering without a documented scale: `surface-2`, `text-3`

Literal palette names such as `amber-500` are appropriate in the token
definitions. Components should normally use semantic names.

## How to choose a token

When adding or styling an element, ask these questions in order:

1. Is this a page background, contained surface, or raised surface?
2. Is this primary, secondary, muted, or placeholder content?
3. Is this the primary brand action or accent?
4. Does it communicate a state such as danger, success, warning, or info?
5. Is it a default structural border?
6. Is this foreground displayed on a strongly colored background?

Use an existing token when its purpose matches. Do not choose a token merely
because its current color looks correct. For example, do not use `danger`
for decorative red text: that would incorrectly communicate an error or
destructive meaning.

## When to add a new token

Add a token when all of the following are true:

1. A distinct semantic role is missing.
2. The role is expected to be reused, or central control provides meaningful
   consistency.
3. Existing tokens would communicate the wrong purpose.

Common future additions might include:

```css
--color-success: var(--color-emerald-400);
--color-warning: var(--color-yellow-400);
--color-info: var(--color-sky-400);

--color-danger-surface: var(--color-red-950);
--color-on-danger: var(--color-white);

--color-border-strong: var(--color-slate-500);
--color-content-disabled: var(--color-slate-600);
```

Do not create a new token for every slight visual difference. First check
whether opacity provides the intended emphasis:

```tsx
className = "border-brand-subtle/20";
className = "bg-surface/90";
className = "focus:ring-brand-hover/20";
```

If the same opacity is used repeatedly for the same semantic purpose, it may
then justify a dedicated token or reusable component style.

## Practical consistency rules

1. Components use semantic utilities; `src/styles/index.css` maps them to raw
   Tailwind colors.
2. Name colors by purpose, not appearance or page.
3. Use `on-*` for foreground colors designed for a specific background.
4. Use state suffixes only when the color is genuinely tied to that state.
5. Keep content emphasis levels ordered: `content`, `content-secondary`,
   `content-muted`, then `content-placeholder`.
6. If a token starts serving unrelated purposes, split it into clearer roles.
7. Check text and control contrast whenever a token value changes.

## Current source of truth

All color values belong in:

```text
src/styles/index.css
```

Usage belongs in component class names:

```tsx
className = "border-border bg-surface-raised text-content";
```

This separation lets the CSS file answer "what colors does the theme use?"
while component markup answers "what purpose does each color serve?"
