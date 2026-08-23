import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useModalStack, useToast } from "@/app/providers";
import { Button, Card, Drawer } from "@/components/ui";
import {
  activateSceneJourneyCharacter,
  addPlaythroughCharacterToScene,
  forfeitSceneParticipantAction,
  getScenePlaythrough,
  ParticipantType,
  recordSceneParticipantMovement,
  SceneOptionsPanel,
  updateSceneParticipantStats,
  type ScenePlaythroughDetails,
  type ScenePlaythroughDialog,
  type ScenePlaythroughParticipant,
} from "@/features/journeys";
import { DialogViewer } from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";

export function ScenePlaythroughPage() {
  const toast = useToast();
  const modalStack = useModalStack();
  const { playthroughId: playthroughIdParam, sceneId: sceneIdParam } =
    useParams<{ playthroughId: string; sceneId: string }>();
  const playthroughId = Number(playthroughIdParam);
  const sceneId = Number(sceneIdParam);
  const [scene, setScene] = useState<ScenePlaythroughDetails>();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [isOptionsOpen, setIsOptionsOpen] = useState(false);
  const [begunTurnKey, setBegunTurnKey] = useState("");
  const [awaitingActionTurnKey, setAwaitingActionTurnKey] = useState("");
  const [optionAction, setOptionAction] = useState<string>();
  const [viewingDialog, setViewingDialog] =
    useState<ScenePlaythroughDialog>();

  const beginParticipantTurn = (
    roundNumber: number,
    participant: ScenePlaythroughParticipant,
  ) => {
    const turnKey = `${roundNumber}:${participant.id}`;

    modalStack.push({
      title: "Movement Roll",
      placement: "center",
      content: (
        <MovementRollOptions
          defaultMovement={participant.movement}
          onContinue={(roll) =>
            completeMovementRoll(turnKey, participant, roll)
          }
        />
      ),
    });
  };

  const forfeitAction = async (
    participant: ScenePlaythroughParticipant,
  ) => {
    try {
      await forfeitSceneParticipantAction(
        playthroughId,
        sceneId,
        participant.id,
      );
      setScene(await getScenePlaythrough(playthroughId, sceneId));
      setBegunTurnKey("");
      setAwaitingActionTurnKey("");
      modalStack.dismissAll();
      toast.success(
        `${participant.name} forfeited their action.`,
        "Action forfeited",
      );
    } catch (requestError: unknown) {
      toast.error(
        getApiError(requestError).message,
        "Unable to forfeit action",
      );
    }
  };

  const openTurnActionDialog = (
    turnKey: string,
    participant: ScenePlaythroughParticipant,
  ) => {
    setAwaitingActionTurnKey(turnKey);
    modalStack.push({
      title: "Turn Action",
      placement: "center",
      content: (
        <TurnActionOptions
          participantType={participant.participantType}
          onSelect={(title) => {
            if (title === "Attack") {
              modalStack.push({
                title: "Attack Type",
                placement: "center",
                content: (
                  <AttackTypeOptions
                    onSelect={(attackType) =>
                      modalStack.push({
                        title: attackType,
                        placement: "center",
                        content: (
                          <p className="text-content-muted">
                            {attackType} options will be added here.
                          </p>
                        ),
                      })
                    }
                  />
                ),
              });
              return;
            }

            modalStack.push({
              title,
              placement: "center",
              content: (
                <p className="text-content-muted">
                  {title} options will be added here.
                </p>
              ),
            });
          }}
          onForfeit={() =>
            modalStack.push({
              title: "Forfeit Action",
              placement: "center",
              content: (
                <ForfeitActionPrompt
                  participantName={participant.name}
                  onConfirm={() => forfeitAction(participant)}
                />
              ),
            })
          }
        />
      ),
    });
  };

  const completeMovementRoll = async (
    turnKey: string,
    participant: ScenePlaythroughParticipant,
    roll: number,
  ) => {
    try {
      const result = await recordSceneParticipantMovement(
        playthroughId,
        sceneId,
        participant.id,
        roll,
      );
      setScene(await getScenePlaythrough(playthroughId, sceneId));
      setBegunTurnKey(turnKey);
      setAwaitingActionTurnKey(turnKey);
      toast.success(
        `${participant.name} can move ${result.movement} spaces.`,
        "Movement",
      );
      modalStack.pop();
      openTurnActionDialog(turnKey, participant);
    } catch (requestError: unknown) {
      toast.error(
        getApiError(requestError).message,
        "Unable to record movement",
      );
    }
  };

  const runSceneOption = async (
    actionKey: string,
    operation: () => Promise<void>,
    successMessage: string,
  ) => {
    setOptionAction(actionKey);
    try {
      await operation();
      setScene(await getScenePlaythrough(playthroughId, sceneId));
      toast.success(successMessage);
    } catch (requestError: unknown) {
      toast.error(getApiError(requestError).message, "Scene option failed");
    } finally {
      setOptionAction(undefined);
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

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === "o") {
        event.preventDefault();
        setIsOptionsOpen(true);
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, []);

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
                  {scene.participants.map((participant) => {
                    const turnKey = `${scene.roundNumber}:${participant.id}`;
                    const isAwaitingAction =
                      awaitingActionTurnKey === turnKey;
                    const hasBegunTurn = begunTurnKey === turnKey;
                    const turnPromptLabel = isAwaitingAction
                      ? "Select Action"
                      : !hasBegunTurn
                        ? "Begin Turn"
                        : undefined;

                    return (
                      <ParticipantCard
                        key={participant.id}
                        participant={participant}
                        turnPromptLabel={turnPromptLabel}
                        onTurnPrompt={() => {
                          if (isAwaitingAction) {
                            openTurnActionDialog(turnKey, participant);
                            return;
                          }

                          beginParticipantTurn(scene.roundNumber, participant);
                        }}
                      />
                    );
                  })}
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
          {scene && (
            <SceneOptionsPanel
              scene={scene}
              busyAction={optionAction}
              onActivateJourneyCharacter={(characterId) =>
                void runSceneOption(
                  `activate-${characterId}`,
                  () =>
                    activateSceneJourneyCharacter(
                      playthroughId,
                      sceneId,
                      characterId,
                    ),
                  "Journey character activated.",
                )
              }
              onAddPlaythroughCharacter={(characterId) =>
                void runSceneOption(
                  `add-${characterId}`,
                  () =>
                    addPlaythroughCharacterToScene(
                      playthroughId,
                      sceneId,
                      characterId,
                    ),
                  "Scene character added.",
                )
              }
              onUpdateParticipant={(participantId, input) =>
                void runSceneOption(
                  `stats-${participantId}`,
                  () =>
                    updateSceneParticipantStats(
                      playthroughId,
                      sceneId,
                      participantId,
                      input,
                    ),
                  "Participant stats updated.",
                )
              }
              onViewDialog={setViewingDialog}
            />
          )}
        </Drawer>
      )}

      {viewingDialog && (
        <DialogViewer
          dialog={viewingDialog}
          onClose={() => setViewingDialog(undefined)}
        />
      )}
    </AppLayout>
  );
}

