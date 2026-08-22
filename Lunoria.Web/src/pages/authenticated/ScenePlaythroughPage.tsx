import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useToast } from "@/app/providers";
import { Button, Card, Drawer } from "@/components/ui";
import {
  addSceneCharacterInstance,
  getScenePlaythrough,
  ParticipantType,
  ScenePlaythroughStatus,
  type ScenePlaythroughDetails,
  type ScenePlaythroughParticipant,
} from "@/features/journeys";
import { getApiError } from "@/lib/apiClient";

export function ScenePlaythroughPage() {
  const toast = useToast();
  const { playthroughId: playthroughIdParam, sceneId: sceneIdParam } =
    useParams<{ playthroughId: string; sceneId: string }>();
  const playthroughId = Number(playthroughIdParam);
  const sceneId = Number(sceneIdParam);
  const [scene, setScene] = useState<ScenePlaythroughDetails>();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [addingSceneCharacterId, setAddingSceneCharacterId] =
    useState<number>();
  const [isOptionsOpen, setIsOptionsOpen] = useState(false);

  const addCharacterInstance = async (scenePlaythroughCharacterId: number) => {
    setAddingSceneCharacterId(scenePlaythroughCharacterId);

    try {
      await addSceneCharacterInstance(
        playthroughId,
        sceneId,
        scenePlaythroughCharacterId,
      );
      const refreshedScene = await getScenePlaythrough(playthroughId, sceneId);
      setScene(refreshedScene);
      toast.success("A new character instance was added to the scene.");
    } catch (requestError: unknown) {
      toast.error(
        getApiError(requestError).message,
        "Unable to add character",
      );
    } finally {
      setAddingSceneCharacterId(undefined);
    }
  };

  useEffect(() => {
    if (
      !Number.isInteger(playthroughId) ||
      playthroughId <= 0 ||
      !Number.isInteger(sceneId) ||
      sceneId <= 0
    ) {
      setError("The playthrough or scene ID is invalid.");
      setIsLoading(false);
      return;
    }

    let isCurrent = true;
    setIsLoading(true);
    setError("");

    void getScenePlaythrough(playthroughId, sceneId)
      .then((loadedScene) => {
        if (isCurrent) setScene(loadedScene);
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
  }, [playthroughId, sceneId]);

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
        <header className="flex flex-wrap items-end justify-between gap-5 p-10">
          <h1 className="text-4xl text-content sm:text-5xl lg:text-6xl">
            {scene?.name ?? ""}
          </h1>
          {scene && (
            <div className="flex items-center gap-3">
              <p className="text-2xl font-semibold text-content sm:text-3xl">
                Round {scene.roundNumber}
              </p>
              <Button
                aria-label="Open scene options"
                className="h-14 w-14 border-transparent p-0 hover:border-transparent"
                onClick={() => setIsOptionsOpen(true)}
              >
                <svg
                  aria-hidden="true"
                  viewBox="0 0 24 24"
                  className="h-8 w-8"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="2"
                  strokeLinecap="round"
                >
                  <path d="M4 6h16M4 12h16M4 18h16" />
                </svg>
              </Button>
            </div>
          )}
        </header>

        {isLoading && <p className="px-10 text-content">Loading scene...</p>}

        {!isLoading && error && (
          <p className="px-10 text-danger" role="alert">
            {error}
          </p>
        )}

        {!isLoading && !error && scene && (
          <div className="grid gap-6 px-6 pb-10 sm:px-10 lg:grid-cols-[minmax(0,4fr)_minmax(14rem,1fr)]">
            <section className="rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px]">
              <h2 className="text-3xl font-semibold text-content">
                Participants
              </h2>

              {scene.participants.length === 0 ? (
                <p className="mt-5 text-content-muted">
                  No active participants were added to this scene.
                </p>
              ) : (
                <div className="mt-5 grid grid-cols-1 gap-5 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4">
                  {scene.participants.map((participant) => (
                    <ParticipantCard
                      key={participant.id}
                      participant={participant}
                      isAdding={
                        addingSceneCharacterId ===
                        participant.scenePlaythroughCharacterId
                      }
                      addDisabled={addingSceneCharacterId !== undefined}
                      canAddCharacterInstance={
                        scene.status === ScenePlaythroughStatus.InProgress
                      }
                      onAddCharacterInstance={addCharacterInstance}
                    />
                  ))}
                </div>
              )}
            </section>

            <aside className="self-start rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px] lg:sticky lg:top-6 lg:max-h-[calc(100vh-3rem)] lg:overflow-y-auto">
              <h2 className="text-3xl font-semibold text-content">
                Event Log
              </h2>

              {scene.eventLogs.length === 0 ? (
                <p className="mt-5 text-content-muted">
                  No events have been recorded.
                </p>
              ) : (
                <ol className="mt-5 space-y-3">
                  {scene.eventLogs.map((eventLog) => (
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

      {isOptionsOpen && (
        <Drawer title="Scene Options" onClose={() => setIsOptionsOpen(false)}>
          <p className="text-content-muted">
            Scene controls will be available here.
          </p>
        </Drawer>
      )}
    </AppLayout>
  );
}

function ParticipantCard({
  participant,
  isAdding,
  addDisabled,
  canAddCharacterInstance,
  onAddCharacterInstance,
}: {
  participant: ScenePlaythroughParticipant;
  isAdding: boolean;
  addDisabled: boolean;
  canAddCharacterInstance: boolean;
  onAddCharacterInstance: (scenePlaythroughCharacterId: number) => void;
}) {
  const imageUrl =
    participant.portraitUrl?.trim() || participant.photoUrl?.trim();

  return (
    <Card
      className={
        participant.isCurrentParticipant
          ? "border-brand"
          : undefined
      }
    >
      <div className="flex min-h-48">
        <div className="flex w-2/5 shrink-0 items-center justify-center bg-canvas">
          {imageUrl ? (
            <img
              src={imageUrl}
              alt=""
              className="h-full max-h-56 w-full object-contain"
            />
          ) : (
            <span className="text-content-muted">No image</span>
          )}
        </div>

        <div className="flex min-w-0 flex-1 flex-col p-4">
          <div>
            <h3 className="text-xl font-semibold text-content">
              {participant.name}
            </h3>
            <p className="text-sm text-content-muted">
              {getParticipantTypeLabel(participant.participantType)}
            </p>
            {participant.description && (
              <p className="mt-2 text-sm text-content-secondary">
                {participant.description}
              </p>
            )}
          </div>

          <dl className="mt-4 grid grid-cols-2 gap-3 text-sm">
            <div className="rounded-lg bg-surface/75 p-2">
              <dt className="text-content-muted">HP</dt>
              <dd className="font-semibold text-content">
                {participant.currentHp} / {participant.maxHp}
              </dd>
            </div>
            <div className="rounded-lg bg-surface/75 p-2">
              <dt className="text-content-muted">MP</dt>
              <dd className="font-semibold text-content">
                {participant.currentMp} / {participant.maxMp}
              </dd>
            </div>
          </dl>

          {(participant.isDown ||
            participant.isDead ||
            participant.isInAlternateForm) && (
            <div className="mt-3 flex flex-wrap gap-2 text-xs font-semibold">
              {participant.isDown && (
                <span className="rounded-full border border-border px-3 py-1 text-content-secondary">
                  Down
                </span>
              )}
              {participant.isDead && (
                <span className="rounded-full border border-danger px-3 py-1 text-danger">
                  Dead
                </span>
              )}
              {participant.isInAlternateForm && (
                <span className="rounded-full border border-border px-3 py-1 text-content-secondary">
                  Alternate form
                </span>
              )}
            </div>
          )}

          {canAddCharacterInstance &&
            participant.scenePlaythroughCharacterId !== null && (
              <Button
                className="mt-4 w-full"
                disabled={addDisabled}
                onClick={() =>
                  onAddCharacterInstance(
                    participant.scenePlaythroughCharacterId!,
                  )
                }
              >
                {isAdding ? "Adding..." : "Add another"}
              </Button>
            )}
        </div>
      </div>
    </Card>
  );
}

function getParticipantTypeLabel(type: ParticipantType) {
  switch (type) {
    case ParticipantType.NPC:
      return "NPC";
    case ParticipantType.Enemy:
      return "Enemy";
    default:
      return "Journey character";
  }
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}
