import { useEffect, useRef, type ReactNode } from "react";
import { Button } from "./Button";

interface DrawerProps {
  title: string;
  children: ReactNode;
  onClose: () => void;
}

export function Drawer({ title, children, onClose }: DrawerProps) {
  const closeButtonRef = useRef<HTMLButtonElement>(null);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (
        event.key === "Escape" &&
        !document.querySelector(
          '[data-nested-dialog="true"], [data-confirm-dialog="true"]',
        )
      ) {
        onClose();
      }
    };

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    document.addEventListener("keydown", handleKeyDown);
    closeButtonRef.current?.focus();

    return () => {
      document.body.style.overflow = previousOverflow;
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [onClose]);

  return (
    <div
      className="fixed inset-0 z-50 bg-canvas/70 backdrop-blur-xs"
      onMouseDown={(event) => {
        if (event.target === event.currentTarget) {
          onClose();
        }
      }}
    >
      <section
        role="dialog"
        aria-modal="true"
        aria-labelledby="drawer-title"
        className="absolute inset-y-0 right-0 flex w-full max-w-xl animate-[drawer-in_250ms_ease-out] flex-col border-l border-border bg-surface-raised shadow-2xl"
      >
        <header className="flex items-center justify-between gap-4 border-b border-border px-6 py-5">
          <h2 id="drawer-title" className="text-2xl font-semibold text-content">
            {title}
          </h2>
          <Button ref={closeButtonRef} onClick={onClose} aria-label="Close">
            Close
          </Button>
        </header>
        <div className="flex-1 overflow-y-auto p-6">{children}</div>
      </section>
    </div>
  );
}