function ParticipantCard({
  participant,
  turnPromptLabel,
  onTurnPrompt,
}: {
  participant: ScenePlaythroughParticipant;
  turnPromptLabel?: "Begin Turn" | "Select Action";
  onTurnPrompt: () => void;
}) {
  const imageUrl =
    participant.portraitUrl?.trim() || participant.photoUrl?.trim();
  const isWaitingForTurn =
    participant.isCurrentParticipant &&
    turnPromptLabel !== undefined;

  return (
    <Card
      className={
        participant.isCurrentParticipant
          ? "relative border-utility"
          : "relative"
      }
    >
      <div
        className={`flex min-h-48 transition ${
          isWaitingForTurn
            ? "pointer-events-none select-none blur-[1.5px]"
            : ""
        }`}
      >
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

          <dl className="mt-4 grid grid-cols-3 gap-2 text-sm">
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
            <div className="rounded-lg bg-surface/75 p-2">
              <dt className="text-content-muted">Move</dt>
              <dd className="font-semibold text-content">
                {participant.movement}
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

        </div>
      </div>

      {isWaitingForTurn && (
        <div className="absolute inset-0 flex items-center justify-center bg-canvas/30">
          <Button
            variant="utility"
            size="lg"
            className="animate-[scene-turn-attention_1.5s_ease-in-out_infinite] motion-reduce:animate-none"
            onClick={onTurnPrompt}
          >
            {turnPromptLabel}
          </Button>
        </div>
      )}
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

const dieDotPositions: Record<number, number[]> = {
  1: [4],
  2: [0, 8],
  3: [0, 4, 8],
  4: [0, 2, 6, 8],
  5: [0, 2, 4, 6, 8],
  6: [0, 2, 3, 5, 6, 8],
};

function MovementRollOptions({
  defaultMovement,
  onContinue,
}: {
  defaultMovement: number;
  onContinue: (roll: number) => Promise<void>;
}) {
  const [selectedRoll, setSelectedRoll] = useState<number>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const totalMovement = defaultMovement + (selectedRoll ?? 0);

  return (
    <div>
      <div className="grid grid-cols-2 gap-4 rounded-xl border border-border bg-surface p-4 text-center">
        <div>
          <p className="text-sm text-content-muted">Default movement</p>
          <p className="mt-1 text-3xl font-semibold text-content">
            {defaultMovement}
          </p>
        </div>
        <div>
          <p className="text-sm text-content-muted">Total movement</p>
          <p className="mt-1 text-3xl font-semibold text-content">
            {totalMovement}
          </p>
        </div>
      </div>

      <p className="mt-6 text-content-secondary">
        Select a die face to add it to the default movement.
      </p>
      <div className="mt-6 grid grid-cols-2 gap-4 sm:grid-cols-3">
        {[1, 2, 3, 4, 5, 6].map((value) => (
          <button
            key={value}
            type="button"
            aria-label={`Select movement roll ${value}`}
            aria-pressed={selectedRoll === value}
            className={`aspect-square cursor-pointer rounded-xl border-2 bg-surface p-5 transition hover:border-utility hover:bg-utility/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/50 ${
              selectedRoll === value
                ? "border-utility bg-utility/10"
                : "border-border"
            }`}
            onClick={() => setSelectedRoll(value)}
          >
            <span className="grid h-full w-full grid-cols-3 grid-rows-3 gap-2">
              {Array.from({ length: 9 }, (_, index) => (
                <span
                  key={index}
                  className={
                    dieDotPositions[value].includes(index)
                      ? "m-auto block h-4 w-4 rounded-full bg-content sm:h-5 sm:w-5"
                      : undefined
                  }
                />
              ))}
            </span>
          </button>
        ))}
      </div>

      <div className="mt-6 flex justify-end">
        <Button
          variant="primary"
          disabled={selectedRoll === undefined || isSubmitting}
          onClick={() => {
            if (selectedRoll === undefined) return;
            setIsSubmitting(true);
            void onContinue(selectedRoll).finally(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Continuing..." : "Continue"}
        </Button>
      </div>
    </div>
  );
}

function TurnActionOptions({
  participantType,
  onSelect,
  onForfeit,
}: {
  participantType: ParticipantType;
  onSelect: (title: "Attack" | "Open Chest" | "Trade Item") => void;
  onForfeit: () => void;
}) {
  const canManageItems = participantType === ParticipantType.Player;

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      <TurnActionButton
        label="Attack"
        imageSrc="/Attack_Action.png"
        onClick={() => onSelect("Attack")}
      />
      {canManageItems && (
        <>
          <TurnActionButton
            label="Open Chest"
            imageSrc="/Open_Chest_Action.png"
            onClick={() => onSelect("Open Chest")}
          />
          <TurnActionButton
            label="Trade Item"
            imageSrc="/Trade_Action.png"
            onClick={() => onSelect("Trade Item")}
          />
        </>
      )}
      <TurnActionButton
        label="Forfeit Action"
        imageSrc="/Forfeit_Action.png"
        onClick={onForfeit}
      />
    </div>
  );
}

function TurnActionButton({
  label,
  imageSrc,
  onClick,
}: {
  label: string;
  imageSrc: string;
  onClick: () => void;
}) {
  return (
    <button
      type="button"
      aria-label={label}
      className="group relative aspect-square cursor-pointer overflow-hidden rounded-xl border-2 border-border bg-canvas shadow-lg transition hover:-translate-y-0.5 hover:border-utility hover:shadow-xl focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60"
      onClick={onClick}
    >
      <img
        src={imageSrc}
        alt=""
        className="h-full w-full object-cover transition duration-200 group-hover:scale-[1.02]"
      />
      <span className="absolute left-0 top-0 rounded-br-xl bg-canvas/90 px-4 py-2 text-left text-lg font-semibold text-content shadow-lg backdrop-blur-sm">
        {label}
      </span>
    </button>
  );
}

function AttackTypeOptions({
  onSelect,
}: {
  onSelect: (
    attackType: "Melee Attack" | "Range Attack" | "Spell Attack",
  ) => void;
}) {
  return (
    <div className="grid gap-4 sm:grid-cols-3">
      <TurnActionButton
        label="Melee Attack"
        imageSrc="/Melee_Attack.png"
        onClick={() => onSelect("Melee Attack")}
      />
      <TurnActionButton
        label="Range Attack"
        imageSrc="/Bow_Attack.png"
        onClick={() => onSelect("Range Attack")}
      />
      <TurnActionButton
        label="Spell Attack"
        imageSrc="/Spell_Attack.png"
        onClick={() => onSelect("Spell Attack")}
      />
    </div>
  );
}

function ForfeitActionPrompt({
  participantName,
  onConfirm,
}: {
  participantName: string;
  onConfirm: () => Promise<void>;
}) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  return (
    <div>
      <p className="text-content-secondary">
        End {participantName}&apos;s turn without taking an action?
      </p>
      <div className="mt-6 flex justify-end">
        <Button
          size="lg"
          variant="danger"
          inverted
          disabled={isSubmitting}
          onClick={() => {
            setIsSubmitting(true);
            void onConfirm().finally(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Forfeiting..." : "Forfeit Action"}
        </Button>
      </div>
    </div>
  );
}
