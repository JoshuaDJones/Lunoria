import { useEffect, useMemo, useState } from "react";
import { createPortal } from "react-dom";
import { Button } from "@/components/ui";
import type { IntroPage } from "@/features/journeys/types";
import { parseIntroPageConfig } from "@/features/journeys/introPageConfig";
import { IntroPagePreview } from "./IntroPagePreview";

interface IntroPageViewerProps {
  pages: IntroPage[];
  initialPageId: number;
  title: string;
  onClose: () => void;
}

export function IntroPageViewer({
  pages,
  initialPageId,
  title,
  onClose,
}: IntroPageViewerProps) {
  const orderedPages = useMemo(
    () => [...pages].sort((left, right) => left.sortOrder - right.sortOrder),
    [pages],
  );
  const initialIndex = Math.max(
    0,
    orderedPages.findIndex((page) => page.id === initialPageId),
  );
  const [pageIndex, setPageIndex] = useState(initialIndex);
  const page = orderedPages[pageIndex];

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        onClose();
      } else if (event.key === "ArrowLeft") {
        setPageIndex((current) => Math.max(0, current - 1));
      } else if (event.key === "ArrowRight") {
        setPageIndex((current) =>
          Math.min(orderedPages.length - 1, current + 1),
        );
      }
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [onClose, orderedPages.length]);

  if (!page) return null;

  return createPortal(
    <div className="fixed inset-0 z-100 flex bg-canvas/95 backdrop-blur-sm">
      <section
        role="dialog"
        aria-modal="true"
        aria-labelledby="intro-page-viewer-title"
        className="relative flex h-dvh min-h-0 w-full flex-col overflow-hidden bg-surface-raised shadow-2xl"
      >
        <header className="flex items-center justify-between gap-4 border-b border-border px-5 py-4">
          <div>
            <h2
              id="intro-page-viewer-title"
              className="text-2xl font-semibold text-content"
            >
              {title}
            </h2>
            <p className="text-sm text-content-muted">
              Page {pageIndex + 1} of {orderedPages.length}
            </p>
          </div>
          <Button onClick={onClose}>Close</Button>
        </header>

        <div className="relative min-h-0 flex-1 overflow-hidden bg-canvas">
          <IntroPagePreview
            key={page.id}
            type={page.type}
            config={parseIntroPageConfig(page.config)}
            imageUrl={page.previewPhotoUrl}
            fullScreen
          />
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
            disabled={pageIndex >= orderedPages.length - 1}
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
