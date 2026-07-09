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
  ModalStackContext,
  type ModalStackEntry,
  type ModalStackOptions,
} from "./modalStackContext";

export function ModalStackProvider({ children }: PropsWithChildren) {
  const [stack, setStack] = useState<ModalStackEntry[]>([]);
  const nextIdRef = useRef(1);

  const push = useCallback((options: ModalStackOptions) => {
    const id = nextIdRef.current;
    nextIdRef.current += 1;

    setStack((current) => [
      ...current,
      {
        id,
        title: options.title,
        content: options.content,
        placement: options.placement ?? "drawer",
        closeOnBackdrop: options.closeOnBackdrop ?? true,
      },
    ]);

    return id;
  }, []);

  const pop = useCallback(() => {
    setStack((current) => current.slice(0, -1));
  }, []);

  const popTo = useCallback((id: number) => {
    setStack((current) => {
      const index = current.findIndex((entry) => entry.id === id);

      return index >= 0 ? current.slice(0, index + 1) : current;
    });
  }, []);

  const dismissAll = useCallback(() => {
    setStack([]);
  }, []);

  useEffect(() => {
    if (stack.length === 0) return;

    const handleKeyDown = (event: KeyboardEvent) => {
      if (
        event.key === "Escape" &&
        !document.querySelector('[data-confirm-dialog="true"]')
      ) {
        pop();
      }
    };

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.body.style.overflow = previousOverflow;
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [pop, stack.length]);

  const value = useMemo(
    () => ({
      push,
      pop,
      popTo,
      dismissAll,
      depth: stack.length,
    }),
    [dismissAll, pop, popTo, push, stack.length],
  );

  return (
    <ModalStackContext.Provider value={value}>
      {children}

      {stack.map((entry, index) => {
        const isTop = index === stack.length - 1;
        const offset = index * 16;

        return (
          <div
            key={entry.id}
            data-nested-dialog="true"
            className="fixed inset-0 z-60 bg-canvas/70 backdrop-blur-xs"
            style={{ zIndex: 60 + index }}
            aria-hidden={!isTop}
            onMouseDown={(event) => {
              if (
                isTop &&
                entry.closeOnBackdrop &&
                event.target === event.currentTarget
              ) {
                pop();
              }
            }}
          >
            <section
              role="dialog"
              aria-modal={isTop}
              aria-labelledby={`modal-stack-title-${entry.id}`}
              className={
                entry.placement === "drawer"
                  ? "absolute inset-y-0 right-0 flex w-full max-w-xl flex-col border-l border-border bg-surface-raised shadow-2xl"
                  : "absolute left-1/2 top-1/2 flex max-h-[85vh] w-[calc(100%-2rem)] max-w-2xl -translate-x-1/2 -translate-y-1/2 flex-col rounded-xl border border-border bg-surface-raised shadow-2xl"
              }
              style={
                entry.placement === "drawer"
                  ? { transform: `translateX(-${offset}px)` }
                  : undefined
              }
            >
              <header className="flex items-center justify-between gap-4 border-b border-border px-6 py-5">
                <h2
                  id={`modal-stack-title-${entry.id}`}
                  className="text-2xl font-semibold text-content"
                >
                  {entry.title}
                </h2>
                <Button onClick={pop} aria-label="Close modal">
                  Close
                </Button>
              </header>
              <div className="min-h-0 flex-1 overflow-y-auto p-6">
                {entry.content}
              </div>
            </section>
          </div>
        );
      })}
    </ModalStackContext.Provider>
  );
}
