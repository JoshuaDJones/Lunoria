import { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlay } from "@fortawesome/free-solid-svg-icons";
import { Link, Navigate, useParams, useSearchParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useToast } from "@/app/providers";
import { Button } from "@/components/ui";
import { getJourney } from "@/features/journeys";
import {
  listPreviousPlaythroughs,
  resumePlaythrough,
  startPlaythrough,
  type JourneyPlaythrough,
} from "@/features/playthroughs";
import { getApiError } from "@/lib/apiClient";

export function PlayHubPage() {
  const toast = useToast();
  const { seriesId, journeyId } = useParams<{
    seriesId: string;
    journeyId: string;
  }>();
  const [journeyName, setJourneyName] = useState("");
  const [searchParams] = useSearchParams();
  const playthroughId = searchParams.get("playthroughId");
  const isViewingLogs = searchParams.get("view") === "logs";
  const numericJourneyId = Number(journeyId);
  const [playthroughs, setPlaythroughs] = useState<JourneyPlaythrough[]>([]);
  const [arePlaythroughsLoading, setArePlaythroughsLoading] = useState(true);
  const [playthroughsError, setPlaythroughsError] = useState("");
  const [starting, setStarting] = useState(false);
  const [resumingId, setResumingId] = useState<number>();

  useEffect(() => {
    const id = Number(journeyId);
    if (!Number.isInteger(id) || id <= 0) return;

    let isCurrent = true;
    void getJourney(id)
      .then((journey) => {
        if (isCurrent) setJourneyName(journey.name);
      })
      .catch(() => {
        if (isCurrent) setJourneyName("");
      });

    return () => {
      isCurrent = false;
    };
  }, [journeyId]);

  useEffect(() => {
    if (!Number.isInteger(numericJourneyId) || numericJourneyId <= 0) return;
    let isCurrent = true;
    void listPreviousPlaythroughs(numericJourneyId)
      .then((loaded) => {
        if (isCurrent) {
          setPlaythroughs(loaded);
          setPlaythroughsError("");
        }
      })
      .catch((requestError: unknown) => {
        if (isCurrent) setPlaythroughsError(getApiError(requestError).message);
      })
      .finally(() => {
        if (isCurrent) setArePlaythroughsLoading(false);
      });
    return () => { isCurrent = false; };
  }, [numericJourneyId]);

  if (!seriesId || !journeyId) {
    return <Navigate to="/journeys" replace />;
  }

  return (
    <AppLayout
      scrolling
      background={
        <div className="stone-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="w-full p-6 sm:p-10">
        <header className="mb-6">
          <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
            Play Hub{journeyName ? ` - ${journeyName}` : ""}
          </h1>
          <Link
            to={`/series/${seriesId}/journeys/${journeyId}`}
            className="text-sm text-content-secondary hover:text-brand-hover"
          >
            ← Back to Journey
          </Link>
        </header>

        <div className="grid gap-5 lg:grid-cols-[minmax(0,1fr)_18rem]">
          <section className="min-h-[30rem] rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px] sm:p-6">
            {isViewingLogs ? <>
            <h2 className="text-3xl font-semibold text-content">
              Playthrough {playthroughId ? `#${playthroughId} ` : ""}Logs
            </h2>
            <div className="mt-3 h-px w-full bg-border" />
            <p className="mt-5 text-content-secondary">
              No log entries have been recorded for this playthrough yet.
            </p>
            </> : <>
              <h2 className="text-3xl font-semibold text-content">Play Session</h2>
              <p className="mt-3 text-content-secondary">Start a new playthrough or resume an incomplete one from the pane.</p>
            </>}
          </section>

          <aside className="flex min-h-72 flex-col rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px]">
            <Button
              onClick={() => void (async () => {
                setStarting(true);
                try {
                  const started = await startPlaythrough(numericJourneyId);
                  setPlaythroughs((current) => [started, ...current]);
                  window.history.replaceState(null, "", `?playthroughId=${started.id}`);
                } catch (requestError) {
                  toast.error(getApiError(requestError).message, "Unable to start playthrough");
                } finally {
                  setStarting(false);
                }
              })()}
              disabled={starting}
              variant="add"
              size="lg"
              className="w-full py-4"
              leftIcon={<FontAwesomeIcon icon={faPlay} />}
            >
              {starting ? "Starting..." : "Start"}
            </Button>

            <h2 className="mt-6 text-2xl font-semibold text-content">Previous Playthroughs</h2>
            <div className="mt-2 h-px w-full bg-border" />

            {arePlaythroughsLoading && <p className="mt-4 text-sm text-content-secondary">Loading playthroughs...</p>}
            {!arePlaythroughsLoading && playthroughsError && <p className="mt-4 text-sm text-danger" role="alert">{playthroughsError}</p>}
            {!arePlaythroughsLoading && !playthroughsError && playthroughs.length === 0 && <p className="mt-4 text-sm text-content-muted">No previous playthroughs.</p>}

            <div className="mt-4 space-y-3">
              {playthroughs.map((playthrough) => (
                <article key={playthrough.id} className="rounded-xl border border-border bg-surface/75 p-3">
                  <div className="flex items-start justify-between gap-2">
                    <div><p className="font-semibold text-content">Playthrough #{playthrough.id}</p><p className="mt-0.5 text-xs text-content-secondary">{formatDate(playthrough.startedAt)} · Revision {playthrough.revisionNumber}</p></div>
                    <span className="text-xs font-semibold text-content-muted">{playthrough.completedAt ? "Complete" : playthrough.isActive ? "Active" : "Paused"}</span>
                  </div>
                  <div className="mt-3 flex flex-wrap justify-end gap-2">
                    <Button onClick={() => window.location.assign(`?playthroughId=${playthrough.id}&view=logs`)} size="sm" variant="secondary">View Logs</Button>
                    {!playthrough.completedAt && <Button onClick={() => void (async () => {
                      setResumingId(playthrough.id);
                      try {
                        await resumePlaythrough(numericJourneyId, playthrough.id);
                        window.location.assign(`?playthroughId=${playthrough.id}`);
                      } catch (requestError) {
                        toast.error(getApiError(requestError).message, "Unable to resume playthrough");
                        setResumingId(undefined);
                      }
                    })()} disabled={resumingId !== undefined} size="sm" variant="primary" leftIcon={<FontAwesomeIcon icon={faPlay} />}>{resumingId === playthrough.id ? "Resuming..." : "Resume"}</Button>}
                  </div>
                </article>
              ))}
            </div>
          </aside>
        </div>
      </main>
    </AppLayout>
  );
}

function formatDate(value: string): string {
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? "Unknown" : new Intl.DateTimeFormat(undefined, { dateStyle: "medium", timeStyle: "short" }).format(date);
}
