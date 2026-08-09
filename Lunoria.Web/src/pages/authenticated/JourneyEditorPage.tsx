import { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlay } from "@fortawesome/free-solid-svg-icons";
import { Link, Navigate, useNavigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useConfirmDialog, useToast } from "@/app/providers";
import { requiredPhoto, textValue } from "@/components/forms/formValues";
import {
  ResourceForm,
  type ResourceFormField,
} from "@/components/forms/ResourceForm";
import { ApiLoadError, Button, Drawer } from "@/components/ui";
import {
  getJourney,
  JourneyCharacterPicker,
  replaceJourneyCharacters,
  type Journey,
} from "@/features/journeys";
import {
  createScene,
  deleteScene,
  listScenes,
  reorderScenes,
  SceneGrid,
  SceneEventManager,
  SceneChestManager,
  SceneCharacterManager,
  SceneOrderEditor,
  updateScene,
  type Scene,
} from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";
import {
  listPreviousPlaythroughs,
  resumePlaythrough,
  startPlaythrough,
  type JourneyPlaythrough,
} from "@/features/playthroughs";

const sceneFields: ResourceFormField[] = [
  { name: "name", label: "Name", required: true },
  {
    name: "description",
    label: "Description",
    type: "textarea",
    required: true,
  },
  { name: "gridUrl", label: "Grid URL", required: true },
];

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
  const [isManagingCharacters, setIsManagingCharacters] = useState(false);
  const [eventsScene, setEventsScene] = useState<Scene>();
  const [chestsScene, setChestsScene] = useState<Scene>();
  const [charactersScene, setCharactersScene] = useState<Scene>();
  const [scenesReloadKey, setScenesReloadKey] = useState(0);
  const [playthroughs, setPlaythroughs] = useState<JourneyPlaythrough[]>([]);
  const [arePlaythroughsLoading, setArePlaythroughsLoading] = useState(true);
  const [playthroughsError, setPlaythroughsError] = useState("");
  const [startingPlaythrough, setStartingPlaythrough] = useState(false);
  const [resumingPlaythroughId, setResumingPlaythroughId] = useState<number>();

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

  useEffect(() => {
    if (!Number.isInteger(journeyId) || journeyId <= 0) return;
    let isCurrent = true;

    void listPreviousPlaythroughs(journeyId)
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
  }, [journeyId]);

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
      scrolling
      background={
        <div className="stone-image absolute inset-0 z-0 h-full w-full" />
      }
    >
      <main className="w-full p-6 sm:p-10">
        <header className="mb-6 flex items-center justify-between">
          <div>
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
        </header>

        {!isLoading && error && (
          <ApiLoadError error={error} onRetry={loadJourney} />
        )}

        {!isLoading && !error && journey && (
          <div className="grid gap-5 lg:grid-cols-[minmax(0,1fr)_18rem]">
            <section className="min-h-[30rem] rounded-3xl bg-surface/65 p-4 backdrop-blur-[2px] sm:p-6">
              <div className="flex flex-wrap items-center justify-between gap-4">
                <h2 className="text-4xl text-content">Scenes</h2>
                <div className="flex flex-wrap gap-3">
                  <Button
                    onClick={() => setIsOrderingScenes(true)}
                    disabled={scenes.length < 2}
                    variant="secondary"
                    inverted
                    size="lg"
                    className="min-w-40"
                  >
                    Scene Order
                  </Button>
                  <Button
                    onClick={() => setEditingScene(null)}
                    variant="add"
                    inverted
                    size="lg"
                    className="min-w-40"
                  >
                    Add Scene
                  </Button>
                </div>
              </div>

              <div className="mt-6">
                {areScenesLoading && (
                  <p className="text-content-secondary" role="status">
                    Loading scenes...
                  </p>
                )}

                {!areScenesLoading && scenesError && (
                  <ApiLoadError error={scenesError} onRetry={retryScenes} />
                )}

                {!areScenesLoading && !scenesError && scenes.length === 0 && (
                  <div className="rounded-xl border border-border bg-surface/60 p-8 text-center">
                    <h3 className="text-2xl font-semibold text-content">
                      No scenes yet
                    </h3>
                    <p className="mt-2 text-content-muted">
                      Add your first scene to get started.
                    </p>
                  </div>
                )}

                {!areScenesLoading && !scenesError && scenes.length > 0 && (
                  <SceneGrid
                    scenes={scenes}
                    className="sm:grid-cols-1 xl:grid-cols-2"
                    onViewEvents={setEventsScene}
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

            <aside className="flex min-h-72 flex-col gap-4 rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px]">
              <Button
                onClick={() => void (async () => {
                  setStartingPlaythrough(true);
                  try {
                    const playthrough = await startPlaythrough(journeyId);
                    setPlaythroughs((current) => [playthrough, ...current]);
                    navigate(`/series/${seriesId}/journeys/${journeyId}/play?playthroughId=${playthrough.id}`);
                  } catch (requestError) {
                    toast.error(getApiError(requestError).message, "Unable to start playthrough");
                    setStartingPlaythrough(false);
                  }
                })()}
                disabled={startingPlaythrough}
                variant="add"                
                size="lg"
                className="w-full py-4"
                leftIcon={<FontAwesomeIcon icon={faPlay} />}
              >
                {startingPlaythrough ? "Starting..." : "Start"}
              </Button>

              <section className="mt-1">
                <h2 className="text-2xl font-semibold text-content">Playthroughs</h2>
                <div className="mt-2 h-px w-full bg-border" />

                {arePlaythroughsLoading && <p className="mt-4 text-sm text-content-secondary">Loading playthroughs...</p>}
                {!arePlaythroughsLoading && playthroughsError && <p className="mt-4 text-sm text-danger" role="alert">{playthroughsError}</p>}
                {!arePlaythroughsLoading && !playthroughsError && playthroughs.length === 0 && <p className="mt-4 text-sm text-content-muted">No previous playthroughs.</p>}

                {!arePlaythroughsLoading && !playthroughsError && playthroughs.length > 0 && (
                  <div className="mt-4 space-y-3">
                    {playthroughs.map((playthrough) => (
                      <article key={playthrough.id} className="rounded-xl border border-border bg-surface/75 p-3">
                        <div className="flex items-start justify-between gap-2">
                          <div>
                            <p className="font-semibold text-content">Playthrough #{playthrough.id}</p>
                            <p className="mt-0.5 text-xs text-content-secondary">Started {formatPlaythroughDate(playthrough.startedAt)}</p>
                          </div>
                          <span className={`rounded-full px-2 py-1 text-[0.7rem] font-semibold ${playthrough.completedAt ? "bg-surface-raised text-content-muted" : playthrough.isActive ? "bg-add/15 text-add" : "bg-utility/15 text-utility-hover"}`}>
                            {playthrough.completedAt ? "Complete" : playthrough.isActive ? "Active" : "Paused"}
                          </span>
                        </div>
                        <div className="mt-3 flex flex-wrap justify-end gap-2">
                          <Button onClick={() => navigate(`/series/${seriesId}/journeys/${journeyId}/play?playthroughId=${playthrough.id}&view=logs`)} size="sm" variant="secondary">View Logs</Button>
                          {!playthrough.completedAt && (
                            <Button
                              onClick={() => void (async () => {
                                setResumingPlaythroughId(playthrough.id);
                                try {
                                  await resumePlaythrough(journeyId, playthrough.id);
                                  navigate(`/series/${seriesId}/journeys/${journeyId}/play?playthroughId=${playthrough.id}`);
                                } catch (requestError) {
                                  toast.error(getApiError(requestError).message, "Unable to resume playthrough");
                                  setResumingPlaythroughId(undefined);
                                }
                              })()}
                              disabled={resumingPlaythroughId !== undefined}
                              size="sm"
                              variant="primary"
                              leftIcon={<FontAwesomeIcon icon={faPlay} />}
                            >
                              {resumingPlaythroughId === playthrough.id ? "Resuming..." : "Resume"}
                            </Button>
                          )}
                        </div>
                      </article>
                    ))}
                  </div>
                )}
              </section>

              <div className="mt-auto space-y-4 pt-2">
              <Button
                onClick={() =>
                  navigate(
                    `/series/${seriesId}/journeys/${journeyId}/intro-pages`,
                  )
                }
                variant="secondary"
                inverted
                size="lg"
                className="w-full py-4"
              >
                Intro Pages
              </Button>
              <Button
                onClick={() => setIsManagingCharacters(true)}
                variant="secondary"
                inverted
                size="lg"
                className="w-full py-4"
              >
                Characters
              </Button>
              </div>
            </aside>
          </div>
        )}
      </main>

      {editingScene !== undefined && (
        <Drawer
          title={editingScene ? "Edit scene" : "Create scene"}
          onClose={() => setEditingScene(undefined)}
        >
          <ResourceForm
            fields={sceneFields}
            initialValues={{
              name: editingScene?.name ?? "",
              description: editingScene?.description ?? "",
              gridUrl: editingScene?.gridUrl ?? "",
            }}
            existingPhotoUrl={editingScene?.photoUrl}
            requirePhoto={!editingScene}
            onSubmit={async (values, photo) => {
              const input = {
                journeyId,
                name: textValue(values, "name"),
                description: textValue(values, "description"),
                gridUrl: textValue(values, "gridUrl"),
              };

              if (editingScene) {
                await updateScene(editingScene.id, { ...input, photo });
                toast.success(`Scene "${input.name}" was updated.`);
              } else {
                await createScene({ ...input, photo: requiredPhoto(photo) });
                toast.success(`Scene "${input.name}" was created.`);
              }

              setEditingScene(undefined);
              setAreScenesLoading(true);
              setScenesReloadKey((value) => value + 1);
            }}
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
          title={`${charactersScene.name} Characters`}
          onClose={() => setCharactersScene(undefined)}
        >
          <SceneCharacterManager key={charactersScene.id} scene={charactersScene} />
        </Drawer>
      )}

      {isOrderingScenes && (
        <Drawer title="Scene Order" onClose={() => setIsOrderingScenes(false)}>
          <SceneOrderEditor
            scenes={scenes}
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
          title="Journey Characters"
          onClose={() => setIsManagingCharacters(false)}
        >
          <JourneyCharacterPicker
            journeyCharacters={journey.journeyCharacters ?? []}
            selectedCharacterIds={
              journey.journeyCharacters?.map(
                (journeyCharacter) => journeyCharacter.character.id,
              ) ?? []
            }
            onCancel={() => setIsManagingCharacters(false)}
            onCharacterUpdated={(updatedCharacter) => {
              setJourney((current) => current ? {
                ...current,
                journeyCharacters: current.journeyCharacters?.map((character) =>
                  character.id === updatedCharacter.id ? updatedCharacter : character,
                ) ?? [],
              } : current);
            }}
            onSave={async (characterIds) => {
              await replaceJourneyCharacters(journeyId, characterIds);
              setJourney(await getJourney(journeyId));
              setIsManagingCharacters(false);
              toast.success("Journey characters were updated.");
            }}
          />
        </Drawer>
      )}
    </AppLayout>
  );
}

function formatPlaythroughDate(value: string): string {
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? "Unknown" : new Intl.DateTimeFormat(undefined, { dateStyle: "medium", timeStyle: "short" }).format(date);
}
