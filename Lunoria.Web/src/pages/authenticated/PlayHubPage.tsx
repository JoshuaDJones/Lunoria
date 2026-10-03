import { useEffect, useState, type ReactNode } from "react";
import { Link, Navigate, useNavigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useConfirmDialog, useToast } from "@/app/providers";
import { ApiLoadError, Button } from "@/components/ui";
import {
  getJourney,
  listJourneyPlaythroughs,
  startJourneyPlaythrough,
  type PlaythroughSummary,
} from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";

export function PlayHubPage() {
  const navigate = useNavigate();
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const { seriesId, journeyId: journeyIdParam } = useParams<{
    seriesId: string;
    journeyId: string;
  }>();
  const journeyId = Number(journeyIdParam);
  const [journeyName, setJourneyName] = useState("");
  const [playthroughs, setPlaythroughs] = useState<PlaythroughSummary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isStarting, setIsStarting] = useState(false);
  const [error, setError] = useState("");
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    if (!Number.isInteger(journeyId) || journeyId <= 0) return;

    let isCurrent = true;
    setIsLoading(true);
    setError("");

    void Promise.all([
      getJourney(journeyId),
      listJourneyPlaythroughs(journeyId),
    ])
      .then(([journey, loadedPlaythroughs]) => {
        if (!isCurrent) return;
        setJourneyName(journey.name);
        setPlaythroughs(loadedPlaythroughs);
      })
      .catch((requestError: unknown) => {
        if (isCurrent) setError(getApiError(requestError).message);
      })
      .finally(() => {
        if (isCurrent) setIsLoading(false);
      });

    return () => {
      isCurrent = false;
    };
  }, [journeyId, reloadKey]);

  if (!seriesId || !Number.isInteger(journeyId) || journeyId <= 0) {
    return <Navigate to="/journeys" replace />;
  }

  const inProgress = playthroughs.filter(
    (playthrough) => !playthrough.isCompleted,
  );
  const completed = playthroughs.filter(
    (playthrough) => playthrough.isCompleted,
  );

  const navigateToPlaythrough = (
    playthroughId: number,
    showIntroPages = false,
  ) => {
    navigate(
      `/series/${seriesId}/journeys/${journeyId}/playthroughs/${playthroughId}`,
      showIntroPages ? { state: { showIntroPages: true } } : undefined,
    );
  };

  const startNewPlaythrough = async () => {
    const confirmed = await confirm({
      title: "Start a new playthrough?",
      message:
        "Are you sure you want to start a new playthrough? All unfinished playthroughs will be marked as completed.",
      confirmLabel: "Yes",
      cancelLabel: "No",
    });

    if (!confirmed) return;

    setIsStarting(true);

    try {
      const startedPlaythrough = await startJourneyPlaythrough(journeyId);
      toast.success("A new playthrough was started.");
      navigateToPlaythrough(startedPlaythrough.id, true);
    } catch (requestError) {
      toast.error(
        getApiError(requestError).message,
        "Unable to start playthrough",
      );
    } finally {
      setIsStarting(false);
    }
  };

  return (
    <AppLayout
      fixedViewport
      bottomPadding
      background={
        <div className="stone-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="flex min-h-0 w-full flex-1 flex-col px-6 pt-6 pb-4 sm:px-10 sm:pt-10">
        <header className="mb-6 flex shrink-0 flex-wrap items-end justify-between gap-5">
          <div>
            <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
              Play Hub{journeyName ? ` - ${journeyName}` : ""}
            </h1>
            <Link
              to={`/series/${seriesId}/journeys/${journeyId}`}
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to Journey
            </Link>
          </div>
        </header>

        <section className="flex min-h-0 flex-1 flex-col overflow-hidden rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px] sm:p-6">
          {isLoading && (
            <p className="text-content-secondary" role="status">
              Loading playthroughs...
            </p>
          )}

          {!isLoading && error && (
            <ApiLoadError
              error={error}
              onRetry={() => setReloadKey((value) => value + 1)}
            />
          )}

          {!isLoading && !error && (
            <div className="grid min-h-0 flex-1 grid-rows-[auto_minmax(0,1fr)] gap-8 xl:grid-cols-2 xl:grid-rows-1">
              <PlaythroughSection
                title="In Progress"
                newPlaythroughAction={
                  <Button
                    variant="add"
                    disabled={isStarting}
                    aria-busy={isStarting}
                    leftIcon={
                      isStarting ? (
                        <span
                          aria-hidden="true"
                          className="block h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent motion-reduce:animate-none"
                        />
                      ) : undefined
                    }
                    onClick={() => void startNewPlaythrough()}
                  >
                    Play New
                  </Button>
                }
                busy={isStarting}
                emptyMessage="No playthroughs are currently in progress."
                playthroughs={inProgress}
                onResume={navigateToPlaythrough}
              />
              <PlaythroughSection
                title="Completed"
                scrollItems
                emptyMessage="No playthroughs have been completed yet."
                playthroughs={completed}
                onResume={navigateToPlaythrough}
              />
            </div>
          )}
        </section>
      </main>
    </AppLayout>
  );
}

