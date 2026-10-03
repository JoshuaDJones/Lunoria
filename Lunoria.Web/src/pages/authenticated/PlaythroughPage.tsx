import { useEffect, useRef, useState } from "react";
import { prepareSceneEntry } from "@/features/sceneplaythrough/utils/sceneEntry";
import SceneBackground from "@/features/sceneplaythrough/components/SceneBackground";
import { usePlaythroughEntrance } from "@/features/journeys/hooks/usePlaythroughEntrance";
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useConfirmDialog, useModalStack, useToast } from "@/app/providers";
import { Button, Card, Drawer } from "@/components/ui";
import {
  getPlaythrough,
  IntroPageViewer,
  ScenePlaythroughStatus,
  startScenePlaythrough,
  type PlaythroughDetails,
} from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";
import {
  getSceneStartInventory,
  resolveSceneStartInventory,
} from "@/features/journeys/api/journeysApi";
import type {
  SceneStartInventory,
  SceneInventoryResolutionInput,
  SceneStartResult,
} from "@/features/journeys/types";
import { SceneStartInventoryDialog } from "@/features/journeys/components/SceneStartInventoryDialog";
import {
  createPlaythroughJoinSession,
  PlaythroughJoinDialog,
  revokePlaythroughJoinSession,
} from "@/features/playthroughSession";

