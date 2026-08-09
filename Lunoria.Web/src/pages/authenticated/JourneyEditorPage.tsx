import { useEffect, useState } from "react";
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
  SceneOrderEditor,
  updateScene,
  type Scene,
} from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";

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
  const [scenesReloadKey, setScenesReloadKey] = useState(0);

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
                onClick={() =>
                  navigate(
                    `/series/${seriesId}/journeys/${journeyId}/play`,
                  )
                }
                variant="add"                
                size="lg"
                className="w-full py-4"
              >
                Play
              </Button>
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
            selectedCharacterIds={
              journey.journeyCharacters?.map(
                (journeyCharacter) => journeyCharacter.character.id,
              ) ?? []
            }
            onCancel={() => setIsManagingCharacters(false)}
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
