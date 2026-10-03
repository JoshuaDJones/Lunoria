import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { Button } from "@/components/ui";
import type { IntroPageType } from "@/features/journeys/types";
import { parseIntroPageConfig } from "@/features/journeys/introPageConfig";
import { IntroPagePreview } from "@/features/journeys/components/IntroPagePreview";
import "@/features/journeys/introPageTransitions.css";

interface IntroPageViewerProps {
  pages: ViewableIntroPage[];
  initialPageId: number;
  title: string;
  onClose: () => void;
}

interface ViewableIntroPage {
  id: number;
  sortOrder: number;
  type: IntroPageType;
  config: string;
  previewPhotoUrl: string | null;
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
  const [visible, setVisible] = useState(false);
  const [closing, setClosing] = useState(false);
  const closeButtonRef = useRef<HTMLButtonElement>(null);
  const navigating = useRef(false);
  const [transition, setTransition] = useState<{
    previous: ViewableIntroPage;
    direction: "forward" | "backward";
  }>();

  const navigatePage = useCallback(
    (step: number) => {
      const next = pageIndex + step;
      if (
        closing ||
        navigating.current ||
        !page ||
        next < 0 ||
        next >= orderedPages.length
      )
        return;
      if (!window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
        navigating.current = true;
        setTransition({
          previous: page,
          direction: step > 0 ? "forward" : "backward",
        });
      }
      setPageIndex(next);
    },
    [closing, page, pageIndex, orderedPages.length],
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
    let secondFrame = 0;
    const previousFocus = document.activeElement;
    const firstFrame = requestAnimationFrame(() => {
      secondFrame = requestAnimationFrame(() => {
        setVisible(true);
        closeButtonRef.current?.focus();
      });
    });
    return () => {
      cancelAnimationFrame(firstFrame);
      cancelAnimationFrame(secondFrame);
      if (previousFocus instanceof HTMLElement && previousFocus.isConnected)
        previousFocus.focus();
    };
  }, []);

  const requestClose = useCallback(() => {
    setClosing(true);
  }, []);

  useEffect(() => {
    if (!closing) return;
    const delay = window.matchMedia("(prefers-reduced-motion: reduce)").matches
      ? 0
      : 350;
    const timer = setTimeout(onClose, delay);
    return () => clearTimeout(timer);
  }, [closing, onClose]);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (closing) return;
      if (event.key === "Escape") {
        requestClose();
      } else if (event.key === "ArrowLeft") {
        event.preventDefault();
        navigatePage(-1);
      } else if (event.key === "ArrowRight") {
        event.preventDefault();
        navigatePage(1);
      }
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [requestClose, closing, navigatePage]);

  if (!page) return null;

  return createPortal(
    <div
      className={`fixed inset-0 z-100 flex bg-canvas/95 backdrop-blur-sm transition-opacity duration-350 motion-reduce:transition-none ${visible && !closing ? "opacity-100" : "opacity-0"}`}
    >
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
          <Button
            ref={closeButtonRef}
            onClick={requestClose}
            disabled={closing}
          >
            Close
          </Button>
        </header>

        <div className="relative min-h-0 flex-1 overflow-hidden bg-canvas">
          {(transition ? [transition.previous, page] : [page]).map((slide) => {
            const outgoing = slide.id !== page.id;
            return (
              <div
                key={slide.id}
                aria-hidden={outgoing || undefined}
                inert={outgoing || Boolean(transition)}
                className={`absolute inset-0 ${transition ? `intro-slide-${outgoing ? "out" : "in"}-${transition.direction}` : ""}`}
              >
                <IntroPagePreview
                  type={slide.type}
                  config={parseIntroPageConfig(slide.config)}
                  imageUrl={slide.previewPhotoUrl}
                  fullScreen
                />
              </div>
            );
          })}
        </div>

        <footer className="flex items-center justify-between border-t border-border px-5 py-4">
          <Button
            disabled={closing || Boolean(transition) || pageIndex === 0}
            onClick={() => navigatePage(-1)}
            size="lg"
            className="py-2.5"
          >
            Previous
          </Button>
          <Button
            disabled={
              closing ||
              Boolean(transition) ||
              pageIndex >= orderedPages.length - 1
            }
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
