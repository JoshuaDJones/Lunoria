import { useEffect, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { useNavigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useModalStack, useToast } from "@/app/providers";
import { Button, Card, Drawer } from "@/components/ui";
import {
  activateSceneJourneyCharacter,
  addChestToScene,
  addPlaythroughCharacterToScene,
  ChestStatus,
  endScenePlaythrough,
  forfeitSceneParticipantAction,
  getScenePlaythrough,
  openSceneParticipantChest,
  ParticipantType,
  recordSceneParticipantMovement,
  resolveSceneParticipantAttack,
  SceneAttackType,
  SceneOptionsPanel,
  tradeSceneParticipantItem,
  transformSceneParticipant,
  updateSceneParticipantStats,
  useSceneParticipantConsumable,
  type SceneAttackResult,
  type SceneOpenChestResult,
  type ScenePlaythroughChest,
  type ScenePlaythroughDetails,
  type ScenePlaythroughDialog,
  type ScenePlaythroughInventoryItem,
  type ScenePlaythroughParticipant,
  type ScenePlaythroughSpell,
} from "@/features/journeys";
import { DialogViewer } from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";

interface AttackAnimationState {
  targetName: string;
  imageUrl: string | null;
}

export function ScenePlaythroughPage() {
  const navigate = useNavigate();
  const toast = useToast();
  const modalStack = useModalStack();
  const {
    seriesId,
    journeyId,
    playthroughId: playthroughIdParam,
    sceneId: sceneIdParam,
  } = useParams<{
    seriesId: string;
    journeyId: string;
    playthroughId: string;
    sceneId: string;
  }>();
  const playthroughId = Number(playthroughIdParam);
  const sceneId = Number(sceneIdParam);
  const [scene, setScene] = useState<ScenePlaythroughDetails>();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [isOptionsOpen, setIsOptionsOpen] = useState(false);
  const [begunTurnKey, setBegunTurnKey] = useState("");
  const [awaitingActionTurnKey, setAwaitingActionTurnKey] = useState("");
  const [optionAction, setOptionAction] = useState<string>();
  const transformPending = useRef(false);
  const [viewingDialog, setViewingDialog] =
    useState<ScenePlaythroughDialog>();
  const [attackAnimation, setAttackAnimation] =
    useState<AttackAnimationState>();

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

  const transform = async (participant: ScenePlaythroughParticipant) => {
    if (transformPending.current) return;
    transformPending.current = true;
    try {
      await transformSceneParticipant(playthroughId, sceneId, participant.id);
      setScene(await getScenePlaythrough(playthroughId, sceneId));
      setBegunTurnKey("");
      setAwaitingActionTurnKey("");
      modalStack.dismissAll();
      toast.success(
        participant.isInAlternateForm
          ? `${participant.name} returned to their base form.`
          : `${participant.name} transformed into their alternate form.`,
        participant.isInAlternateForm ? "Form reverted" : "Transformed",
      );
    } catch (requestError: unknown) {
      toast.error(getApiError(requestError).message, "Unable to transform");
    } finally {
      transformPending.current = false;
    }
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

  const attack = async (
    participant: ScenePlaythroughParticipant,
    targetParticipantId: number,
    attackType: SceneAttackType,
    roll: number,
    playthroughSpellId: number | null,
  ) => {
    const target = scene?.participants.find(
      (candidate) => candidate.id === targetParticipantId,
    );
    let stopAttackSounds = () => {};

    try {
      const result = await resolveSceneParticipantAttack(
        playthroughId,
        sceneId,
        participant.id,
        {
          targetParticipantId,
          attackType,
          roll,
          playthroughSpellId,
        },
      );

      setAttackAnimation({
        targetName: target?.name ?? "Target",
        imageUrl:
          target?.portraitUrl?.trim() || target?.photoUrl?.trim() || null,
      });
      stopAttackSounds = playAttackSlashSounds();
      await delay(2_000);
      stopAttackSounds();
      setAttackAnimation(undefined);

      const loadedScene = await getScenePlaythrough(playthroughId, sceneId);
      setScene(loadedScene);
      const refreshedAttacker = loadedScene.participants.find(
        (candidate) => candidate.id === participant.id,
      );
      if (
        refreshedAttacker?.isCurrentParticipant &&
        refreshedAttacker.attacksRemaining > 0
      ) {
        const turnKey = `${loadedScene.roundNumber}:${participant.id}`;
        setBegunTurnKey(turnKey);
        setAwaitingActionTurnKey(turnKey);
      } else {
        setBegunTurnKey("");
        setAwaitingActionTurnKey("");
      }
      modalStack.dismissAll();
      toast.success(getAttackResultMessage(result), "Attack complete");
    } catch (requestError: unknown) {
      stopAttackSounds();
      setAttackAnimation(undefined);
      toast.error(getApiError(requestError).message, "Unable to attack");
    }
  };

  const openChest = async (
    participant: ScenePlaythroughParticipant,
    chestId: number,
    roll: number,
  ): Promise<SceneOpenChestResult> => {
    try {
      const result = await openSceneParticipantChest(
        playthroughId,
        sceneId,
        participant.id,
        chestId,
        roll,
      );
      setScene(await getScenePlaythrough(playthroughId, sceneId));
      return result;
    } catch (requestError: unknown) {
      toast.error(
        getApiError(requestError).message,
        "Unable to open chest",
      );
      throw requestError;
    }
  };

  const usePotion = async (
    participant: ScenePlaythroughParticipant,
    inventoryItem: ScenePlaythroughInventoryItem,
  ) => {
    try {
      const result = await useSceneParticipantConsumable(
        playthroughId,
        sceneId,
        participant.id,
        inventoryItem.inventoryItemId,
      );
      setScene(await getScenePlaythrough(playthroughId, sceneId));
      setBegunTurnKey("");
      setAwaitingActionTurnKey("");
      modalStack.dismissAll();
      toast.success(
        `${participant.name} used ${result.itemName} and restored ${result.hpRestored} HP and ${result.mpRestored} MP.`,
        "Potion used",
      );
    } catch (requestError: unknown) {
      toast.error(getApiError(requestError).message, "Unable to use potion");
      throw requestError;
    }
  };

  const tradeItem = async (
    participant: ScenePlaythroughParticipant,
    target: ScenePlaythroughParticipant,
    inventoryItem: ScenePlaythroughInventoryItem,
  ) => {
    try {
      await tradeSceneParticipantItem(
        playthroughId,
        sceneId,
        participant.id,
        {
          targetParticipantId: target.id,
          inventoryItemId: inventoryItem.inventoryItemId,
          isEquippable: inventoryItem.isEquippable,
        },
      );
      setScene(await getScenePlaythrough(playthroughId, sceneId));
      setBegunTurnKey("");
      setAwaitingActionTurnKey("");
      modalStack.dismissAll();
      toast.success(
        `${inventoryItem.item.name} was traded successfully.`,
        "Trade complete",
      );
    } catch (requestError: unknown) {
      toast.error(getApiError(requestError).message, "Unable to trade item");
      throw requestError;
    }
  };

  const completeChestAction = (
    participant: ScenePlaythroughParticipant,
    result: SceneOpenChestResult,
  ) => {
    setBegunTurnKey("");
    setAwaitingActionTurnKey("");
    modalStack.dismissAll();

    if (result.awarded) {
      toast.success(
        `${participant.name} received ${result.quantity} × ${result.item.name} from ${result.chestName}.`,
        "Chest opened",
      );
      return;
    }

    const inventoryType = result.isEquippable ? "equippable" : "consumable";
    toast.error(
      `${participant.name} rolled ${result.quantity} × ${result.item.name}, but their ${inventoryType} inventory was full. The chest remains unopened and their turn was forfeited.`,
      "Inventory full",
    );
  };

  const openTurnActionDialog = (
    turnKey: string,
    participant: ScenePlaythroughParticipant,
  ) => {
    setAwaitingActionTurnKey(turnKey);
    const unopenedChests = (scene?.chests ?? []).filter(
      (chest) => chest.status === ChestStatus.Unopened,
    );
    modalStack.push({
      title: "Turn Action",
      placement: "center",
      content: (
        <TurnActionOptions
          participantType={participant.participantType}
          canTransform={participant.canTransform}
          isInAlternateForm={participant.isInAlternateForm}
          hasUnopenedChests={unopenedChests.length > 0}
          hasConsumables={participant.consumableItems.length > 0}
          onSelect={(title) => {
            if (title === "Transform") {
              void transform(participant);
              return;
            }

            if (title === "Attack") {
              modalStack.push({
                title: "Attack Type",
                placement: "center",
                content: (
                  <AttackTypeOptions
                    onSelect={(attackType) =>
                      modalStack.push({
                        title: getAttackTypeLabel(attackType),
                        placement: "center",
                        content: (
                          <AttackResolutionOptions
                            attacker={participant}
                            targets={getEligibleAttackTargets(
                              participant,
                              scene?.participants ?? [],
                            )}
                            attackType={attackType}
                            onAttack={(targetId, roll, spellId) =>
                              attack(
                                participant,
                                targetId,
                                attackType,
                                roll,
                                spellId,
                              )
                            }
                          />
                        ),
                      })
                    }
                  />
                ),
              });
              return;
            }

            if (title === "Open Chest") {
              modalStack.push({
                title: "Open Chest",
                placement: "center",
                content: (
                  <OpenChestOptions
                    chests={unopenedChests}
                    onOpen={(chestId, roll) =>
                      openChest(participant, chestId, roll)
                    }
                    onComplete={(result) =>
                      completeChestAction(participant, result)
                    }
                  />
                ),
              });
              return;
            }

            if (title === "Use Potion") {
              modalStack.push({
                title: "Use Potion",
                placement: "center",
                content: (
                  <PotionOptions
                    participant={participant}
                    onUse={(inventoryItem) =>
                      usePotion(participant, inventoryItem)
                    }
                  />
                ),
              });
              return;
            }

            if (title === "Trade Item") {
              const tradePartners = getEligibleTradePartners(
                participant,
                scene?.participants ?? [],
              );
              modalStack.push({
                title: "Choose Trade Partner",
                placement: "center",
                content: (
                  <TradePartnerOptions
                    participants={tradePartners}
                    onSelect={(target) =>
                      modalStack.push({
                        title: `Trade with ${target.name}`,
                        placement: "center",
                        content: (
                          <TradeInventoryOptions
                            participant={participant}
                            target={target}
                            onTrade={(item) =>
                              tradeItem(participant, target, item)
                            }
                          />
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

  const endScene = async () => {
    setOptionAction("end-scene");

    try {
      await endScenePlaythrough(playthroughId, sceneId);
      modalStack.dismissAll();
      setIsOptionsOpen(false);
      toast.success(`${scene?.name ?? "Scene"} has ended.`, "Scene ended");
      navigate(
        `/series/${seriesId}/journeys/${journeyId}/playthroughs/${playthroughId}`,
        { replace: true },
      );
    } catch (requestError: unknown) {
      toast.error(getApiError(requestError).message, "Unable to end scene");
      throw requestError;
    } finally {
      setOptionAction(undefined);
    }
  };

  const requestEndScene = () => {
    modalStack.push({
      title: "End Scene",
      placement: "center",
      content: (
        <EndScenePrompt
          sceneName={scene?.name ?? "this scene"}
          onConfirm={endScene}
        />
      ),
    });
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
              onAddChest={(input) =>
                void runSceneOption(
                  "add-chest",
                  () => addChestToScene(playthroughId, sceneId, input),
                  `Chest "${input.name}" added to the scene.`,
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
              onEndScene={requestEndScene}
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

      {attackAnimation && (
        <AttackAnimationOverlay animation={attackAnimation} />
      )}
    </AppLayout>
  );
}

function AttackAnimationOverlay({
  animation,
}: {
  animation: AttackAnimationState;
}) {
  return createPortal(
    <div
      className="attack-animation-overlay fixed inset-0 z-[9999] flex items-center justify-center bg-canvas/90 p-6 backdrop-blur-sm"
      role="status"
      aria-live="assertive"
      aria-label={`${animation.targetName} was attacked`}
    >
      <div className="attack-animation-target relative aspect-square w-full max-w-md overflow-hidden rounded-3xl border-2 border-danger/70 bg-canvas shadow-2xl shadow-danger/25">
        {animation.imageUrl ? (
          <img
            src={animation.imageUrl}
            alt=""
            className="h-full w-full object-contain"
          />
        ) : (
          <div className="flex h-full items-center justify-center p-8 text-center text-3xl font-semibold text-content">
            {animation.targetName}
          </div>
        )}
        <span className="attack-animation-slash attack-animation-slash-first" />
        <span className="attack-animation-slash attack-animation-slash-second" />
      </div>
    </div>,
    document.body,
  );
}

function playAttackSlashSounds() {
  const timers: Array<ReturnType<typeof setTimeout>> = [];
  const sounds = new Set<HTMLAudioElement>();

  const playSlash = () => {
    const sound = new Audio("/sounds/sword_slash.wav");
    sound.volume = 0.75;
    sounds.add(sound);

    const releaseSound = () => sounds.delete(sound);
    sound.addEventListener("ended", releaseSound, { once: true });
    void sound.play().catch(releaseSound);
  };

  timers.push(setTimeout(playSlash, 160));
  timers.push(setTimeout(playSlash, 720));

  return () => {
    timers.forEach(clearTimeout);
    sounds.forEach((sound) => {
      sound.pause();
      sound.currentTime = 0;
    });
    sounds.clear();
  };
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

          <dl className="mt-4 grid grid-cols-2 gap-2 text-sm sm:grid-cols-4">
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
            <div className="rounded-lg bg-surface/75 p-2">
              <dt className="text-content-muted">Attacks</dt>
              <dd className="font-semibold text-content">
                {participant.attacksRemaining} / {participant.attacksPerTurn}
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
                  {participant.downedTurnsRemaining !== null
                    ? ` · ${participant.downedTurnsRemaining} scheduled turns`
                    : ""}
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

function delay(milliseconds: number) {
  return new Promise<void>((resolve) => {
    window.setTimeout(resolve, milliseconds);
  });
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
  canTransform,
  isInAlternateForm,
  hasUnopenedChests,
  hasConsumables,
  onSelect,
  onForfeit,
}: {
  participantType: ParticipantType;
  canTransform: boolean;
  isInAlternateForm: boolean;
  hasUnopenedChests: boolean;
  hasConsumables: boolean;
  onSelect: (
    title: "Attack" | "Open Chest" | "Use Potion" | "Trade Item" | "Transform",
  ) => void;
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
      {canTransform && (
        <TurnActionButton
          label={isInAlternateForm ? "Revert" : "Transform"}
          imageSrc="/Transform_Action.png"
          onClick={() => onSelect("Transform")}
        />
      )}
      {hasConsumables && (
        <TurnActionButton
          label="Use Potion"
          imageSrc="/Use_Potion_Action.png"
          onClick={() => onSelect("Use Potion")}
        />
      )}
      {canManageItems && (
        <>
          {hasUnopenedChests && (
            <TurnActionButton
              label="Open Chest"
              imageSrc="/Open_Chest_Action.png"
              onClick={() => onSelect("Open Chest")}
            />
          )}
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

function PotionOptions({
  participant,
  onUse,
}: {
  participant: ScenePlaythroughParticipant;
  onUse: (item: ScenePlaythroughInventoryItem) => Promise<void>;
}) {
  const potionGroups = groupTradeInventoryItems(
    participant.consumableItems,
    false,
  );
  const [selectedInventoryItemId, setSelectedInventoryItemId] =
    useState<number>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const selectedGroup = potionGroups.find(
    ({ inventoryItem }) =>
      inventoryItem.inventoryItemId === selectedInventoryItemId,
  );
  const selectedPotion = selectedGroup?.inventoryItem;
  const hpEffect = selectedPotion?.item.hpEffect ?? 0;
  const mpEffect = selectedPotion?.item.mpEffect ?? 0;
  const hpRestored = Math.min(
    hpEffect,
    Math.max(0, participant.maxHp - participant.currentHp),
  );
  const mpRestored = Math.min(
    mpEffect,
    Math.max(0, participant.maxMp - participant.currentMp),
  );

  if (potionGroups.length === 0) {
    return (
      <p className="text-content-muted">
        This participant has no available consumable items.
      </p>
    );
  }

  return (
    <div>
      <div className="grid gap-4 sm:grid-cols-2">
        {potionGroups.map(({ inventoryItem, quantity }) => {
          const isSelected =
            inventoryItem.inventoryItemId === selectedInventoryItemId;
          const item = inventoryItem.item;

          return (
            <button
              key={inventoryItem.inventoryItemId}
              type="button"
              aria-pressed={isSelected}
              className={`overflow-hidden rounded-xl border-2 bg-surface text-left transition hover:border-utility focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 ${
                isSelected ? "border-utility ring-2 ring-utility/30" : "border-border"
              }`}
              disabled={isSubmitting}
              onClick={() =>
                setSelectedInventoryItemId(inventoryItem.inventoryItemId)
              }
            >
              <div className="relative aspect-[3/2] bg-canvas">
                {item.photoUrl ? (
                  <img
                    src={item.photoUrl}
                    alt=""
                    className="h-full w-full object-contain"
                  />
                ) : (
                  <div className="grid h-full place-items-center text-content-muted">
                    No image
                  </div>
                )}
                {quantity > 1 && (
                  <span className="absolute right-2 top-2 rounded-full bg-utility px-2 py-1 text-xs font-bold text-on-utility shadow-lg">
                    ×{quantity}
                  </span>
                )}
              </div>
              <div className="p-3">
                <p className="font-semibold text-content">{item.name}</p>
                {item.description && (
                  <p className="mt-1 line-clamp-2 text-sm text-content-muted">
                    {item.description}
                  </p>
                )}
                <div className="mt-3 flex flex-wrap gap-2 text-xs">
                  <span className="rounded-full bg-danger/10 px-2 py-1 font-semibold text-danger">
                    HP +{item.hpEffect ?? 0}
                  </span>
                  <span className="rounded-full bg-magic/10 px-2 py-1 font-semibold text-magic-hover">
                    MP +{item.mpEffect ?? 0}
                  </span>
                </div>
              </div>
            </button>
          );
        })}
      </div>

      {selectedPotion && (
        <div className="mt-5 grid grid-cols-2 gap-3 rounded-xl border border-border bg-surface p-4 text-center">
          <div>
            <p className="text-xs text-content-muted">HP after potion</p>
            <p className="mt-1 text-xl font-semibold text-content">
              {participant.currentHp + hpRestored} / {participant.maxHp}
            </p>
          </div>
          <div>
            <p className="text-xs text-content-muted">MP after potion</p>
            <p className="mt-1 text-xl font-semibold text-content">
              {participant.currentMp + mpRestored} / {participant.maxMp}
            </p>
          </div>
        </div>
      )}

      <div className="mt-6 flex justify-end">
        <Button
          variant="magic"
          size="lg"
          disabled={!selectedPotion || isSubmitting}
          onClick={() => {
            if (!selectedPotion) return;
            setIsSubmitting(true);
            void onUse(selectedPotion).finally(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Using Potion..." : "Use Potion"}
        </Button>
      </div>
    </div>
  );
}

interface TradeSelection {
  ownerParticipantId: number;
  inventoryItem: ScenePlaythroughInventoryItem;
}

function TradePartnerOptions({
  participants,
  onSelect,
}: {
  participants: ScenePlaythroughParticipant[];
  onSelect: (participant: ScenePlaythroughParticipant) => void;
}) {
  if (participants.length === 0) {
    return (
      <p className="text-content-muted">
        There are no other active player participants available to trade.
      </p>
    );
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {participants.map((participant) => {
        const imageUrl =
          participant.portraitUrl?.trim() || participant.photoUrl?.trim();

        return (
          <button
            key={participant.id}
            type="button"
            className="group overflow-hidden rounded-xl border-2 border-border bg-surface text-left transition hover:border-utility focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60"
            onClick={() => onSelect(participant)}
          >
            {imageUrl ? (
              <img
                src={imageUrl}
                alt=""
                className="aspect-square w-full object-cover"
              />
            ) : (
              <div className="grid aspect-square w-full place-items-center bg-surface-raised text-4xl font-semibold text-content-muted">
                {participant.name.charAt(0)}
              </div>
            )}
            <span className="block p-3 font-semibold text-content">
              {participant.name}
            </span>
          </button>
        );
      })}
    </div>
  );
}

function TradeInventoryOptions({
  participant,
  target,
  onTrade,
}: {
  participant: ScenePlaythroughParticipant;
  target: ScenePlaythroughParticipant;
  onTrade: (item: ScenePlaythroughInventoryItem) => Promise<void>;
}) {
  const [selection, setSelection] = useState<TradeSelection>();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const transferTo = async (
    destinationParticipantId: number,
    isEquippable: boolean,
  ) => {
    if (
      !selection ||
      selection.ownerParticipantId === destinationParticipantId ||
      selection.inventoryItem.isEquippable !== isEquippable ||
      isSubmitting
    ) {
      return;
    }

    setIsSubmitting(true);
    try {
      await onTrade(selection.inventoryItem);
    } catch {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="w-full max-w-5xl">
      <p className="mb-5 text-sm text-content-secondary">
        Drag one item into the matching inventory on the other side. You can
        also select an item and use the Move Here button. A successful trade
        completes the current turn.
      </p>
      <div className="grid gap-5 lg:grid-cols-2">
        <TradeCharacterInventory
          participant={participant}
          selection={selection}
          disabled={isSubmitting}
          onSelect={setSelection}
          onReceive={(isEquippable) =>
            void transferTo(participant.id, isEquippable)
          }
        />
        <TradeCharacterInventory
          participant={target}
          selection={selection}
          disabled={isSubmitting}
          onSelect={setSelection}
          onReceive={(isEquippable) =>
            void transferTo(target.id, isEquippable)
          }
        />
      </div>
      {isSubmitting && (
        <p className="mt-4 text-center text-sm font-semibold text-content-secondary">
          Trading item...
        </p>
      )}
    </div>
  );
}

function TradeCharacterInventory({
  participant,
  selection,
  disabled,
  onSelect,
  onReceive,
}: {
  participant: ScenePlaythroughParticipant;
  selection: TradeSelection | undefined;
  disabled: boolean;
  onSelect: (selection: TradeSelection) => void;
  onReceive: (isEquippable: boolean) => void;
}) {
  const imageUrl =
    participant.portraitUrl?.trim() || participant.photoUrl?.trim();

  return (
    <section className="rounded-2xl border border-border bg-surface p-4">
      <header className="mb-4 flex items-center gap-3">
        {imageUrl ? (
          <img
            src={imageUrl}
            alt=""
            className="h-14 w-14 rounded-xl object-cover"
          />
        ) : (
          <div className="grid h-14 w-14 place-items-center rounded-xl bg-surface-raised text-xl font-semibold text-content-muted">
            {participant.name.charAt(0)}
          </div>
        )}
        <div>
          <h3 className="text-xl font-semibold text-content">
            {participant.name}
          </h3>
          <p className="text-xs text-content-muted">Player inventory</p>
        </div>
      </header>

      <div className="space-y-5">
        <TradeInventorySlots
          title="Consumables"
          participantId={participant.id}
          items={participant.consumableItems ?? []}
          limit={participant.maxConsumableInventory}
          isEquippable={false}
          selection={selection}
          disabled={disabled}
          onSelect={onSelect}
          onReceive={onReceive}
        />
        <TradeInventorySlots
          title="Equippable Items"
          participantId={participant.id}
          items={participant.equippableItems ?? []}
          limit={participant.maxEquippableInventory}
          isEquippable
          selection={selection}
          disabled={disabled}
          onSelect={onSelect}
          onReceive={onReceive}
        />
      </div>
    </section>
  );
}

function TradeInventorySlots({
  title,
  participantId,
  items,
  limit,
  isEquippable,
  selection,
  disabled,
  onSelect,
  onReceive,
}: {
  title: string;
  participantId: number;
  items: ScenePlaythroughInventoryItem[];
  limit: number;
  isEquippable: boolean;
  selection: TradeSelection | undefined;
  disabled: boolean;
  onSelect: (selection: TradeSelection) => void;
  onReceive: (isEquippable: boolean) => void;
}) {
  const itemGroups = groupTradeInventoryItems(items, isEquippable);
  const freeSlotCount = Math.max(0, limit - items.length);
  const displayedSlotCount = itemGroups.length + freeSlotCount;
  const canReceive = Boolean(
    selection &&
      selection.ownerParticipantId !== participantId &&
      selection.inventoryItem.isEquippable === isEquippable &&
      !disabled,
  );

  return (
    <section
      className={`rounded-xl border p-3 transition ${
        canReceive
          ? "border-utility bg-utility/5"
          : "border-border bg-canvas/35"
      }`}
      onDragOver={(event) => {
        if (canReceive) event.preventDefault();
      }}
      onDrop={(event) => {
        event.preventDefault();
        if (canReceive) onReceive(isEquippable);
      }}
    >
      <div className="mb-3 flex items-center justify-between gap-3">
        <h4 className="font-semibold text-content">{title}</h4>
        <span className="text-xs text-content-muted">
          {items.length} / {limit}
        </span>
      </div>

      {displayedSlotCount === 0 ? (
        <p className="rounded-lg border border-dashed border-border p-3 text-center text-xs text-content-muted">
          No inventory slots
        </p>
      ) : (
        <div className="grid grid-cols-2 gap-2 sm:grid-cols-3">
          {Array.from({ length: displayedSlotCount }, (_, index) => {
            const itemGroup = itemGroups[index];
            if (!itemGroup) {
              return (
                <div
                  key={`empty-${index}`}
                  className={`grid aspect-square place-items-center rounded-lg border border-dashed text-center text-xs ${
                    canReceive
                      ? "border-utility text-utility-hover"
                      : "border-border text-content-muted"
                  }`}
                >
                  Empty slot
                </div>
              );
            }

            const { inventoryItem, quantity } = itemGroup;

            const isSelected =
              selection?.ownerParticipantId === participantId &&
              selection.inventoryItem.inventoryItemId ===
                inventoryItem.inventoryItemId &&
              selection.inventoryItem.isEquippable ===
                inventoryItem.isEquippable;
            return (
              <button
                key={`${inventoryItem.isEquippable ? "equipment" : "consumable"}-${inventoryItem.inventoryItemId}`}
                type="button"
                draggable={!disabled}
                aria-pressed={isSelected}
                title={`Trade ${inventoryItem.item.name}`}
                className={`relative aspect-square overflow-hidden rounded-lg border text-left transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 ${
                  isSelected
                    ? "border-utility ring-2 ring-utility/40"
                    : "border-border"
                } cursor-grab hover:border-utility active:cursor-grabbing`}
                onClick={() => {
                  if (!disabled) {
                    onSelect({ ownerParticipantId: participantId, inventoryItem });
                  }
                }}
                onDragStart={(event) => {
                  if (disabled) {
                    event.preventDefault();
                    return;
                  }
                  event.dataTransfer.effectAllowed = "move";
                  event.dataTransfer.setData(
                    "text/plain",
                    String(inventoryItem.inventoryItemId),
                  );
                  onSelect({ ownerParticipantId: participantId, inventoryItem });
                }}
              >
                {inventoryItem.item.photoUrl ? (
                  <img
                    src={inventoryItem.item.photoUrl}
                    alt=""
                    className="h-full w-full object-cover"
                  />
                ) : (
                  <div className="h-full w-full bg-surface-raised" />
                )}
                {quantity > 1 && (
                  <span className="absolute right-1.5 top-1.5 rounded-full bg-utility px-2 py-0.5 text-xs font-bold text-on-utility shadow-lg">
                    ×{quantity}
                  </span>
                )}
                <span className="absolute inset-x-0 bottom-0 bg-canvas/90 px-2 py-1 text-xs font-semibold text-content backdrop-blur-sm">
                  {inventoryItem.item.name}
                </span>
              </button>
            );
          })}
        </div>
      )}

      {canReceive && (
        <Button
          size="sm"
          variant="utility"
          className="mt-3 w-full"
          disabled={items.length >= limit}
          onClick={() => onReceive(isEquippable)}
        >
          {items.length >= limit ? "Inventory Full" : "Move Here"}
        </Button>
      )}
    </section>
  );
}

function groupTradeInventoryItems(
  items: ScenePlaythroughInventoryItem[],
  isEquippable: boolean,
) {
  if (isEquippable) {
    return items.map((inventoryItem) => ({ inventoryItem, quantity: 1 }));
  }

  const groups = new Map<
    number,
    { inventoryItem: ScenePlaythroughInventoryItem; quantity: number }
  >();

  for (const inventoryItem of items) {
    const existing = groups.get(inventoryItem.item.id);
    if (existing) {
      existing.quantity += 1;
    } else {
      groups.set(inventoryItem.item.id, { inventoryItem, quantity: 1 });
    }
  }

  return Array.from(groups.values());
}

function OpenChestOptions({
  chests,
  onOpen,
  onComplete,
}: {
  chests: ScenePlaythroughChest[];
  onOpen: (chestId: number, roll: number) => Promise<SceneOpenChestResult>;
  onComplete: (result: SceneOpenChestResult) => void;
}) {
  const [selectedChestId, setSelectedChestId] = useState<number | undefined>(
    chests.length === 1 ? chests[0].id : undefined,
  );
  const [selectedRoll, setSelectedRoll] = useState<number>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [resolvedLoot, setResolvedLoot] = useState<SceneOpenChestResult>();
  const chestSoundRef = useRef<HTMLAudioElement | null>(null);
  const selectedChest = chests.find((chest) => chest.id === selectedChestId);

  useEffect(() => {
    const sound = new Audio(
      "/sounds/Treasure_Chest_Magical_Glittering_Gold.wav",
    );
    sound.volume = 0.75;
    sound.loop = true;
    chestSoundRef.current = sound;
    void sound.play().catch(() => {
      // Browsers may block audio if playback permission has not been granted.
    });

    return () => {
      sound.pause();
      sound.currentTime = 0;
      chestSoundRef.current = null;
    };
  }, []);

  const selectRoll = async (roll: number) => {
    if (
      selectedChest === undefined ||
      roll > selectedChest.dieSides ||
      isSubmitting
    ) {
      return;
    }

    setSelectedRoll(roll);
    setIsSubmitting(true);
    chestSoundRef.current?.pause();
    if (chestSoundRef.current) chestSoundRef.current.currentTime = 0;

    try {
      const result = await onOpen(selectedChest.id, roll);
      setResolvedLoot(result);
      setIsSubmitting(false);
    } catch {
      setSelectedRoll(undefined);
      setIsSubmitting(false);
    }
  };

  if (resolvedLoot) {
    return (
      <ChestLootResult
        result={resolvedLoot}
        onContinue={() => onComplete(resolvedLoot)}
      />
    );
  }

  return (
    <div>
      <div className="mx-auto aspect-square w-full max-w-sm overflow-hidden rounded-2xl border-2 border-utility/60 bg-canvas shadow-xl shadow-utility/10">
        <img
          src="/Opening_Chest.png"
          alt="An open treasure chest"
          className="h-full w-full object-contain"
          onError={(event) => {
            event.currentTarget.onerror = null;
            event.currentTarget.src = "/Open_Chest_Action.png";
          }}
        />
      </div>

      <section className="mt-6">
        <h3 className="text-lg font-semibold text-content">
          {chests.length === 1 ? "Chest" : "Select a chest"}
        </h3>
        <div className="mt-3 grid gap-3 sm:grid-cols-2">
          {chests.map((chest) => {
            const isSelected = selectedChestId === chest.id;

            return (
              <button
                key={chest.id}
                type="button"
                disabled={isSubmitting}
                aria-pressed={isSelected}
                className={`cursor-pointer rounded-xl border-2 bg-surface p-4 text-left transition hover:border-utility focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 disabled:cursor-not-allowed disabled:opacity-50 ${
                  isSelected ? "border-utility" : "border-border"
                }`}
                onClick={() => {
                  setSelectedChestId(chest.id);
                  setSelectedRoll(undefined);
                }}
              >
                <span className="block font-semibold text-content">
                  {chest.name}
                </span>
                <span className="mt-1 block text-sm text-content-muted">
                  Roll a d{chest.dieSides} for loot
                </span>
              </button>
            );
          })}
        </div>
      </section>

      <section className="mt-6">
        <h3 className="text-lg font-semibold text-content">
          Select a die face
        </h3>
        <div className="mt-3 grid grid-cols-3 gap-3 sm:grid-cols-6">
          {[1, 2, 3, 4, 5, 6].map((value) => {
            const isUnavailable =
              selectedChest !== undefined && value > selectedChest.dieSides;

            return (
              <DieFaceButton
                key={value}
                value={value}
                selected={selectedRoll === value}
                disabled={
                  selectedChest === undefined || isUnavailable || isSubmitting
                }
                ariaLabel={`Select chest roll ${value}`}
                onClick={() => void selectRoll(value)}
                compact
              />
            );
          })}
        </div>
      </section>
      {isSubmitting && !resolvedLoot && (
        <p className="mt-5 text-center text-sm text-content-muted">
          Opening chest...
        </p>
      )}
    </div>
  );
}

function ChestLootResult({
  result,
  onContinue,
}: {
  result: SceneOpenChestResult;
  onContinue: () => void;
}) {
  const item = result.item;
  const equippableStats = [
    ["Melee attack", item.meleeAttackDamageModifier],
    ["Bow attack", item.bowAttackDamageModifier],
    ["Movement", item.movementModifier],
    ["Maximum HP", item.maxHpModifier],
    ["Maximum MP", item.maxMpModifier],
    ["Consumable slots", item.maxConsumableInventoryModifier],
    ["Equipment slots", item.maxEquippableInventoryModifier],
    ["Additional attacks", item.additionalAttacksPerTurn],
    ["Melee reduction", item.meleeDamageReduction],
    ["Bow reduction", item.bowDamageReduction],
    ["Spell reduction", item.spellDamageReduction],
  ] as const;
  const consumableStats = [
    ["HP effect", item.hpEffect],
    ["MP effect", item.mpEffect],
  ] as const;
  const stats = result.isEquippable ? equippableStats : consumableStats;

  return (
    <div className="chest-loot-reveal">
      <div className="grid gap-6 md:grid-cols-2 md:items-start">
        <div className="flex aspect-square items-center justify-center overflow-hidden rounded-2xl border-2 border-utility/60 bg-canvas p-4 shadow-xl shadow-utility/10">
          {item.photoUrl?.trim() ? (
            <img
              src={item.photoUrl}
              alt={item.name}
              className="max-h-full max-w-full object-contain drop-shadow-[0_0_1.25rem_rgba(250,204,21,0.65)]"
            />
          ) : (
            <span className="p-6 text-center text-2xl font-semibold text-content">
              {item.name}
            </span>
          )}
        </div>

        <div className="min-w-0">
          <p
            className={`text-sm font-semibold uppercase tracking-wide ${
              result.awarded ? "text-utility" : "text-danger"
            }`}
          >
            {result.awarded ? "Loot received" : "Inventory full"}
          </p>
          <h3 className="mt-1 text-2xl font-semibold text-content">
            {result.quantity} × {item.name}
          </h3>
          {item.description && (
            <p className="mt-3 text-sm leading-6 text-content-secondary">
              {item.description}
            </p>
          )}

          {!result.awarded && (
            <p className="mt-4 rounded-xl border border-danger/50 bg-danger/10 p-3 text-sm text-content-secondary">
              This item did not fit in the player&apos;s inventory. The chest
              remains unopened and the turn was forfeited.
            </p>
          )}

          <div className="mt-5 grid grid-cols-2 gap-2">
            {stats.map(([label, value]) => (
              <LootStat
                key={label}
                label={label}
                value={formatLootModifier(value ?? 0)}
              />
            ))}
            {result.isEquippable && item.affectedSpellType && (
              <LootStat label="Spell type" value={item.affectedSpellType} />
            )}
            {result.isEquippable && item.spellDamageModifier !== null && (
              <LootStat
                label="Spell damage"
                value={formatLootModifier(item.spellDamageModifier)}
              />
            )}
          </div>

          {result.isEquippable && item.addedSpells.length > 0 && (
            <div className="mt-4 rounded-xl border border-border bg-surface p-3">
              <p className="text-xs font-semibold uppercase tracking-wide text-content-muted">
                Added spells
              </p>
              <ul className="mt-2 space-y-2">
                {item.addedSpells.map((spell) => (
                  <li key={spell.id} className="text-sm text-content-secondary">
                    <span className="font-semibold text-content">
                      {spell.name}
                    </span>
                    {` · ${spell.mpCost} MP`}
                    {spell.damageEffect !== null &&
                      ` · ${spell.damageEffect} damage`}
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      </div>

      <div className="mt-6 flex justify-end">
        <Button variant="primary" size="lg" onClick={onContinue}>
          Continue
        </Button>
      </div>
    </div>
  );
}

function LootStat({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border border-border bg-surface p-3">
      <p className="text-xs text-content-muted">{label}</p>
      <p className="mt-1 font-semibold text-content">{value}</p>
    </div>
  );
}

function formatLootModifier(value: number) {
  return value > 0 ? `+${value}` : String(value);
}

function AttackTypeOptions({
  onSelect,
}: {
  onSelect: (attackType: SceneAttackType) => void;
}) {
  return (
    <div className="grid gap-4 sm:grid-cols-3">
      <TurnActionButton
        label="Melee Attack"
        imageSrc="/Melee_Attack.png"
        onClick={() => onSelect(SceneAttackType.Melee)}
      />
      <TurnActionButton
        label="Range Attack"
        imageSrc="/Bow_Attack.png"
        onClick={() => onSelect(SceneAttackType.Range)}
      />
      <TurnActionButton
        label="Spell Attack"
        imageSrc="/Spell_Attack.png"
        onClick={() => onSelect(SceneAttackType.Spell)}
      />
    </div>
  );
}

function AttackResolutionOptions({
  attacker,
  targets,
  attackType,
  onAttack,
}: {
  attacker: ScenePlaythroughParticipant;
  targets: ScenePlaythroughParticipant[];
  attackType: SceneAttackType;
  onAttack: (
    targetParticipantId: number,
    roll: number,
    playthroughSpellId: number | null,
  ) => Promise<void>;
}) {
  const damageSpells = attacker.spells.filter(
    (spell) => spell.damageEffect !== null,
  );
  const [selectedTargetId, setSelectedTargetId] = useState<number>();
  const [selectedRoll, setSelectedRoll] = useState<number>();
  const [selectedSpellId, setSelectedSpellId] = useState<number>();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const selectedSpell = damageSpells.find(
    (spell) => spell.id === selectedSpellId,
  );
  const selectedTarget = targets.find(
    (target) => target.id === selectedTargetId,
  );
  const baseDamage = getAttackBaseDamage(attacker, attackType, selectedSpell);
  const damageReduction = selectedTarget
    ? getAttackDamageReduction(selectedTarget, attackType)
    : 0;
  const totalDamage =
    baseDamage === null || selectedRoll === undefined
      ? null
      : Math.max(0, baseDamage + selectedRoll - damageReduction);
  const hasRequiredSpell =
    attackType !== SceneAttackType.Spell || selectedSpell !== undefined;
  const canAttack =
    selectedTargetId !== undefined &&
    selectedRoll !== undefined &&
    totalDamage !== null &&
    hasRequiredSpell &&
    !isSubmitting;

  return (
    <div>
      <div className="grid grid-cols-3 gap-4 rounded-xl border border-border bg-surface p-4 text-center">
        <div>
          <p className="text-sm text-content-muted">Attack damage</p>
          <p className="mt-1 text-3xl font-semibold text-content">
            {baseDamage ?? "—"}
          </p>
        </div>
        <div>
          <p className="text-sm text-content-muted">Target reduction</p>
          <p className="mt-1 text-3xl font-semibold text-content">
            {selectedTarget ? damageReduction : "—"}
          </p>
        </div>
        <div>
          <p className="text-sm text-content-muted">Total attack damage</p>
          <p className="mt-1 text-3xl font-semibold text-content">
            {totalDamage ?? "—"}
          </p>
        </div>
      </div>

      {attackType === SceneAttackType.Spell && (
        <SpellAttackSelector
          spells={damageSpells}
          currentMp={attacker.currentMp}
          selectedSpellId={selectedSpellId}
          onSelect={setSelectedSpellId}
        />
      )}

      <section className="mt-6">
        <h3 className="text-lg font-semibold text-content">Select a target</h3>
        {targets.length === 0 ? (
          <p className="mt-3 rounded-xl border border-border bg-surface p-4 text-content-muted">
            There are no eligible targets for this attack.
          </p>
        ) : (
          <div className="mt-3 grid gap-3 sm:grid-cols-2">
            {targets.map((target) => {
              const imageUrl =
                target.portraitUrl?.trim() || target.photoUrl?.trim();
              const isSelected = selectedTargetId === target.id;

              return (
                <button
                  key={target.id}
                  type="button"
                  aria-pressed={isSelected}
                  className={`flex cursor-pointer items-center gap-3 overflow-hidden rounded-xl border-2 bg-surface p-3 text-left transition hover:border-utility focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/60 ${
                    isSelected ? "border-utility" : "border-border"
                  }`}
                  onClick={() => setSelectedTargetId(target.id)}
                >
                  <span className="flex h-20 w-20 shrink-0 items-center justify-center overflow-hidden rounded-lg bg-canvas">
                    {imageUrl ? (
                      <img
                        src={imageUrl}
                        alt=""
                        className="h-full w-full object-contain"
                      />
                    ) : (
                      <span className="text-xs text-content-muted">
                        No image
                      </span>
                    )}
                  </span>
                  <span className="min-w-0">
                    <span className="block font-semibold text-content">
                      {target.name}
                    </span>
                    <span className="mt-1 block text-sm text-content-muted">
                      HP {target.currentHp} / {target.maxHp}
                    </span>
                  </span>
                </button>
              );
            })}
          </div>
        )}
      </section>

      <section className="mt-6">
        <h3 className="text-lg font-semibold text-content">
          Select a die face
        </h3>
        <div className="mt-3 grid grid-cols-3 gap-3 sm:grid-cols-6">
          {[1, 2, 3, 4, 5, 6].map((value) => (
            <DieFaceButton
              key={value}
              value={value}
              selected={selectedRoll === value}
              onClick={() => setSelectedRoll(value)}
              compact
            />
          ))}
        </div>
      </section>

      <div className="mt-6 flex justify-end">
        <Button
          variant="danger"
          size="lg"
          disabled={!canAttack}
          onClick={() => {
            if (
              selectedTargetId === undefined ||
              selectedRoll === undefined ||
              !hasRequiredSpell
            ) {
              return;
            }

            setIsSubmitting(true);
            void onAttack(
              selectedTargetId,
              selectedRoll,
              selectedSpell?.id ?? null,
            ).finally(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Attacking..." : "Attack"}
        </Button>
      </div>
    </div>
  );
}

function SpellAttackSelector({
  spells,
  currentMp,
  selectedSpellId,
  onSelect,
}: {
  spells: ScenePlaythroughSpell[];
  currentMp: number;
  selectedSpellId?: number;
  onSelect: (spellId: number) => void;
}) {
  return (
    <section className="mt-6">
      <h3 className="text-lg font-semibold text-content">Select a spell</h3>
      {spells.length === 0 ? (
        <p className="mt-3 rounded-xl border border-border bg-surface p-4 text-content-muted">
          This participant has no damage spells.
        </p>
      ) : (
        <div className="mt-3 grid gap-3 sm:grid-cols-2">
          {spells.map((spell) => {
            const canAfford = spell.mpCost <= currentMp;
            const isSelected = selectedSpellId === spell.id;

            return (
              <button
                key={spell.id}
                type="button"
                disabled={!canAfford}
                aria-pressed={isSelected}
                className={`cursor-pointer rounded-xl border-2 bg-surface p-3 text-left transition hover:border-magic focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-magic/60 disabled:cursor-not-allowed disabled:opacity-40 ${
                  isSelected ? "border-magic" : "border-border"
                }`}
                onClick={() => onSelect(spell.id)}
              >
                <span className="block font-semibold text-content">
                  {spell.name}
                </span>
                <span className="mt-1 block text-sm text-content-muted">
                  Damage {spell.damageEffect} · MP {spell.mpCost}
                </span>
                {spell.description && (
                  <span className="mt-2 block text-sm text-content-secondary">
                    {spell.description}
                  </span>
                )}
              </button>
            );
          })}
        </div>
      )}
    </section>
  );
}

function DieFaceButton({
  value,
  selected,
  onClick,
  compact = false,
  disabled = false,
  ariaLabel,
}: {
  value: number;
  selected: boolean;
  onClick: () => void;
  compact?: boolean;
  disabled?: boolean;
  ariaLabel?: string;
}) {
  return (
    <button
      type="button"
      aria-label={ariaLabel ?? `Select die face ${value}`}
      aria-pressed={selected}
      disabled={disabled}
      className={`aspect-square cursor-pointer rounded-xl border-2 bg-surface transition hover:border-utility hover:bg-utility/10 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-utility/50 disabled:cursor-not-allowed disabled:opacity-30 ${
        compact ? "p-2" : "p-5"
      } ${selected ? "border-utility bg-utility/10" : "border-border"}`}
      onClick={onClick}
    >
      <span className="grid h-full w-full grid-cols-3 grid-rows-3 gap-1">
        {Array.from({ length: 9 }, (_, index) => (
          <span
            key={index}
            className={
              dieDotPositions[value].includes(index)
                ? `m-auto block rounded-full bg-content ${
                    compact ? "h-2.5 w-2.5" : "h-4 w-4 sm:h-5 sm:w-5"
                  }`
                : undefined
            }
          />
        ))}
      </span>
    </button>
  );
}

function getAttackBaseDamage(
  attacker: ScenePlaythroughParticipant,
  attackType: SceneAttackType,
  spell?: ScenePlaythroughSpell,
) {
  switch (attackType) {
    case SceneAttackType.Melee:
      return attacker.meleeAttackDamage;
    case SceneAttackType.Range:
      return attacker.bowAttackDamage;
    case SceneAttackType.Spell:
      return spell?.damageEffect ?? null;
  }
}

function getAttackDamageReduction(
  target: ScenePlaythroughParticipant,
  attackType: SceneAttackType,
) {
  switch (attackType) {
    case SceneAttackType.Melee:
      return target.meleeDamageReduction;
    case SceneAttackType.Range:
      return target.bowDamageReduction;
    case SceneAttackType.Spell:
      return target.spellDamageReduction;
  }
}

function getEligibleAttackTargets(
  attacker: ScenePlaythroughParticipant,
  participants: ScenePlaythroughParticipant[],
) {
  return participants.filter((target) => {
    if (
      target.id === attacker.id ||
      !target.isActive ||
      target.isDown ||
      target.isDead
    ) {
      return false;
    }

    return attacker.participantType === ParticipantType.Enemy
      ? target.participantType === ParticipantType.Player ||
          target.participantType === ParticipantType.NPC
      : target.participantType === ParticipantType.Enemy;
  });
}

function getEligibleTradePartners(
  participant: ScenePlaythroughParticipant,
  participants: ScenePlaythroughParticipant[],
) {
  return participants.filter(
    (candidate) =>
      candidate.id !== participant.id &&
      candidate.participantType === ParticipantType.Player &&
      candidate.isActive &&
      !candidate.isDead,
  );
}

function getAttackTypeLabel(attackType: SceneAttackType) {
  switch (attackType) {
    case SceneAttackType.Melee:
      return "Melee Attack";
    case SceneAttackType.Range:
      return "Range Attack";
    default:
      return "Spell Attack";
  }
}

function getAttackResultMessage(result: SceneAttackResult) {
  const reward = result.rewardStat
    ? result.rewardAmount > 0
      ? ` You gained ${result.rewardAmount} ${result.rewardStat}.`
      : ` Your ${result.rewardStat} was already at maximum.`
    : "";
  const outcome = result.targetDefeated
    ? " The target was defeated."
    : ` The target has ${result.targetCurrentHp} HP remaining.`;

  return `Dealt ${result.damage} damage.${outcome}${reward}`;
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

function EndScenePrompt({
  sceneName,
  onConfirm,
}: {
  sceneName: string;
  onConfirm: () => Promise<void>;
}) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  return (
    <div>
      <p className="text-content-secondary">
        End {sceneName}? The scene will be marked completed and cannot be
        resumed.
      </p>
      <div className="mt-6 flex justify-end">
        <Button
          size="lg"
          variant="danger"
          inverted
          disabled={isSubmitting}
          onClick={() => {
            setIsSubmitting(true);
            void onConfirm().catch(() => setIsSubmitting(false));
          }}
        >
          {isSubmitting ? "Ending..." : "End Scene"}
        </Button>
      </div>
    </div>
  );
}
