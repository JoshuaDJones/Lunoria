import { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useModalStack, useToast } from "@/app/providers";
import { Button, Card } from "@/components/ui";
import {
  getPlaythrough,
  IntroPageViewer,
  ScenePlaythroughStatus,
  startScenePlaythrough,
  type PlaythroughDetails,
} from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";
import {
  createPlaythroughJoinSession,
  PlaythroughJoinDialog,
  revokePlaythroughJoinSession,
} from "@/features/playthroughSession";

export function PlaythroughPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const toast = useToast();
  const modalStack = useModalStack();
  const {
    seriesId,
    journeyId,
    playthroughId: playthroughIdParam,
  } = useParams<{
    seriesId: string;
    journeyId: string;
    playthroughId: string;
  }>();
  const playthroughId = Number(playthroughIdParam);
  const [playthrough, setPlaythrough] = useState<PlaythroughDetails>();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [viewingIntroPageId, setViewingIntroPageId] = useState<number>();
  const [startingSceneId, setStartingSceneId] = useState<number>();
  const [isCreatingJoinSession, setIsCreatingJoinSession] = useState(false);
  const hasHandledAutomaticIntro = useRef(false);

  const navigateToScene = (sceneId: number) => {
    navigate(
      `/series/${seriesId}/journeys/${journeyId}/playthroughs/${playthroughId}/scenes/${sceneId}`,
    );
  };

  const startScene = async (sceneId: number) => {
    setStartingSceneId(sceneId);

    try {
      await startScenePlaythrough(playthroughId, sceneId);
      navigateToScene(sceneId);
    } catch (requestError: unknown) {
      toast.error(
        getApiError(requestError).message,
        "Unable to start scene",
      );
      setStartingSceneId(undefined);
    }
  };

  const openJoinDialog = async () => {
    setIsCreatingJoinSession(true);
    try {
      const session = await createPlaythroughJoinSession(playthroughId);
      const joinUrl = new URL(
        `/join/${encodeURIComponent(session.token)}`,
        window.location.origin,
      ).toString();

      modalStack.push({
        title: "Join Playthrough",
        placement: "center",
        content: (
          <PlaythroughJoinDialog
            joinUrl={joinUrl}
            expiresAt={session.expiresAt}
            onRevoke={async () => {
              try {
                await revokePlaythroughJoinSession(playthroughId);
                modalStack.dismissAll();
                toast.success("The guest session was closed.");
              } catch (requestError: unknown) {
                toast.error(
                  getApiError(requestError).message,
                  "Unable to close guest session",
                );
              }
            }}
          />
        ),
      });
    } catch (requestError: unknown) {
      toast.error(
        getApiError(requestError).message,
        "Unable to create guest session",
      );
    } finally {
      setIsCreatingJoinSession(false);
    }
  };

  useEffect(() => {
    if (!Number.isInteger(playthroughId) || playthroughId <= 0) {
      setError("The playthrough ID is invalid.");
      setIsLoading(false);
      return;
    }

    let isCurrent = true;
    setIsLoading(true);
    setError("");

    void getPlaythrough(playthroughId)
      .then((loadedPlaythrough) => {
        if (isCurrent) setPlaythrough(loadedPlaythrough);
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
  }, [playthroughId]);

  useEffect(() => {
    const shouldShowIntroPages = Boolean(
      (location.state as { showIntroPages?: boolean } | null)?.showIntroPages,
    );

    if (
      !playthrough ||
      !shouldShowIntroPages ||
      hasHandledAutomaticIntro.current
    ) {
      return;
    }

    hasHandledAutomaticIntro.current = true;

    if (playthrough.introPages.length > 0) {
      setViewingIntroPageId(playthrough.introPages[0].id);
    }

    navigate(location.pathname, { replace: true, state: null });
  }, [location.pathname, location.state, navigate, playthrough]);

  return (
    <AppLayout
      sidebar={<></>}
      scrolling
      bottomPadding={false}
      background={
        <div className="valley-village-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="w-full flex-1">
        <header className="p-10 flex flex-wrap items-end justify-between gap-5">
          <div>
            <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
              {playthrough?.playthrough
                ? `${playthrough.playthrough.name}`
                : ""}
            </h1>
          </div>
          {playthrough && (
            <div className="flex flex-wrap gap-3">
              {playthrough.introPages.length > 0 && (
                <Button
                  variant="primary"
                  onClick={() =>
                    setViewingIntroPageId(playthrough.introPages[0].id)
                  }
                >
                  Play Intro
                </Button>
              )}
              <Button
                variant="utility"
                disabled={isCreatingJoinSession}
                onClick={() => void openJoinDialog()}
              >
                {isCreatingJoinSession ? "Creating..." : "Join"}
              </Button>
            </div>
          )}
        </header>
        {isLoading && (
          <p className="px-10 text-content">Loading playthrough...</p>
        )}

        {!isLoading && error && (
          <p className="px-10 text-danger" role="alert">
            {error}
          </p>
        )}

        {!isLoading && !error && playthrough && (
          <div className="grid gap-6 px-6 pb-10 sm:px-10 lg:grid-cols-[minmax(0,4fr)_minmax(14rem,1fr)]">
            <section className="rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px]">
              <h2 className="text-3xl font-semibold text-content">Scenes</h2>

              {playthrough.scenes.length === 0 ? (
                <p className="mt-5 text-content-muted">
                  This playthrough has no scenes.
                </p>
              ) : (
                <div className="mt-5 grid gap-5 [grid-template-columns:repeat(auto-fit,minmax(min(100%,28rem),1fr))]">
                  {playthrough.scenes.map((scene) => (
                    <Card key={scene.id} className="flex flex-col">
                      {scene.photoUrl && (
                        <img
                          src={scene.photoUrl}
                          alt=""
                          className="h-64 w-full object-cover"
                        />
                      )}

                      <div className="flex flex-1 flex-col p-4">
                        <div className="flex flex-wrap items-start justify-between gap-3">
                          <h3 className="text-2xl font-semibold text-content">
                            {scene.name}
                          </h3>
                          <span className="rounded-full border border-border px-3 py-1 text-xs font-semibold text-content-secondary">
                            {getSceneStatusLabel(scene.status)}
                          </span>
                        </div>

                        {scene.description && (
                          <p className="mt-2 text-content-secondary">
                            {scene.description}
                          </p>
                        )}

                        <dl className="mt-auto grid gap-3 pt-5 text-sm text-content-secondary sm:grid-cols-2">
                          {scene.status !== ScenePlaythroughStatus.NotStarted && (
                            <div>
                              <dt className="text-content-muted">Round</dt>
                              <dd>{scene.roundNumber}</dd>
                            </div>
                          )}
                          {scene.startedAt && (
                            <div>
                              <dt className="text-content-muted">Started</dt>
                              <dd>{formatDate(scene.startedAt)}</dd>
                            </div>
                          )}
                          {scene.endedAt && (
                            <div>
                              <dt className="text-content-muted">Completed</dt>
                              <dd>{formatDate(scene.endedAt)}</dd>
                            </div>
                          )}
                        </dl>

                        {scene.status === ScenePlaythroughStatus.NotStarted && (
                          <Button
                            variant="primary"
                            className="mt-5 self-end px-10"
                            disabled={startingSceneId !== undefined}
                            onClick={() => void startScene(scene.id)}
                          >
                            {startingSceneId === scene.id
                              ? "Starting..."
                              : "Start"}
                          </Button>
                        )}

                        {scene.status === ScenePlaythroughStatus.InProgress && (
                          <Button
                            variant="primary"
                            className="mt-5 self-end px-10"
                            onClick={() => navigateToScene(scene.id)}
                          >
                            Resume
                          </Button>
                        )}
                      </div>
                    </Card>
                  ))}
                </div>
              )}
            </section>

            <aside className="self-start rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px] lg:sticky lg:top-6 lg:max-h-[calc(100vh-3rem)] lg:overflow-y-auto">
              <h2 className="text-3xl font-semibold text-content">
                Event Log
              </h2>

              {playthrough.eventLogs.length === 0 ? (
                <p className="mt-5 text-content-muted">
                  No events have been recorded.
                </p>
              ) : (
                <ol className="mt-5 space-y-3">
                  {playthrough.eventLogs.map((eventLog) => (
                    <li
                      key={eventLog.id}
                      className="rounded-xl border border-border bg-surface/75 p-3"
                    >
                      <p className="font-semibold text-content">
                        {eventLog.message}
                      </p>
                      <time className="mt-1 block text-xs text-content-muted">
                        {formatDate(eventLog.eventTime)}
                      </time>
                    </li>
                  ))}
                </ol>
              )}
            </aside>
          </div>
        )}
      </main>

      {playthrough && viewingIntroPageId !== undefined && (
        <IntroPageViewer
          pages={playthrough.introPages}
          initialPageId={viewingIntroPageId}
          title={`${playthrough.playthrough.name} Intro Pages`}
          onClose={() => setViewingIntroPageId(undefined)}
        />
      )}
    </AppLayout>
  );
}

function getSceneStatusLabel(status: ScenePlaythroughStatus) {
  switch (status) {
    case ScenePlaythroughStatus.InProgress:
      return "In progress";
    case ScenePlaythroughStatus.Completed:
      return "Completed";
    default:
      return "Not started";
  }
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}
