# Dialogs and Modals

Lunoria uses three overlay patterns. Choose the smallest pattern that matches
the workflow.

## Page-owned drawers

Use a page-owned `Drawer` for ordinary create and edit flows that belong to the
current route.

```tsx
{
  editing !== undefined && (
    <Drawer title="Edit character" onClose={() => setEditing(undefined)}>
      <ResourceForm />
    </Drawer>
  );
}
```

The page owns the state because the drawer is part of that page workflow.

## Confirm dialogs

Use `useConfirmDialog` for destructive or irreversible confirmation prompts.
Do not use `window.confirm`.

```tsx
const { confirm } = useConfirmDialog();

const confirmed = await confirm({
  title: "Delete character?",
  message: "This action cannot be undone.",
  confirmLabel: "Delete",
  variant: "danger",
});
```

The provider handles Escape, backdrop cancel, focus, and consistent danger
button styling.

## Modal stack

Use `useModalStack` when an overlay needs to open another overlay, or when a
nested modal should sit above a drawer.

```tsx
const modalStack = useModalStack();

modalStack.push({
  title: "Select spells",
  placement: "center",
  content: <SpellPickerDialog onClose={modalStack.pop} />,
});
```

Prefer `placement: "drawer"` for longer forms and `placement: "center"` for
focused pickers, previews, and short decisions. The stack owns Escape handling,
backdrop close, body scroll lock, and top-modal ordering.

## Component Display

The authenticated `/components` route contains live examples for buttons,
confirm dialogs, centered modals, and nested modal stacks.
