# Dialogs and Modals

Lunoria uses four overlay patterns. Choose the smallest pattern that matches
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

## Toast notifications

Use `useToast` to report the result of an operation or provide brief,
non-blocking information. Toasts must not replace a confirmation dialog when
the user needs to make a decision.

```tsx
const toast = useToast();

toast.success("Journey deleted.");
toast.info("Your changes are still processing.");
toast.error("The journey could not be deleted.", "Unable to delete journey");
```

The `success`, `info`, and `error` helpers accept a message and an optional
title. For custom behavior, use `show`:

```tsx
toast.show({
  title: "Import complete",
  message: "12 characters were imported.",
  variant: "success",
  duration: 10_000,
});
```

Success and information toasts dismiss after five seconds, while errors remain
for eight seconds. Set `duration: 0` to require manual dismissal. Users can
always dismiss a toast with its close button, and the timer pauses while the
toast is hovered or focused and restarts when that interaction ends.

The global toast viewport stacks up to five notifications and appears above
drawers, modal stacks, confirmation dialogs, and full-screen viewers. Keep
messages concise and do not include important content that is available only in
a toast.

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
toast variants, confirm dialogs, centered modals, and nested modal stacks.
