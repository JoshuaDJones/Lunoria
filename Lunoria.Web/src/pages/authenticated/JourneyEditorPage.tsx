import { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faBookOpen,
  faPlay,
  faPlus,
  faSort,
  faUsers,
} from "@fortawesome/free-solid-svg-icons";
import { Link, Navigate, useNavigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useConfirmDialog, useToast } from "@/app/providers";
import { ApiLoadError, Button, Drawer } from "@/components/ui";
import {
  getJourney,
  JourneyCharacterPicker,
  type Journey,
} from "@/features/journeys";
import {
  deleteScene,
  listScenes,
  reorderScenes,
  SceneEditorForm,
  SceneGrid,
  SceneEventManager,
  SceneChestManager,
  SceneCharacterManager,
  SceneOrderEditor,
  type Scene,
} from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";
import { SceneObjectiveEditor } from "@/features/scenes/components/SceneObjectiveEditor";

export function JourneyEditorPage() {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const navigate = useNavigate();
  const { seriesId, journeyId: journeyIdParam } = useParams<{
    seriesId: string;
    journeyId: string;
  }>();
  const journeyId = Number(journeyIdParam);
  const [journey, setJourney] = useState<Journey>();
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [scenes, setScenes] = useState<Scene[]>([]);
  const [scenesError, setScenesError] = useState("");
  const [areScenesLoading, setAreScenesLoading] = useState(true);
  const [editingScene, setEditingScene] = useState<Scene | null | undefined>();
  const [isOrderingScenes, setIsOrderingScenes] = useState(false);
  const [isSavingSceneOrder, setIsSavingSceneOrder] = useState(false);
  const [isManagingCharacters, setIsManagingCharacters] = useState(false);
  const [playersBusy, setPlayersBusy] = useState(false);
  const [playersDirty, setPlayersDirty] = useState(false);
  const [eventsScene, setEventsScene] = useState<Scene>();
  const [objectivesScene, setObjectivesScene] = useState<Scene>();
  const [objectivesBusy, setObjectivesBusy] = useState(false);
  const [chestsScene, setChestsScene] = useState<Scene>();
  const [charactersScene, setCharactersScene] = useState<Scene>();
  const [scenesReloadKey, setScenesReloadKey] = useState(0);

  useEffect(() => {
    if (!isManagingCharacters || (!playersDirty && !playersBusy)) return;
    const preventUnload = (event: BeforeUnloadEvent) => {
      event.preventDefault();
      event.returnValue = "";
    };
    window.addEventListener("beforeunload", preventUnload);
    return () => window.removeEventListener("beforeunload", preventUnload);
  }, [isManagingCharacters, playersDirty, playersBusy]);

  const closePlayers = async () => {
    if (playersBusy) return;
    if (
      playersDirty &&
      !(await confirm({
        title: "Discard player edits?",
        message: "Your unsaved player changes will be lost.",
        confirmLabel: "Discard changes",
        variant: "danger",
      }))
    )
      return;
    setPlayersDirty(false);
    setIsManagingCharacters(false);
  };

  const loadJourney = async () => {
    setIsLoading(true);
    setError("");

    try {
      setJourney(await getJourney(journeyId));
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  const retryScenes = async () => {
    setAreScenesLoading(true);
    setScenesError("");

    try {
      setScenes(await listScenes({ journeyId }));
    } catch (requestError) {
      setScenesError(getApiError(requestError).message);
    } finally {
      setAreScenesLoading(false);
    }
  };

  useEffect(() => {
    if (!Number.isInteger(journeyId) || journeyId <= 0) return;

    let isCurrent = true;

    void getJourney(journeyId)
      .then((loadedJourney) => {
        if (isCurrent) {
          setJourney(loadedJourney);
          setError("");
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
  }, [journeyId]);

  useEffect(() => {
    if (!Number.isInteger(journeyId) || journeyId <= 0) return;

    let isCurrent = true;

    void listScenes({ journeyId })
      .then((loadedScenes) => {
        if (isCurrent) {
          setScenes(loadedScenes);
          setScenesError("");
        }
      })
      .catch((requestError: unknown) => {
        if (isCurrent) setScenesError(getApiError(requestError).message);
      })
      .finally(() => {
        if (isCurrent) setAreScenesLoading(false);
      });

    return () => {
      isCurrent = false;
    };
  }, [journeyId, scenesReloadKey]);

  if (!Number.isInteger(journeyId) || journeyId <= 0) {
    return <Navigate to="/home" replace />;
  }

  const openConfirmDeleteScene = async (scene: Scene) => {
    const confirmed = await confirm({
      title: `Delete scene "${scene.name}"?`,
      message: "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteScene(scene.id, journeyId);
      setAreScenesLoading(true);
      setScenesReloadKey((value) => value + 1);
      toast.success(`Scene "${scene.name}" was deleted.`);
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to delete scene");
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
        <header className="mb-6 flex shrink-0 flex-wrap items-center justify-between gap-4">
          <div className="min-w-0">
            <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
              {journey?.name ?? (isLoading ? "Loading journey..." : "Journey")}
            </h1>
            <Link
              to={`/series/${seriesId}/journeys`}
              className="text-sm text-content-secondary hover:text-brand-hover"
            >
              ← Back to Journeys
            </Link>
          </div>
          {!isLoading && !error && journey && (
            <div className="ml-auto flex items-center gap-2 sm:gap-3">
              <Button
                onClick={() =>
                  navigate(
                    `/series/${seriesId}/journeys/${journeyId}/intro-pages`,
                  )
                }
                variant="secondary"
                className="h-11 border-border bg-surface/90 text-content-secondary hover:border-content-muted hover:bg-surface hover:text-content"
                leftIcon={<FontAwesomeIcon icon={faBookOpen} />}
              >
                Intro Pages
              </Button>
              <Button
                onClick={() => setIsManagingCharacters(true)}
                variant="secondary"
                className="h-11 border-border bg-surface/90 text-content-secondary hover:border-content-muted hover:bg-surface hover:text-content"
                leftIcon={<FontAwesomeIcon icon={faUsers} />}
              >
                Players
              </Button>
              <Button
                onClick={() =>
                  navigate(`/series/${seriesId}/journeys/${journeyId}/play`)
                }
                variant="add"
                className="ml-2 h-11 sm:ml-3"
                leftIcon={<FontAwesomeIcon icon={faPlay} />}
              >
                Play
              </Button>
            </div>
          )}
        </header>

        {!isLoading && error && (
          <ApiLoadError error={error} onRetry={loadJourney} />
        )}

        {(isLoading || (!error && journey)) && (
          <div className="flex min-h-0 flex-1 flex-col">
            <section className="flex min-h-0 flex-1 flex-col overflow-hidden rounded-3xl bg-surface/65 p-4 backdrop-blur-[2px] sm:p-6">
              <div className="flex shrink-0 flex-wrap items-center justify-between gap-4">
                <h2 className="text-4xl text-content">Scenes</h2>
                <div className="flex flex-wrap gap-3">
                  <Button
                    onClick={() => setIsOrderingScenes(true)}
                    disabled={
                      isLoading ||
                      areScenesLoading ||
                      Boolean(scenesError) ||
                      scenes.length < 2
                    }
                    variant="secondary"
                    className="h-11 border-border bg-surface/90 text-content-secondary hover:border-content-muted hover:bg-surface hover:text-content"
                    leftIcon={<FontAwesomeIcon icon={faSort} />}
                  >
                    Scene Order
                  </Button>
                  <Button
                    onClick={() => setEditingScene(null)}
                    disabled={isLoading || !journey}
                    variant="secondary"
                    className="h-11 border-border bg-surface/90 text-content hover:border-content-muted hover:bg-surface hover:text-content"
                    leftIcon={<FontAwesomeIcon icon={faPlus} />}
                  >
                    Add Scene
                  </Button>
                </div>
              </div>

              <div
                className="mt-6 min-h-0 flex-1 overflow-y-auto overscroll-contain pr-2"
                role="region"
                aria-label="Scene list"
                tabIndex={0}
              >
                {(isLoading || areScenesLoading) && (
                  <div
                    role="status"
                    className="flex min-h-32 items-center justify-center rounded-xl border border-border bg-surface/50 p-5 text-content-secondary"
                  >
                    <span
                      aria-hidden="true"
                      className="h-7 w-7 animate-spin rounded-full border-2 border-current border-r-transparent motion-reduce:animate-none"
                    />
                    <span className="sr-only">Loading scenes...</span>
                  </div>
                )}

                {!isLoading && !areScenesLoading && scenesError && (
                  <ApiLoadError error={scenesError} onRetry={retryScenes} />
                )}

                {!isLoading &&
                  !areScenesLoading &&
                  !scenesError &&
                  scenes.length === 0 && (
                    <div className="rounded-xl border border-border bg-surface/60 p-8 text-center">
                      <h3 className="text-2xl font-semibold text-content">
                        No scenes yet
                      </h3>
                      <p className="mt-2 text-content-muted">
                        Add your first scene to get started.
                      </p>
                    </div>
                  )}

                {!isLoading &&
                  !areScenesLoading &&
                  !scenesError &&
                  scenes.length > 0 && (
                    <SceneGrid
                      scenes={scenes}
                      className="sm:grid-cols-1 xl:grid-cols-2"
                      onViewEvents={setEventsScene}
                      onViewObjectives={setObjectivesScene}
                      onViewChests={setChestsScene}
                      onViewCharacters={setCharactersScene}
                      onEdit={setEditingScene}
                      onDelete={(scene) => void openConfirmDeleteScene(scene)}
                      onViewDialogs={(scene) =>
                        navigate(
                          `/series/${seriesId}/journeys/${journeyId}/scenes/${scene.id}/dialogs`,
                        )
                      }
                    />
                  )}
              </div>
            </section>
          </div>
        )}
      </main>

      {editingScene !== undefined && (
        <Drawer
          title={editingScene ? "Edit scene" : "Create scene"}
          onClose={() => setEditingScene(undefined)}
        >
          <SceneEditorForm
            journeyId={journeyId}
            scene={editingScene}
            onSaved={(sceneName, editing) => {
              toast.success(
                `Scene "${sceneName}" was ${editing ? "updated" : "created"}.`,
              );
              setEditingScene(undefined);
              setAreScenesLoading(true);
              setScenesReloadKey((value) => value + 1);
            }}
          />
        </Drawer>
      )}

      {objectivesScene && (
        <Drawer
          title={`${objectivesScene.name} Objectives`}
          onClose={() => {
            if (!objectivesBusy) setObjectivesScene(undefined);
          }}
        >
          <SceneObjectiveEditor
            key={objectivesScene.id}
            sceneId={objectivesScene.id}
            onBusyChange={setObjectivesBusy}
          />
        </Drawer>
      )}
      {eventsScene && (
        <Drawer
          title={`${eventsScene.name} Events`}
          onClose={() => setEventsScene(undefined)}
        >
          <SceneEventManager
            key={eventsScene.id}
            scene={eventsScene}
            journeyCharacters={journey?.journeyCharacters ?? []}
          />
        </Drawer>
      )}

      {chestsScene && (
        <Drawer
          title={`${chestsScene.name} Chests`}
          onClose={() => setChestsScene(undefined)}
        >
          <SceneChestManager key={chestsScene.id} scene={chestsScene} />
        </Drawer>
      )}

      {charactersScene && (
        <Drawer
          title="Scene Characters"
          onClose={() => setCharactersScene(undefined)}
        >
          <SceneCharacterManager
            key={charactersScene.id}
            scene={charactersScene}
          />
        </Drawer>
      )}

      {isOrderingScenes && (
        <Drawer
          title="Scene Order"
          onClose={() => setIsOrderingScenes(false)}
          closeDisabled={isSavingSceneOrder}
        >
          <SceneOrderEditor
            scenes={scenes}
            onBusyChange={setIsSavingSceneOrder}
            onCancel={() => setIsOrderingScenes(false)}
            onSave={async (orderedScenes) => {
              await reorderScenes(
                journeyId,
                orderedScenes.map((scene, sortOrder) => ({
                  id: scene.id,
                  sortOrder,
                })),
              );
              setScenes(
                orderedScenes.map((scene, sortOrder) => ({
                  ...scene,
                  sortOrder,
                })),
              );
              setIsOrderingScenes(false);
              toast.success("Scene order was updated.");
            }}
          />
        </Drawer>
      )}

      {isManagingCharacters && journey && (
        <Drawer
          title="Journey Players"
          closeDisabled={playersBusy}
          onClose={() => void closePlayers()}
        >
          <JourneyCharacterPicker
            journeyId={journeyId}
            journeyCharacters={journey.journeyCharacters ?? []}
            onBusyChange={setPlayersBusy}
            onDirtyChange={setPlayersDirty}
            onRosterChanged={(characters) =>
              setJourney((current) =>
                current
                  ? { ...current, journeyCharacters: characters }
                  : current,
              )
            }
          />
        </Drawer>
      )}
    </AppLayout>
  );
}
