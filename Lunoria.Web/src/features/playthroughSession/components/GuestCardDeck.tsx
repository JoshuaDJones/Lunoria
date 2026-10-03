import { useEffect, useRef, type ReactNode } from "react";
import { CharacterThumbnail } from "@/features/playthroughSession/components/CharacterThumbnail";

interface GuestCardDeckProps {
  cards: {
    key: string;
    label: string;
    imageUrl?: string | null;
    content: ReactNode;
  }[];
  selectedIndex: number;
  onSelect: (key: string) => void;
}

export function GuestCardDeck({
  cards,
  selectedIndex,
  onSelect,
}: GuestCardDeckProps) {
  const viewportRef = useRef<HTMLDivElement>(null);
  const thumbnailStripRef = useRef<HTMLElement>(null);
  const indexRef = useRef(selectedIndex);
  const structureKey = cards.map((card) => card.key).join("|");

  useEffect(() => {
    indexRef.current = selectedIndex;
    const strip = thumbnailStripRef.current;
    const active = strip?.querySelector<HTMLElement>('[aria-current="true"]');
    if (strip && active) {
      const bounds = strip.getBoundingClientRect();
      const thumbnail = active.getBoundingClientRect();
      if (thumbnail.left < bounds.left)
        strip.scrollLeft -= bounds.left - thumbnail.left + 8;
      else if (thumbnail.right > bounds.right)
        strip.scrollLeft += thumbnail.right - bounds.right + 8;
    }
  }, [selectedIndex]);

  useEffect(() => {
    const viewport = viewportRef.current;
    if (!viewport) return;
    // The 24px gutters and 12px gap leave a glimpse of each neighboring card.
    const align = () =>
      viewport.scrollTo({
        left: indexRef.current * (viewport.clientWidth - 36),
        behavior: "instant",
      });
    align();
    const observer = new ResizeObserver(align);
    observer.observe(viewport);
    return () => observer.disconnect();
  }, [structureKey]);

  const navigate = (index: number) => {
    const viewport = viewportRef.current;
    if (!viewport || !cards[index]) return;
    viewport.scrollTo({
      left: index * (viewport.clientWidth - 36),
      behavior: window.matchMedia("(prefers-reduced-motion: reduce)").matches
        ? "instant"
        : "smooth",
    });
  };

  return (
    <>
      <div
        ref={viewportRef}
        tabIndex={0}
        onKeyDown={(event) => {
          if (event.target !== event.currentTarget) return;
          if (event.key === "ArrowLeft" || event.key === "ArrowRight") {
            event.preventDefault();
            navigate(selectedIndex + (event.key === "ArrowRight" ? 1 : -1));
          }
        }}
        role="region"
        aria-roledescription="carousel"
        aria-label="Playthrough cards"
        className="relative flex min-h-0 w-full flex-1 snap-x snap-mandatory gap-3 overflow-x-auto overflow-y-hidden overscroll-x-contain px-6 py-3 scrollbar-hide"
        onScroll={(event) => {
          const viewport = event.currentTarget;
          const step = viewport.clientWidth - 36;
          if (step <= 0) return;
          const index = Math.max(
            0,
            Math.min(cards.length - 1, Math.round(viewport.scrollLeft / step)),
          );
          if (index !== indexRef.current && cards[index]) {
            indexRef.current = index;
            onSelect(cards[index].key);
          }
        }}
      >
        {cards.map((card, index) => (
          <div
            key={card.key}
            role="group"
            aria-roledescription="slide"
            aria-label={`${card.label}, ${index + 1} of ${cards.length}`}
            aria-hidden={index !== selectedIndex}
            className="h-full w-full shrink-0 snap-center snap-always"
          >
            <div
              inert={index !== selectedIndex}
              className={`h-full origin-center overflow-y-auto overscroll-y-contain rounded-3xl border border-white/15 shadow-xl transition-[transform,opacity] duration-300 motion-reduce:transition-none scrollbar-hide ${index === selectedIndex ? "scale-100 opacity-100" : "scale-[0.96] opacity-60"}`}
            >
              {card.content}
            </div>
          </div>
        ))}
      </div>
      <nav
        ref={thumbnailStripRef}
        aria-label="Choose a character"
        className="shrink-0 overflow-x-auto overscroll-x-contain px-4 pt-1 pb-[max(0.5rem,env(safe-area-inset-bottom))] scrollbar-hide"
      >
        <div className="mx-auto flex w-max gap-2">
          {cards.map((card, index) => (
            <CharacterThumbnail
              key={card.key}
              name={card.label}
              imageUrl={card.imageUrl}
              selected={selectedIndex === index}
              onSelect={() => navigate(index)}
            />
          ))}
        </div>
      </nav>
    </>
  );
}
