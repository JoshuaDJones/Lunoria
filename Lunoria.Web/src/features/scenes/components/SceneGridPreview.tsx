import { useEffect, useRef } from "react";

interface SceneGridPreviewProps {
  rows: number;
  columns: number;
  gridColor: string;
  background?: File;
  backgroundImageUrl?: string | null;
}

export function SceneGridPreview({
  rows,
  columns,
  gridColor,
  background,
  backgroundImageUrl,
}: SceneGridPreviewProps) {
  const imageRef = useRef<HTMLImageElement>(null);
  const validDimensions =
    Number.isInteger(rows) &&
    rows >= 1 &&
    rows <= 100 &&
    Number.isInteger(columns) &&
    columns >= 1 &&
    columns <= 100;

  useEffect(() => {
    const objectUrl = background ? URL.createObjectURL(background) : undefined;
    const image = imageRef.current;
    if (image) {
      const source = objectUrl ?? backgroundImageUrl;
      if (source) image.src = source;
      else image.removeAttribute("src");
    }
    return () => {
      if (objectUrl) URL.revokeObjectURL(objectUrl);
    };
  }, [background, backgroundImageUrl, validDimensions]);

  const lines = validDimensions
    ? [
        ...Array.from({ length: columns + 1 }, (_, x) => `M${x} 0V${rows}`),
        ...Array.from({ length: rows + 1 }, (_, y) => `M0 ${y}H${columns}`),
      ].join(" ")
    : "";

  return (
    <figure className="space-y-3 rounded-xl border border-border bg-surface p-3">
      <figcaption className="flex flex-wrap items-center justify-between gap-2 text-sm">
        <span className="font-semibold text-content">Grid preview</span>
        {validDimensions && (
          <span className="text-content-muted">
            {columns} columns × {rows} rows
          </span>
        )}
      </figcaption>
      {validDimensions ? (
        <div className="flex justify-center">
          <div
            role="img"
            aria-label={`Grid preview: ${columns} columns and ${rows} rows`}
            className="relative overflow-hidden bg-black"
            style={{
              width: `min(100%, ${(320 * columns) / rows}px)`,
              aspectRatio: `${columns} / ${rows}`,
            }}
          >
            <img
              ref={imageRef}
              alt=""
              hidden={!background && !backgroundImageUrl}
              draggable={false}
              className="pointer-events-none absolute inset-0 size-full object-cover"
            />
            <svg
              aria-hidden="true"
              viewBox={`0 0 ${columns} ${rows}`}
              className="pointer-events-none absolute inset-0 size-full"
            >
              <path
                d={lines}
                stroke={gridColor}
                strokeWidth={1}
                vectorEffect="non-scaling-stroke"
                fill="none"
              />
            </svg>
          </div>
        </div>
      ) : (
        <p className="py-6 text-center text-sm text-content-muted">
          Enter rows and columns between 1 and 100 to preview your grid.
        </p>
      )}
      <p className="text-xs text-content-muted">
        Preview uses square tiles. The opened grid fits the display.
      </p>
    </figure>
  );
}
