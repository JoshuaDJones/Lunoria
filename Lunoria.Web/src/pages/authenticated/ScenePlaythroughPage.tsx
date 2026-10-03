import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import AppLayout from "@/app/layouts";
import { useModalStack, useToast } from "@/app/providers";
import { Button, Drawer } from "@/components/ui";
import {
  activateSceneJourneyCharacter,
  addChestToScene,
  addPlaythroughCharacterToScene,
  ChestStatus,
  endScenePlaythrough,
  forfeitSceneParticipantAction,
  getScenePlaythrough,
  openSceneParticipantChest,
  recordSceneParticipantMovement,
  resolveSceneParticipantAttack,
  removeSceneParticipant,
  passSceneCounterattack,
  SceneAttackType,
  SceneOptionsPanel,
  tradeSceneParticipantItem,
  transformSceneParticipant,
  updateSceneParticipantStats,
  useSceneParticipantConsumable,
  type SceneOpenChestResult,
  type ScenePlaythroughDetails,
  type ScenePlaythroughDialog,
  type ScenePlaythroughInventoryItem,
  type ScenePlaythroughParticipant,
  type ScenePlaythroughCharacterOption,
} from "@/features/journeys";
import { DialogViewer } from "@/features/scenes";
import { getApiError } from "@/lib/apiClient";
import { ParticipantDetails } from "@/features/journeys/components/ParticipantDetails";
import {
  CounterAttackDialog,
  AttackAnimationOverlay,
  AttackTypeOptions,
  AttackResolutionOptions,
  ParticipantCard,
  SceneBackground,
  MovementRollOptions,
  TurnActionOptions,
  PotionOptions,
  TradePartnerOptions,
  TradeInventoryOptions,
  TransformConfirmation,
  OpenChestOptions,
  ForfeitActionPrompt,
  EndScenePrompt,
  ActionDialogContent,
} from "@/features/sceneplaythrough";
import { type AttackAnimationState } from "@/features/sceneplaythrough/types";
import {
  getAttackTypeLabel,
  getAttackResultMessage,
} from "@/features/sceneplaythrough/utils/sceneCombatUtils";
import { getEligibleTradePartners } from "@/features/sceneplaythrough/utils/sceneInventoryUtils";
import { formatDate } from "@/features/sceneplaythrough/utils/sceneDisplayUtils";
import {
  playAttackSlashSounds,
  delay,
} from "@/features/sceneplaythrough/utils/sceneAudio";

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
  const [viewingDialog, setViewingDialog] = useState<ScenePlaythroughDialog>();
  const [attackAnimation, setAttackAnimation] =
    useState<AttackAnimationState>();

  const animateAttack = async (target?: ScenePlaythroughParticipant) => {
    setAttackAnimation({
      targetName: target?.name ?? "Target",
      imageUrl: target?.portraitUrl?.trim() || target?.photoUrl?.trim() || null,
    });
    const stopAttackSounds = playAttackSlashSounds();
    try {
      await delay(2_000);
    } finally {
      stopAttackSounds();
      setAttackAnimation(undefined);
    }
  };

  const beginParticipantTurn = (
    roundNumber: number,
    participant: ScenePlaythroughParticipant,
    playthroughCharacters: ScenePlaythroughCharacterOption[],
  ) => {
    const turnKey = `${roundNumber}:${participant.id}`;

    modalStack.push({
      title: "Movement Roll",
      placement: "center",
      content: (
        <MovementRollOptions
          defaultMovement={participant.movement}
          onContinue={(roll) =>
            completeMovementRoll(
              turnKey,
              participant,
              roll,
              playthroughCharacters,
            )
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

  const forfeitAction = async (participant: ScenePlaythroughParticipant) => {
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
    targetParticipantId: number | null,
    attackType: SceneAttackType,
    roll: number,
    playthroughSpellId: number | null,
  ) => {
    const target = scene?.participants.find(
      (candidate) => candidate.id === targetParticipantId,
    );

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

      if (!result.isSupport && !result.isUtility) {
        await animateAttack(target);
      }

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
      toast.success(
        getAttackResultMessage(result),
        result.isSupport || result.isUtility ? "Spell cast" : "Attack complete",
      );
    } catch (requestError: unknown) {
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
      toast.error(getApiError(requestError).message, "Unable to open chest");
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
      await tradeSceneParticipantItem(playthroughId, sceneId, participant.id, {
        targetParticipantId: target.id,
        inventoryItemId: inventoryItem.inventoryItemId,
        isEquippable: inventoryItem.isEquippable,
      });
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
              modalStack.push({
                title: participant.isInAlternateForm
                  ? "Confirm revert"
                  : "Confirm transformation",
                placement: "center",
                content: (
                  <TransformConfirmation
                    participant={participant}
                    onConfirm={() => transform(participant)}
                    onCancel={() => modalStack.pop()}
                  />
                ),
              });
              return;
            }

            if (title === "Attack") {
              modalStack.push({
                title: "Attack Type",
                placement: "center",
                content: (
                  <AttackTypeOptions
                    participant={participant}
                    onSelect={(attackType) =>
                      modalStack.push({
                        title: getAttackTypeLabel(attackType),
                        placement: "center",
                        content: (
                          <AttackResolutionOptions
                            attacker={participant}
                            targets={scene?.participants ?? []}
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
                    onSelect={(target) => {
                      const tradeModalId = modalStack.push({
                        title: `Trade with ${target.name}`,
                        placement: "center",
                        content: (
                          <TradeInventoryOptions
                            participant={participant}
                            target={target}
                            onSubmittingChange={(isSubmitting) =>
                              modalStack.setDismissible(
                                tradeModalId,
                                !isSubmitting,
                              )
                            }
                            onTrade={(item) =>
                              tradeItem(participant, target, item)
                            }
                          />
                        ),
                      });
                    }}
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
    playthroughCharacters: ScenePlaythroughCharacterOption[],
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

      openActivationDialog(turnKey, participant, playthroughCharacters);

      //openTurnActionDialog(turnKey, participant);
    } catch (requestError: unknown) {
      toast.error(
        getApiError(requestError).message,
        "Unable to record movement",
      );
    }
  };

  const openActivationDialog = (
    turnKey: string,
    participant: ScenePlaythroughParticipant,
    playthroughCharacters: ScenePlaythroughCharacterOption[],
  ) => {
    setAwaitingActionTurnKey(turnKey);

    modalStack.push({
      title: "Activation",
      placement: "center",
      content: (
        <ActionDialogContent
          playthroughCharacters={playthroughCharacters}
          onSkip={() => {
            modalStack.pop();
            openTurnActionDialog(turnKey, participant);
          }}
        />
      ),
    });
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
        <SceneBackground
          key={scene?.photoUrl?.trim() ?? ""}
          photoUrl={scene?.photoUrl?.trim()}
        />
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
            <section className="rounded-3xl  p-5 ">
              {scene.participants.length === 0 ? (
                <p className="mt-5 text-content-muted">
                  No active participants were added to this scene.
                </p>
              ) : (
                <div className="mt-5 grid grid-cols-1 gap-5 sm:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4">
                  {scene.participants.map((participant) => {
                    const turnKey = `${scene.roundNumber}:${participant.id}`;
                    const isAwaitingAction = awaitingActionTurnKey === turnKey;
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
                        onShowDetails={() =>
                          modalStack.push({
                            title: `${participant.name} — Details`,
                            placement: "center",
                            content: (
                              <ParticipantDetails participant={participant} />
                            ),
                          })
                        }
                        turnPromptLabel={turnPromptLabel}
                        onTurnPrompt={() => {
                          if (isAwaitingAction) {
                            openTurnActionDialog(turnKey, participant);
                            return;
                          }

                          beginParticipantTurn(
                            scene.roundNumber,
                            participant,
                            scene.playthroughCharacters,
                          );
                        }}
                      />
                    );
                  })}
                </div>
              )}
            </section>

            <aside className="self-start rounded-3xl bg-surface/65 p-5 backdrop-blur-[2px] lg:sticky lg:top-6 lg:max-h-[calc(100vh-3rem)] lg:overflow-y-auto">
              <h2 className="text-3xl font-semibold text-content">Event Log</h2>

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
              onRemoveParticipant={(participantId) =>
                void runSceneOption(
                  `remove-${participantId}`,
                  async () => {
                    await removeSceneParticipant(
                      playthroughId,
                      sceneId,
                      participantId,
                    );
                    if (scene.currentParticipantId === participantId) {
                      setBegunTurnKey("");
                      setAwaitingActionTurnKey("");
                    }
                  },
                  "Scene character removed.",
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

      {scene?.counterattackToken && (
        <CounterAttackDialog
          key={scene.counterattackToken}
          scene={scene}
          attackAnimation={attackAnimation}
          onResolve={async (input) => {
            if (input) {
              const result = await resolveSceneParticipantAttack(
                playthroughId,
                sceneId,
                scene.counterattackerId!,
                {
                  ...input,
                  targetParticipantId: scene.counterattackTargetId,
                  isCounterattack: true,
                  counterattackToken: scene.counterattackToken!,
                },
              );
              await animateAttack(
                scene.participants.find(
                  (participant) =>
                    participant.id === scene.counterattackTargetId,
                ),
              );
              toast.success(
                getAttackResultMessage(result),
                "Counterattack complete",
              );
            } else
              await passSceneCounterattack(
                playthroughId,
                sceneId,
                scene.counterattackToken!,
              );
            setScene(await getScenePlaythrough(playthroughId, sceneId));
            setBegunTurnKey("");
            setAwaitingActionTurnKey("");
          }}
          onReload={async () =>
            setScene(await getScenePlaythrough(playthroughId, sceneId))
          }
        />
      )}
      {attackAnimation && !scene?.counterattackToken && (
        <AttackAnimationOverlay animation={attackAnimation} />
      )}
    </AppLayout>
  );
}
