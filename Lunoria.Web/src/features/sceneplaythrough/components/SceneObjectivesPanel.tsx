import { useId, useState } from "react";
import { Button } from "@/components/ui";
import type { SceneObjective } from "@/features/scenes/objectives";

export function SceneObjectivesPanel({
  objective,
  index,
  count,
  onSelect,
  busy = false,
  error,
  visible = true,
  aboveNavigation = false,
}: {
  objective?: SceneObjective | null;
  index?: number;
  count?: number;
  onSelect?: (index: number) => void;
  busy?: boolean;
  error?: string;
  visible?: boolean;
  aboveNavigation?: boolean;
}) {
  const [open, setOpen] = useState(true);
  const contentId = useId();
  if (!objective) return null;
  return (
    <aside
      aria-label="Scene objectives"
      inert={!visible}
      className={`fixed right-4 z-30 max-w-[calc(100vw-2rem)] transition-opacity duration-600 motion-reduce:transition-none ${aboveNavigation ? "bottom-[calc(5rem+env(safe-area-inset-bottom))]" : "bottom-4"} ${visible ? "opacity-100" : "pointer-events-none opacity-0"}`}
    >
      {!open ? (
        <Button
          aria-expanded={false}
          aria-controls={contentId}
          onClick={() => setOpen(true)}
        >
          Objectives
        </Button>
      ) : (
        <div className="w-80 max-w-full rounded-2xl border border-white/15 bg-black/80 p-4 text-white shadow-xl backdrop-blur-sm">
          <div className="mb-3 flex items-center justify-between gap-3">
            <h2 className="text-2xl">Objectives</h2>
            <Button
              size="sm"
              aria-label="Hide objectives"
              aria-expanded={true}
              aria-controls={contentId}
              onClick={() => setOpen(false)}
            >
              ×
            </Button>
          </div>
          <div
            id={contentId}
            className="max-h-[40dvh] overflow-y-auto"
            aria-live="polite"
            aria-atomic="true"
          >
            <ul className="list-disc space-y-2 break-words pl-5">
              {objective.points.map((text, pointIndex) => (
                <li key={pointIndex} className="whitespace-pre-wrap">
                  {text}
                </li>
              ))}
            </ul>
          </div>
          {onSelect && index !== undefined && count !== undefined && (
            <div className="mt-4 flex items-center justify-between gap-3">
              <Button
                size="sm"
                aria-label="Previous objective set"
                disabled={busy || index === 0}
                onClick={() => onSelect(index - 1)}
              >
                ←
              </Button>
              <span className="text-sm" role="status">
                {busy ? (
                  <span
                    aria-label="Saving objectives"
                    className="block h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent"
                  />
                ) : (
                  `${index + 1} / ${count}`
                )}
              </span>
              <Button
                size="sm"
                aria-label="Next objective set"
                disabled={busy || index >= count - 1}
                onClick={() => onSelect(index + 1)}
              >
                →
              </Button>
            </div>
          )}
          {error && (
            <p role="alert" className="mt-3 text-sm text-red-300">
              {error}
            </p>
          )}
        </div>
      )}
    </aside>
  );
}