interface PlaythroughSectionProps {
  scrollItems?: boolean;
  newPlaythroughAction?: ReactNode;
  busy?: boolean;
  title: string;
  emptyMessage: string;
  playthroughs: PlaythroughSummary[];
  onResume: (playthroughId: number) => void;
}

function PlaythroughSection({
  scrollItems = false,
  newPlaythroughAction,
  busy = false,
  title,
  emptyMessage,
  playthroughs,
  onResume,
}: PlaythroughSectionProps) {
  return (
    <section className="flex min-h-0 min-w-0 flex-col">
      <div className="flex shrink-0 items-center justify-between gap-3">
        <h2 className="text-3xl font-semibold text-content">{title}</h2>
        <span className="rounded-full bg-surface px-3 py-1 text-sm text-content-secondary">
          {playthroughs.length}
        </span>
      </div>
      <div className="mt-3 h-px w-full shrink-0 bg-border" />

      {playthroughs.length === 0 ? (
        <div className="mt-5 rounded-xl border border-border bg-surface/50 p-5 text-content-muted">
          <p>{emptyMessage}</p>
          {newPlaythroughAction && (
            <div className="mt-4 flex justify-end">{newPlaythroughAction}</div>
          )}
        </div>
      ) : (
        <div
          className={`mt-5 space-y-4 ${scrollItems ? "min-h-0 flex-1 overflow-y-auto overscroll-y-contain pr-2" : ""}`}
          tabIndex={scrollItems ? 0 : undefined}
          role={scrollItems ? "region" : undefined}
          aria-label={scrollItems ? "Completed playthroughs" : undefined}
        >
          {playthroughs.map((playthrough) => (
            <article
              key={playthrough.id}
              className="rounded-2xl border border-border bg-surface/75 p-4"
            >
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div>
                  <h3 className="text-xl font-semibold text-content">
                    {playthrough.name}
                  </h3>
                  {playthrough.description && (
                    <p className="mt-1 text-sm text-content-secondary">
                      {playthrough.description}
                    </p>
                  )}
                </div>
                <span className="rounded-full border border-border px-3 py-1 text-xs font-semibold text-content-secondary">
                  {playthrough.isCompleted ? "Completed" : "In progress"}
                </span>
              </div>

              <dl className="mt-4 grid gap-2 text-sm text-content-secondary sm:grid-cols-2">
                <div>
                  <dt className="text-content-muted">Started</dt>
                  <dd>{formatDate(playthrough.startedAt)}</dd>
                </div>
                {playthrough.completedAt && (
                  <div>
                    <dt className="text-content-muted">Completed</dt>
                    <dd>{formatDate(playthrough.completedAt)}</dd>
                  </div>
                )}
                {!playthrough.completedAt && (
                  <div className="flex flex-1 flex-wrap items-center justify-end gap-3">
                    <Button
                      variant="primary"
                      disabled={busy}
                      onClick={() => onResume(playthrough.id)}
                    >
                      Resume
                    </Button>
                    {newPlaythroughAction}
                  </div>
                )}
              </dl>
            </article>
          ))}
        </div>
      )}
    </section>
  );
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}
