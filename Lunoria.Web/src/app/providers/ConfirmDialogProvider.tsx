import {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type PropsWithChildren,
} from "react";
import { Button } from "@/components/ui";
import {
  ConfirmDialogContext,
  type ConfirmDialogOptions,
  type PendingConfirmation,
} from "./confirmDialogContext";

export function ConfirmDialogProvider({ children }: PropsWithChildren) {
  const [pending, setPending] = useState<PendingConfirmation>();
  const cancelButtonRef = useRef<HTMLButtonElement>(null);

  const close = useCallback(
    (confirmed: boolean) => {
      if (!pending) return;

      pending.resolve(confirmed);
      setPending(undefined);
    },
    [pending],
  );

  const confirm = useCallback((options: ConfirmDialogOptions) => {
    return new Promise<boolean>((resolve) => {
      setPending((current) => {
        current?.resolve(false);

        return {
          title: options.title,
          message: options.message,
          confirmLabel: options.confirmLabel ?? "Confirm",
          cancelLabel: options.cancelLabel ?? "Cancel",
          variant: options.variant ?? "primary",
          resolve,
        };
      });
    });
  }, []);

  useEffect(() => {
    if (!pending) return;

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        close(false);
      }
    };

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    document.addEventListener("keydown", handleKeyDown);
    cancelButtonRef.current?.focus();

    return () => {
      document.body.style.overflow = previousOverflow;
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [close, pending]);

  const value = useMemo(() => ({ confirm }), [confirm]);

  return (
    <ConfirmDialogContext.Provider value={value}>
      {children}

      {pending && (
        <div
          data-confirm-dialog="true"
          className="fixed inset-0 z-80 flex items-center justify-center bg-canvas/80 p-4 backdrop-blur-xs"
          onMouseDown={(event) => {
            if (event.target === event.currentTarget) {
              close(false);
            }
          }}
        >
          <section
            role="alertdialog"
            aria-modal="true"
            aria-labelledby="confirm-dialog-title"
            aria-describedby={
              pending.message ? "confirm-dialog-message" : undefined
            }
            className="w-full max-w-md rounded-xl border border-border bg-surface-raised p-6 shadow-2xl"
          >
            <h2
              id="confirm-dialog-title"
              className="text-2xl font-semibold text-content"
            >
              {pending.title}
            </h2>

            {pending.message && (
              <div
                id="confirm-dialog-message"
                className="mt-3 text-content-secondary"
              >
                {pending.message}
              </div>
            )}

            <div className="mt-6 flex justify-end gap-3">
              <Button ref={cancelButtonRef} onClick={() => close(false)}>
                {pending.cancelLabel}
              </Button>
              <Button
                variant={pending.variant}
                inverted={pending.variant === "danger"}
                onClick={() => close(true)}
              >
                {pending.confirmLabel}
              </Button>
            </div>
          </section>
        </div>
      )}
    </ConfirmDialogContext.Provider>
  );
}