export function PlaythroughPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const toast = useToast();
  const { confirm } = useConfirmDialog();
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
  const [pendingInventory, setPendingInventory] = useState<{
    sceneId: number;
    inventory: SceneStartInventory;
  }>();
  const [inventoryBusy, setInventoryBusy] = useState(false);
  const [inventoryError, setInventoryError] = useState("");
  const [isCreatingJoinSession, setIsCreatingJoinSession] = useState(false);
  const [showLogs, setShowLogs] = useState(false);
  const hasHandledAutomaticIntro = useRef(false);
  const [cinematicEntry] = useState(() =>
    Boolean(
      (location.state as { showIntroPages?: boolean } | null)?.showIntroPages,
    ),
  );
  const entrance = usePlaythroughEntrance(
    String(playthroughId),
    playthrough?.playthrough.photoUrl || undefined,
    !isLoading && !error && Boolean(playthrough),
    cinematicEntry,
  );
  const showContent = entrance.contentVisible || Boolean(error);

  const navigateToScene = async (sceneId: number) => {
    setStartingSceneId(sceneId);
    try {
      await prepareSceneEntry(playthroughId, sceneId);
      navigate(
        `/series/${seriesId}/journeys/${journeyId}/playthroughs/${playthroughId}/scenes/${sceneId}`,
      );
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to open scene");
      setStartingSceneId(undefined);
      // Starting may have succeeded even if loading its details failed.
      void getPlaythrough(playthroughId)
        .then(setPlaythrough)
        .catch(() => {});
    }
  };

  const startScene = async (sceneId: number) => {
    setStartingSceneId(sceneId);

    try {
      const result = await startScenePlaythrough(playthroughId, sceneId);
      await handleStartResult(sceneId, result);
    } catch (requestError: unknown) {
      toast.error(getApiError(requestError).message, "Unable to start scene");
      setStartingSceneId(undefined);
    }
  };

  const handleStartResult = async (
    sceneId: number,
    result: SceneStartResult,
  ) => {
    if (result.started) {
      setPendingInventory(undefined);
      await navigateToScene(sceneId);
    } else if (result.pendingInventory) {
      setPendingInventory({ sceneId, inventory: result.pendingInventory });
      setStartingSceneId(undefined);
    } else {
      setPendingInventory(undefined);
      setStartingSceneId(undefined);
    }
  };

  const resolveInventory = async (input?: SceneInventoryResolutionInput) => {
    if (!pendingInventory || inventoryBusy) return;
    setInventoryBusy(true);
    setInventoryError("");
    try {
      const result = input
        ? await resolveSceneStartInventory(
            playthroughId,
            pendingInventory.sceneId,
            input,
          )
        : await getSceneStartInventory(playthroughId, pendingInventory.sceneId);
      await handleStartResult(pendingInventory.sceneId, result);
    } catch (requestError: unknown) {
      setInventoryError(getApiError(requestError).message);
    } finally {
      setInventoryBusy(false);
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
      .then(async (loadedPlaythrough) => {
        if (!isCurrent) return;
        setPlaythrough(loadedPlaythrough);
        const pendingScene = loadedPlaythrough.scenes.find(
          (scene) => scene.hasPendingInventory,
        );
        if (pendingScene) {
          const result = await getSceneStartInventory(
            playthroughId,
            pendingScene.id,
          );
          if (isCurrent && result.pendingInventory) {
            setPendingInventory({
              sceneId: pendingScene.id,
              inventory: result.pendingInventory,
            });
          }
        }
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
      !entrance.ready ||
      !shouldShowIntroPages ||
      hasHandledAutomaticIntro.current
    ) {
      return;
    }

    hasHandledAutomaticIntro.current = true;

    if (playthrough.introPages.length > 0) {
      const firstPage = [...playthrough.introPages].sort(
        (a, b) => a.sortOrder - b.sortOrder,
      )[0];
      void confirm({
        title: "Start with the intro pages?",
        message: "Would you like to watch the journey intro before choosing a scene? You can also open it later using Play Intro.",
        confirmLabel: "Yes, Play Intro",
        cancelLabel: "No, Skip Intro",
      }).then((accepted) => {
        if (accepted) setViewingIntroPageId(firstPage.id);
      });
    }

    navigate(location.pathname, { replace: true, state: null });
  }, [
    location.pathname,
    location.state,
    navigate,
    playthrough,
    entrance.ready,
    confirm,
  ]);

  return (
    <AppLayout
      sidebar={<></>}
      fixedViewport
      bottomPadding
      background={
        <SceneBackground
          key={playthrough?.playthrough.photoUrl || "fallback"}
          photoUrl={playthrough?.playthrough.photoUrl || undefined}
          visible={entrance.backgroundVisible || Boolean(error)}
        />
      }
    >
      {pendingInventory && entrance.ready && (
        <SceneStartInventoryDialog
          key={pendingInventory.inventory.resolutionToken}
          pending={pendingInventory.inventory}
          busy={inventoryBusy}
          error={inventoryError}
          onResolve={(input) => void resolveInventory(input)}
          onReload={() => void resolveInventory()}
        />
      )}
      {cinematicEntry && !entrance.ready && (
        <div
          aria-hidden="true"
          className="pointer-events-none fixed inset-0 z-20 flex items-center justify-center px-6"
        >
          <p
            className={`max-w-5xl text-center text-5xl text-content drop-shadow-lg transition-opacity duration-600 motion-reduce:transition-none sm:text-7xl ${entrance.titleVisible ? "opacity-100" : "opacity-0"}`}
          >
            {playthrough?.playthrough.name}
          </p>
        </div>
      )}
      <main
        inert={!showContent}
        aria-hidden={!showContent}
        className={`flex min-h-0 w-full flex-1 flex-col overflow-hidden transition-opacity duration-700 motion-reduce:transition-none ${showContent ? "opacity-100" : "opacity-0"}`}
      >
        <header className="flex shrink-0 flex-wrap items-end justify-between gap-5 p-6 sm:p-10">
          <div>
            <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
              {playthrough?.playthrough
                ? `${playthrough.playthrough.name}`
                : ""}
            </h1>
            <Link
              to={`/series/${seriesId}/journeys/${journeyId}/play`}
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to Play Hub
            </Link>
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
              <Button
                variant="secondary"
                onClick={() => setShowLogs(true)}
                aria-haspopup="dialog"
                aria-expanded={showLogs}
              >
                Logs
              </Button>
            </div>
          )}
        </header>
        {isLoading && (
          <p className="sr-only" role="status">
            Loading playthrough
          </p>
        )}

        {!isLoading && error && (
          <p className="px-10 text-danger" role="alert">
            {error}
          </p>
        )}

        {!isLoading && !error && playthrough && (
          <div className="grid min-h-0 flex-1 grid-rows-[minmax(0,1fr)] px-6 pb-4 sm:px-10">
            <section className="flex min-h-0 flex-col overflow-hidden rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px]">
              <h2 className="shrink-0 text-3xl font-semibold text-content">
                Scenes
              </h2>

              {playthrough.scenes.length === 0 ? (
                <p className="mt-5 text-content-muted">
                  This playthrough has no scenes.
                </p>
              ) : (
                <div
                  role="region"
                  aria-label="Scene cards"
                  tabIndex={0}
                  className="mt-5 min-h-0 flex-1 overflow-y-auto overscroll-contain pr-2"
                >
                  <div className="grid gap-5 [grid-template-columns:repeat(auto-fit,minmax(min(100%,28rem),1fr))]">
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
                            {scene.status !==
                              ScenePlaythroughStatus.NotStarted && (
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
                                <dt className="text-content-muted">
                                  Completed
                                </dt>
                                <dd>{formatDate(scene.endedAt)}</dd>
                              </div>
                            )}
                          </dl>

                          {scene.status ===
                            ScenePlaythroughStatus.NotStarted && (
                            <Button
                              variant="primary"
                              className="mt-5 self-end px-10"
                              disabled={startingSceneId !== undefined}
                              aria-busy={startingSceneId === scene.id}
                              leftIcon={
                                startingSceneId === scene.id ? (
                                  <SceneButtonSpinner />
                                ) : undefined
                              }
                              onClick={() => void startScene(scene.id)}
                            >
                              {scene.hasPendingInventory
                                ? "Resolve inventory"
                                : "Start"}
                            </Button>
                          )}

                          {scene.status ===
                            ScenePlaythroughStatus.InProgress && (
                            <Button
                              variant="primary"
                              className="mt-5 self-end px-10"
                              disabled={startingSceneId !== undefined}
                              aria-busy={startingSceneId === scene.id}
                              leftIcon={
                                startingSceneId === scene.id ? (
                                  <SceneButtonSpinner />
                                ) : undefined
                              }
                              onClick={() => void navigateToScene(scene.id)}
                            >
                              Resume
                            </Button>
                          )}
                        </div>
                      </Card>
                    ))}
                  </div>
                </div>
              )}
            </section>
          </div>
        )}
      </main>

      {playthrough && showLogs && (
        <Drawer title="Event Logs" onClose={() => setShowLogs(false)}>
          {playthrough.eventLogs.length === 0 ? (
            <p className="mt-5 text-content-muted">
              No events have been recorded.
            </p>
          ) : (
            <ol aria-label="Event log entries" className="space-y-3">
              {[...playthrough.eventLogs]
                .sort(
                  (a, b) =>
                    Date.parse(b.eventTime) - Date.parse(a.eventTime) ||
                    b.id - a.id,
                )
                .map((eventLog) => (
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
        </Drawer>
      )}

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

function SceneButtonSpinner() {
  return (
    <span
      aria-hidden="true"
      className="block h-4 w-4 animate-spin rounded-full border-2 border-current border-r-transparent motion-reduce:animate-none"
    />
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
