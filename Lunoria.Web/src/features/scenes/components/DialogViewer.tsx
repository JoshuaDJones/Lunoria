import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { Button } from "@/components/ui";
import { DialogPageType } from "@/features/scenes/types";
import { DialogViewerPage } from "@/features/scenes/components/DialogViewerPage";
import "@/features/journeys/introPageTransitions.css";

export interface DialogViewerDialog {
  title: string;
  dialogPages: Array<{
    id: number;
    orderNum: number;
    pageType: DialogPageType;
    mediaUrl: string;
    mediaContentType: string;
    dialogPageSections: Array<{
      id: number;
      orderNum: number;
      readingText: string;
      isNarrator: boolean;
      character: {
        name: string;
        photoUrl: string | null;
        dialogActiveColor?: string;
        characterDialogSettings?: { dialogActiveColor: string } | null;
      } | null;
    }> | null;
  }> | null;
}

interface DialogViewerProps {
  dialog: DialogViewerDialog;
  onClose: () => void;
}

export function DialogViewer({ dialog, onClose }: DialogViewerProps) {
  const pages = useMemo(
    () =>
      [...(dialog.dialogPages ?? [])].sort(
        (left, right) => left.orderNum - right.orderNum,
      ),
    [dialog.dialogPages],
  );
  const [pageIndex, setPageIndex] = useState(0);
  const page = pages[pageIndex];
  const pageContainer = useRef<HTMLDivElement>(null);
  const navigating = useRef(false);
  const [transition, setTransition] = useState<{
    previous: NonNullable<DialogViewerDialog["dialogPages"]>[number];
    direction: "forward" | "backward";
  }>();

  const navigatePage = useCallback(
    (step: number) => {
      const next = pageIndex + step;
      if (navigating.current || !page || next < 0 || next >= pages.length)
        return;
      pageContainer.current
        ?.querySelectorAll("video")
        .forEach((video) => video.pause());
      if (!window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
        navigating.current = true;
        setTransition({
          previous: page,
          direction: step > 0 ? "forward" : "backward",
        });
      }
      setPageIndex(next);
    },
    [page, pageIndex, pages.length],
  );

  useEffect(() => {
    if (!transition) return;
    const timer = setTimeout(() => {
      setTransition(undefined);
      navigating.current = false;
    }, 320);
    return () => clearTimeout(timer);
  }, [transition]);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        onClose();
        return;
      }

      const target = event.target;
      if (
        target instanceof HTMLElement &&
        (target.closest("video") ||
          target.closest("input, textarea, select, button"))
      ) {
        return;
      }

      if (event.key === "ArrowLeft") {
        event.preventDefault();
        navigatePage(-1);
      } else if (event.key === "ArrowRight" && pages.length > 0) {
        event.preventDefault();
        navigatePage(1);
      }
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [onClose, pages.length, navigatePage]);

  return createPortal(
    <div
      data-nested-dialog="true"
      className="fixed inset-0 z-100 flex bg-canvas/95 backdrop-blur-sm"
    >
      <section
        role="dialog"
        aria-modal="true"
        aria-labelledby="dialog-viewer-title"
        className="relative flex h-dvh min-h-0 w-full flex-col overflow-hidden bg-surface-raised shadow-2xl"
      >
        <header className="flex items-center justify-between gap-4 border-b border-border px-5 py-4">
          <div>
            <h2
              id="dialog-viewer-title"
              className="text-2xl font-semibold text-content"
            >
              {dialog.title}
            </h2>
            <p className="text-sm text-content-muted">
              {pages.length
                ? `Page ${pageIndex + 1} of ${pages.length}`
                : "No pages"}
            </p>
          </div>
          <Button onClick={onClose}>Close</Button>
        </header>

        <div
          ref={pageContainer}
          className="relative min-h-0 flex-1 overflow-hidden bg-canvas"
        >
          {!page && (
            <p className="flex h-full items-center justify-center text-content-muted">
              This dialog has no pages.
            </p>
          )}
          {(page
            ? transition
              ? [transition.previous, page]
              : [page]
            : []
          ).map((slide) => {
            const outgoing = slide.id !== page?.id;
            return (
              <div
                key={slide.id}
                aria-hidden={outgoing || undefined}
                inert={outgoing || Boolean(transition)}
                className={`absolute inset-0 ${transition ? `intro-slide-${outgoing ? "out" : "in"}-${transition.direction}` : ""}`}
              >
                <DialogViewerPage page={slide} />
              </div>
            );
          })}
        </div>

        <footer className="flex items-center justify-between border-t border-border px-5 py-4">
          <Button
            disabled={Boolean(transition) || pageIndex === 0}
            onClick={() => navigatePage(-1)}
            size="lg"
            className="py-2.5"
          >
            Previous
          </Button>
          <Button
            disabled={Boolean(transition) || pageIndex >= pages.length - 1}
            onClick={() => navigatePage(1)}
            variant="primary"
            size="lg"
            className="py-2.5"
          >
            Next
          </Button>
        </footer>
      </section>
    </div>,
    document.body,
  );
}
