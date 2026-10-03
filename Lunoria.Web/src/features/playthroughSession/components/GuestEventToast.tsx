import { useEffect, useState } from "react";
import type { PublicPlaythroughEventLog } from "@/features/playthroughSession/types";

interface GuestEventToastProps {
  event: PublicPlaythroughEventLog;
  onDismiss: (id: number) => void;
}

export function GuestEventToast({ event, onDismiss }: GuestEventToastProps) {
  const [fading, setFading] = useState(false);

  useEffect(() => {
    const fadeTimer = window.setTimeout(() => setFading(true), 4000);
    const dismissTimer = window.setTimeout(() => onDismiss(event.id), 4300);
    return () => {
      window.clearTimeout(fadeTimer);
      window.clearTimeout(dismissTimer);
    };
  }, [event.id, onDismiss]);

  return (
    <div className="pointer-events-none fixed right-4 top-4 z-[10000] w-[calc(100%-2rem)] max-w-sm">
      <section
        role="status"
        aria-atomic="true"
        className={`pointer-events-auto flex items-start gap-3 rounded-xl border border-border border-l-4 border-l-brand bg-surface-raised p-4 text-content shadow-xl transition-opacity duration-300 motion-reduce:transition-none ${fading ? "opacity-0" : "opacity-100"}`}
      >
        <p className="min-w-0 flex-1 whitespace-pre-wrap break-words text-sm">
          {event.message}
        </p>
        <button
          type="button"
          aria-label="Dismiss event notification"
          className="shrink-0 rounded p-1 text-content-muted hover:text-content focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility"
          onClick={() => onDismiss(event.id)}
        >
          ×
        </button>
      </section>
    </div>
  );
}
