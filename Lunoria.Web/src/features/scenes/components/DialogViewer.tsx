import { useEffect, useMemo, useState } from "react";
import { createPortal } from "react-dom";
import type { SceneDialog } from "@/features/scenes/types";
import { Button } from "@/components/ui";

interface DialogViewerProps {
  dialog: SceneDialog;
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
  const sections = [...(page?.dialogPageSections ?? [])].sort(
    (left, right) => left.orderNum - right.orderNum,
  );

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        onClose();
      } else if (event.key === "ArrowLeft") {
        setPageIndex((current) => Math.max(0, current - 1));
      } else if (event.key === "ArrowRight" && pages.length > 0) {
        setPageIndex((current) => Math.min(pages.length - 1, current + 1));
      }
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [onClose, pages.length]);

  return createPortal(
    <div className="fixed inset-0 z-100 flex bg-canvas/95 backdrop-blur-sm">
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

        <div className="relative flex min-h-0 flex-1 items-center justify-center overflow-hidden bg-canvas">
          {page?.photoUrl ? (
            <img
              src={page.photoUrl}
              alt=""
              className="h-full max-h-full w-full max-w-full object-contain"
            />
          ) : (
            <p className="text-content-muted">This page has no image.</p>
          )}

          {sections.length > 0 && (
            <div className="scrollbar-hide absolute inset-x-4 bottom-4 h-full py-10 space-y-3 overflow-y-auto sm:inset-x-[10%] flex items-center justify-center">
              <div className="flex flex-col items-center gap-5 w-[60%]">
                {sections.map((section) => {
                  const speaker = section.isNarrator
                    ? "Narrator"
                    : (section.character?.name ?? "Unknown character");

                  return (
                    <article
                      key={section.id}
                      style={{
                        borderColor:
                          section.character?.characterDialogSettings
                            ?.dialogActiveColor || undefined,
                      }}
                      className="rounded-xl border-4 border-border bg-canvas/85 p-4 shadow-xl backdrop-blur-sm w-full opacity-50 transition-opacity hover:opacity-100"
                    >
                      <div className="flex items-center gap-3">
                        {!section.isNarrator && section.character?.photoUrl && (
                          <img
                            src={section.character.photoUrl}
                            alt=""
                            className="h-10 w-10 rounded-md object-cover"
                          />
                        )}
                        <h3 className="font-semibold text-4xl text-content">
                          {speaker}
                        </h3>
                      </div>
                      <p className="mt-2 whitespace-pre-wrap text-2xl text-content-secondary">
                        {section.readingText}
                      </p>
                    </article>
                  );
                })}
              </div>
            </div>
          )}
        </div>

        <footer className="flex items-center justify-between border-t border-border px-5 py-4">
          <Button
            disabled={pageIndex === 0}
            onClick={() => setPageIndex((current) => current - 1)}
            size="lg"
            className="py-2.5"
          >
            Previous
          </Button>
          <Button
            disabled={pageIndex >= pages.length - 1}
            onClick={() => setPageIndex((current) => current + 1)}
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
