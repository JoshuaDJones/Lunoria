import { Button } from "@/components/ui";
import type { ScenePlaythroughDetails } from "@/features/journeys/types";
import { formatDate } from "@/features/sceneplaythrough/utils/sceneDisplayUtils";

export function SceneEventLogs({
  logs,
  onBack,
}: {
  logs: ScenePlaythroughDetails["eventLogs"];
  onBack: () => void;
}) {
  return (
    <section className="space-y-5">
      <Button onClick={onBack} autoFocus>
        Back to options
      </Button>
      <h3 className="text-2xl font-semibold text-content">Event Logs</h3>
      {logs.length === 0 ? (
        <p className="text-content-muted">No events have been recorded.</p>
      ) : (
        <ol className="space-y-3">
          {logs.map((log) => (
            <li
              key={log.id}
              className="rounded-xl border border-border bg-surface/75 p-3"
            >
              <p className="break-words font-semibold text-content">
                {log.message}
              </p>
              <time
                dateTime={log.eventTime}
                className="mt-1 block text-xs text-content-muted"
              >
                {formatDate(log.eventTime)}
              </time>
            </li>
          ))}
        </ol>
      )}
    </section>
  );
}
