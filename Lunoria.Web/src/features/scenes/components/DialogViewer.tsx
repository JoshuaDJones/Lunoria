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
  const [sectionIndex, setSectionIndex] = useState(0);
  const page = pages[pageIndex];
  const pageContainer = useRef<HTMLDivElement>(null);
  const viewer = useRef<HTMLElement>(null);
  const navigating = useRef(false);
  const [transition, setTransition] = useState<{
    previous: NonNullable<DialogViewerDialog["dialogPages"]>[number];
    direction: "forward" | "backward";
    previousSectionIndex: number;
  }>();

  useEffect(() => {
    const previousFocus = document.activeElement;
    // Wait for the launching menu/dialog to finish restoring its own focus.
    let secondFrame = 0;
    const firstFrame = requestAnimationFrame(() => {
      secondFrame = requestAnimationFrame(() => viewer.current?.focus());
    });
    return () => {
      cancelAnimationFrame(firstFrame);
      cancelAnimationFrame(secondFrame);
      if (previousFocus instanceof HTMLElement && previousFocus.isConnected)
        previousFocus.focus();
    };
  }, []);

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
          previousSectionIndex: sectionIndex,
          direction: step > 0 ? "forward" : "backward",
        });
      }
      setPageIndex(next);
      setSectionIndex(0);
    },
    [page, pageIndex, pages.length, sectionIndex],
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
        event.preventDefault();
        event.stopImmediatePropagation();
        onClose();
        return;
      }

      const target = event.target;
      if (event.altKey || event.ctrlKey || event.metaKey) return;
      if (
        target instanceof HTMLElement &&
        viewer.current?.contains(target) &&
        (target.closest("video") ||
          target.closest(
            "input, textarea, select, [contenteditable]:not([contenteditable='false'])",
          ))
      ) {
        return;
      }

      if (event.key === "ArrowLeft") {
        event.preventDefault();
        event.stopImmediatePropagation();
        navigatePage(-1);
      } else if (event.key === "ArrowRight" && pages.length > 0) {
        event.preventDefault();
        event.stopImmediatePropagation();
        navigatePage(1);
      } else if (event.key === "ArrowUp" || event.key === "ArrowDown") {
        if (
          page?.pageType !== DialogPageType.Image ||
          !page.dialogPageSections?.length
        )
          return;
        event.preventDefault();
        event.stopImmediatePropagation();
        if (navigating.current) return;
        const count = page.dialogPageSections.length;
        setSectionIndex((current) =>
          Math.max(
            0,
            Math.min(count - 1, current + (event.key === "ArrowDown" ? 1 : -1)),
          ),
        );
      }
    };

    // The full-screen viewer owns navigation before underlying controls handle it.
    window.addEventListener("keydown", handleKeyDown, true);
    return () => window.removeEventListener("keydown", handleKeyDown, true);
  }, [onClose, pages.length, navigatePage, page]);

  return createPortal(
    <div
      data-nested-dialog="true"
      className="fixed inset-0 z-100 flex bg-canvas/95 backdrop-blur-sm"
    >
      <section
        ref={viewer}
        tabIndex={-1}
        role="dialog"
        aria-modal="true"
        aria-labelledby="dialog-viewer-title"
        className="relative flex h-dvh min-h-0 w-full flex-col overflow-hidden bg-surface-raised shadow-2xl outline-none"
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
            {page?.pageType === DialogPageType.Image &&
              !!page.dialogPageSections?.length && (
                <p className="text-sm text-content-muted" role="status">
                  Section {sectionIndex + 1} of {page.dialogPageSections.length}
                </p>
              )}
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
                <DialogViewerPage
                  page={slide}
                  activeSectionIndex={
                    outgoing && transition
                      ? transition.previousSectionIndex
                      : sectionIndex
                  }
                  outgoing={outgoing}
                />
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
          <p className="hidden text-sm text-content-muted sm:block">
            ↑ ↓ Sections · ← → Pages
          </p>
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
