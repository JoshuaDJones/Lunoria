import { useState } from "react";
import type { IntroPage } from "@/features/journeys/types";
import {
  introPageTypeLabels,
  parseIntroPageConfig,
} from "@/features/journeys/introPageConfig";
import { IntroPageCanvas } from "@/features/journeys/components/IntroPageCanvas";

interface IntroPageRailProps {
  pages: IntroPage[];
  selectedId?: number;
  disabled: boolean;
  onSelect: (page: IntroPage) => void;
  onReorder: (pages: IntroPage[]) => void;
}

export function IntroPageRail({
  pages,
  selectedId,
  disabled,
  onSelect,
  onReorder,
}: IntroPageRailProps) {
  const [draggedId, setDraggedId] = useState<number>();
  const [dropIndex, setDropIndex] = useState<number>();
  const insertionIndex = (list: HTMLOListElement, clientX: number) => {
    const cards = Array.from(list.children);
    const index = cards.findIndex((card) => {
      const rect = card.getBoundingClientRect();
      return clientX < rect.left + rect.width / 2;
    });
    return index < 0 ? pages.length : index;
  };
  const clearDrag = () => {
    setDraggedId(undefined);
    setDropIndex(undefined);
  };
  const move = (from: number, insertion: number) => {
    const to = insertion > from ? insertion - 1 : insertion;
    if (disabled || from < 0 || to < 0 || to >= pages.length || from === to)
      return;
    const next = [...pages];
    const [page] = next.splice(from, 1);
    next.splice(to, 0, page);
    onReorder(next);
  };
  return (
    <nav
      aria-label="Intro pages"
      className="min-w-0 shrink-0 border-t border-border pt-3"
    >
      <p className="mb-2 px-1 text-xs text-content-muted">
        Pages · Drag to reorder
      </p>
      <ol
        className="flex gap-2 overflow-x-auto overscroll-x-contain px-2 pb-1"
        onDragOver={(event) => {
          if (disabled || draggedId === undefined) return;
          event.preventDefault();
          event.dataTransfer.dropEffect = "move";
          setDropIndex(insertionIndex(event.currentTarget, event.clientX));
        }}
        onDragLeave={(event) => {
          if (
            !(event.relatedTarget instanceof Node) ||
            !event.currentTarget.contains(event.relatedTarget)
          ) {
            setDropIndex(undefined);
          }
        }}
        onDrop={(event) => {
          if (disabled || draggedId === undefined) return;
          event.preventDefault();
          move(
            pages.findIndex((item) => item.id === draggedId),
            insertionIndex(event.currentTarget, event.clientX),
          );
          clearDrag();
        }}
      >
        {pages.map((page, index) => (
          <li
            key={page.id}
            draggable={!disabled}
            onDragStart={(event) => {
              setDraggedId(page.id);
              setDropIndex(undefined);
              event.dataTransfer.setData("text/plain", String(page.id));
              event.dataTransfer.effectAllowed = "move";
            }}
            onDragEnd={clearDrag}
            className={`relative w-36 shrink-0 select-none rounded-lg border p-2 ${disabled ? "cursor-not-allowed" : draggedId !== undefined ? "cursor-grabbing" : "cursor-grab active:cursor-grabbing"} ${page.id === selectedId ? "border-brand-hover bg-brand/10" : "border-border bg-surface"}`}
          >
            {dropIndex === index && (
              <span
                aria-hidden="true"
                className="pointer-events-none absolute -left-1.5 inset-y-1 w-1 rounded-full bg-brand-hover shadow-[0_0_8px_currentColor] text-brand-hover"
              />
            )}
            {index === pages.length - 1 && dropIndex === pages.length && (
              <span
                aria-hidden="true"
                className="pointer-events-none absolute -right-1.5 inset-y-1 w-1 rounded-full bg-brand-hover shadow-[0_0_8px_currentColor] text-brand-hover"
              />
            )}
            <button
              type="button"
              title={introPageTypeLabels[page.type]}
              disabled={disabled}
              aria-current={page.id === selectedId ? "page" : undefined}
              onClick={() => onSelect(page)}
              className={`block w-full cursor-inherit rounded-lg text-left focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-hover disabled:opacity-50 ${draggedId === page.id ? "opacity-50" : ""}`}
            >
              <div className="mb-2">
                <IntroPageCanvas
                  type={page.type}
                  config={parseIntroPageConfig(page.config)}
                  imageUrl={page.previewPhotoUrl}
                  thumbnail
                />
              </div>
              <span className="block text-sm font-semibold">
                Page {String(index + 1).padStart(2, "0")}
              </span>
            </button>
          </li>
        ))}
      </ol>
    </nav>
  );
}
