import { useLayoutEffect, useRef, useState } from "react";
import { IntroPagePreview } from "@/features/journeys/components/IntroPagePreview";
import type { IntroPageType } from "@/features/journeys/types";
import type { IntroPageConfig } from "@/features/journeys/introPageConfig";

interface IntroPageCanvasProps {
  type: IntroPageType;
  config: IntroPageConfig;
  imageUrl?: string | null;
  thumbnail?: boolean;
}

// Use the same slide dimensions for the editor and thumbnails so text scales
// with the slide instead of reflowing inside a tiny thumbnail.
export function IntroPageCanvas({
  type,
  config,
  imageUrl,
  thumbnail = false,
}: IntroPageCanvasProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const [scale, setScale] = useState(0);
  useLayoutEffect(() => {
    const container = containerRef.current;
    if (!container) return;
    const measure = () => {
      const bounds = container.getBoundingClientRect();
      setScale(Math.min(bounds.width / 1280, bounds.height / 720));
    };
    measure();
    const observer = new ResizeObserver(([entry]) => {
      setScale(
        Math.min(
          entry.contentRect.width / 1280,
          entry.contentRect.height / 720,
        ),
      );
    });
    observer.observe(container);
    return () => observer.disconnect();
  }, []);

  return (
    <div
      ref={containerRef}
      aria-hidden={thumbnail || undefined}
      className={
        thumbnail
          ? "relative aspect-video w-full overflow-hidden rounded-lg"
          : "relative aspect-video min-h-0 w-full overflow-hidden xl:aspect-auto xl:h-full"
      }
    >
      <div
        className="absolute left-1/2 top-1/2 overflow-hidden rounded-xl shadow-xl"
        style={{
          width: 1280 * scale,
          height: 720 * scale,
          transform: "translate(-50%, -50%)",
        }}
      >
        <div
          inert={thumbnail}
          className={thumbnail ? "pointer-events-none" : undefined}
          style={{
            width: 1280,
            height: 720,
            transform: `scale(${scale})`,
            transformOrigin: "top left",
          }}
        >
          <IntroPagePreview
            type={type}
            config={config}
            imageUrl={imageUrl}
            fullScreen
          />
        </div>
      </div>
    </div>
  );
}
