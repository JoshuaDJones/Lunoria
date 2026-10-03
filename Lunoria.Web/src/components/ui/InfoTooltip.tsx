import { useState } from "react";
import {
  autoUpdate,
  flip,
  FloatingPortal,
  hide,
  offset,
  safePolygon,
  shift,
  useDismiss,
  useFloating,
  useFocus,
  useHover,
  useInteractions,
  useRole,
} from "@floating-ui/react";

export function InfoTooltip({ text }: { text: string }) {
  const [open, setOpen] = useState(false);
  const {
    refs: { setReference, setFloating },
    floatingStyles,
    context,
    middlewareData,
  } = useFloating({
    open,
    onOpenChange: setOpen,
    placement: "top-end",
    strategy: "fixed",
    whileElementsMounted: autoUpdate,
    middleware: [
      offset(8),
      flip({ padding: 8 }),
      shift({ padding: 8 }),
      hide(),
    ],
  });
  const hover = useHover(context, { handleClose: safePolygon() });
  const focus = useFocus(context);
  const dismiss = useDismiss(context, { bubbles: false });
  const role = useRole(context, { role: "tooltip" });
  const { getReferenceProps, getFloatingProps } = useInteractions([
    hover,
    focus,
    dismiss,
    role,
  ]);

  return (
    <>
      <button
        ref={setReference}
        type="button"
        aria-label={`Item information: ${text}`}
        className="flex h-6 w-6 items-center justify-center rounded-full bg-canvas/90 text-content shadow-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility"
        {...getReferenceProps()}
      >
        <svg
          aria-hidden="true"
          viewBox="0 0 24 24"
          className="h-4 w-4"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          strokeLinecap="round"
        >
          <circle cx="12" cy="12" r="9" />
          <path d="M12 11v6M12 7h.01" />
        </svg>
      </button>
      {open && (
        <FloatingPortal>
          <div
            ref={setFloating}
            style={{
              ...floatingStyles,
              visibility: middlewareData.hide?.referenceHidden
                ? "hidden"
                : undefined,
            }}
            className="z-[1000] w-max max-w-[min(12rem,calc(100vw-1rem))] break-words rounded-lg border border-border bg-canvas px-3 py-2 text-xs text-content shadow-lg"
            {...getFloatingProps()}
          >
            {text}
          </div>
        </FloatingPortal>
      )}
    </>
  );
}
