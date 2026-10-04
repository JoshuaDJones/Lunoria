import { useEffect, useRef, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowDown,
  faArrowUp,
  faChevronDown,
  faGripVertical,
  faPlus,
  faSpinner,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import { Button } from "@/components/ui";
import { useConfirmDialog, useToast } from "@/app/providers";
import {
  CharacterType,
  listCharacters,
  type Character,
} from "@/features/characters";
import {
  addJourneyPlayers,
  deleteJourneyCharacter,
  getJourney,
  reorderJourneyPlayers,
  updateJourneyCharacter,
} from "@/features/journeys/api/journeysApi";
import { CharacterSyncButton } from "@/features/characterSync/components/CharacterSyncButton";
import { JourneyCharacterForm } from "@/features/journeys/components/JourneyCharacterForm";
import { JourneyPlayerSelection } from "@/features/journeys/components/JourneyPlayerSelection";
import { DialogAccordionPanel } from "@/features/scenes/components/DialogAccordionPanel";
import type { JourneyCharacter } from "@/features/journeys/types";
import { getApiError } from "@/lib/apiClient";

interface Props {
  journeyId: number;
  journeyCharacters: JourneyCharacter[];
  onRosterChanged: (characters: JourneyCharacter[]) => void;
  onBusyChange: (busy: boolean) => void;
  onDirtyChange: (dirty: boolean) => void;
}

export function JourneyCharacterPicker({
  journeyId,
  journeyCharacters,
  onRosterChanged,
  onBusyChange,
  onDirtyChange,
}: Props) {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [characters, setCharacters] = useState<Character[]>([]);
  const [loading, setLoading] = useState(true);
  const [catalogError, setCatalogError] = useState("");
  const [reloadKey, setReloadKey] = useState(0);
  const [adding, setAdding] = useState(false);
  const [editingId, setEditingId] = useState<number>();
  const [dirty, setDirty] = useState(false);
  const [busy, setBusy] = useState(false);
  const busyRef = useRef(false);
  const [error, setError] = useState("");
  const [needsReload, setNeedsReload] = useState(false);
  const [dragId, setDragId] = useState<number>();
  const [dropIndex, setDropIndex] = useState<number>();
  const roster = [...journeyCharacters].sort(
    (a, b) => a.sortOrder - b.sortOrder || a.id - b.id,
  );
  const locked = busy || needsReload;

  useEffect(() => {
    onDirtyChange(dirty);
  }, [dirty, onDirtyChange]);
  useEffect(() => {
    let current = true;
    const load = async () => {
      const all: Character[] = [];
      for (let skip = 0; ; skip += 100) {
        const page = await listCharacters({
          typeFilter: CharacterType.Player,
          skip,
          take: 100,
        });
        all.push(...page);
        if (page.length < 100) return all;
      }
    };
    void load()
      .then((result) => {
        if (current) setCharacters(result);
      })
      .catch((e: unknown) => {
        if (current) setCatalogError(getApiError(e).message);
      })
      .finally(() => {
        if (current) setLoading(false);
      });
    return () => {
      current = false;
    };
  }, [reloadKey]);

  const retryCatalog = () => {
    setCatalogError("");
    setLoading(true);
    setReloadKey((key) => key + 1);
  };
  const setWorking = (value: boolean) => {
    busyRef.current = value;
    setBusy(value);
    onBusyChange(value);
  };
  const refresh = async () => {
    const journey = await getJourney(journeyId);
    onRosterChanged(journey.journeyCharacters ?? []);
    setNeedsReload(false);
  };
  const changeView = async (id?: number, add = false) => {
    if (busyRef.current) return;
    if (
      dirty &&
      !(await confirm({
        title: "Discard player edits?",
        message: "Your unsaved changes will be lost.",
        confirmLabel: "Discard changes",
        variant: "danger",
      }))
    )
      return;
    setDirty(false);
    setEditingId(id);
    setAdding(add);
  };
  const mutate = async (action: () => Promise<void>, message: string) => {
    if (busyRef.current) return;
    setWorking(true);
    setError("");
    let saved = false;
    try {
      await action();
      saved = true;
      await refresh();
      toast.success(message);
    } catch (e) {
      setError(
        saved
          ? "Changes were saved, but the roster could not be refreshed. Reload before continuing."
          : getApiError(e).message,
      );
      try {
        await refresh();
      } catch {
        setNeedsReload(true);
      }
    } finally {
      setWorking(false);
    }
  };
  const move = async (from: number, to: number) => {
    setDragId(undefined);
    setDropIndex(undefined);
    if (
      locked ||
      busyRef.current ||
      dirty ||
      from === to ||
      to < 0 ||
      to >= roster.length
    )
      return;
    const next = [...roster];
    const [moved] = next.splice(from, 1);
    next.splice(to, 0, moved);
    setEditingId(undefined);
    onRosterChanged(next.map((item, sortOrder) => ({ ...item, sortOrder })));
    await mutate(async () => {
      try {
        await reorderJourneyPlayers(
          journeyId,
          next.map((item) => item.id),
        );
      } catch (e) {
        onRosterChanged(roster);
        throw e;
      }
    }, "Player order updated.");
  };

  return (
    <div className="space-y-4">
      {error && (
        <div
          role="alert"
          className="rounded-xl border border-danger/40 p-3 text-sm text-danger"
        >
          {error}
          {needsReload && (
            <Button
              disabled={busy}
              className="mt-2"
              onClick={() => void mutate(async () => {}, "Roster reloaded.")}
            >
              Reload roster
            </Button>
          )}
        </div>
      )}
      {adding ? (
        <JourneyPlayerSelection
          characters={characters.filter(
            (character) =>
              !character.isAlternateFormOnly &&
              !roster.some((item) => item.characterId === character.id),
          )}
          loading={loading}
          error={catalogError}
          busy={locked}
          onRetry={retryCatalog}
          onDirtyChange={setDirty}
          onBack={() => void changeView()}
          onAdd={async (ids) => {
            await mutate(async () => {
              await addJourneyPlayers(journeyId, ids);
              setDirty(false);
              setAdding(false);
            }, "Players added.");
          }}
        />
      ) : (
        <>
          <div className="flex items-center justify-between gap-3">
            <h3 className="text-lg font-semibold text-content">Players</h3>
            <Button
              variant="primary"
              disabled={locked}
              onClick={() => void changeView(undefined, true)}
              leftIcon={<FontAwesomeIcon icon={faPlus} />}
            >
              Add players
            </Button>
          </div>
          <p className="text-sm text-content-secondary">
            Drag players into turn order, or use the arrows. Expand a player to
            adjust their journey settings. Changes apply to new playthroughs.
          </p>
          <div
            role="status"
            aria-live="polite"
            className="text-sm text-content-muted"
          >
            {busy && (
              <>
                <FontAwesomeIcon icon={faSpinner} spin className="mr-2" />
                Saving changes
              </>
            )}
          </div>
          {roster.length === 0 && (
            <p className="rounded-xl border border-dashed border-border p-6 text-center text-content-muted">
              No players yet. Add players to build your journey roster.
            </p>
          )}
          <div
            className="space-y-3"
            onDragLeave={(event) => {
              if (!event.currentTarget.contains(event.relatedTarget as Node))
                setDropIndex(undefined);
            }}
          >
            {roster.map((assignment, index) => {
              const expanded = editingId === assignment.id;
              const status = assignment.syncStatus;
              return (
                <article
                  key={assignment.id}
                  onDragOver={(event) => {
                    if (dragId === undefined || locked || dirty) return;
                    event.preventDefault();
                    event.dataTransfer.dropEffect = "move";
                    const rect = event.currentTarget.getBoundingClientRect();
                    setDropIndex(
                      index +
                        (event.clientY > rect.top + rect.height / 2 ? 1 : 0),
                    );
                  }}
                  onDrop={(event) => {
                    event.preventDefault();
                    if (dragId === undefined || dropIndex === undefined) return;
                    const from = roster.findIndex((item) => item.id === dragId);
                    if (from >= 0)
                      void move(
                        from,
                        dropIndex > from ? dropIndex - 1 : dropIndex,
                      );
                  }}
                  className={`relative rounded-xl border bg-surface transition-colors ${expanded ? "border-add/60" : "border-border"} ${dragId === assignment.id ? "opacity-50" : ""}`}
                >
                  {dropIndex === index && (
                    <div className="pointer-events-none absolute -top-2 right-0 left-0 h-1 rounded bg-add" />
                  )}
                  <div className="flex items-center gap-2 p-3">
                    <button
                      type="button"
                      draggable={!locked && !dirty}
                      disabled={locked || dirty}
                      aria-label={`Drag ${assignment.character.name} to reorder; use arrow keys to move`}
                      className="cursor-grab p-2 text-content-muted active:cursor-grabbing disabled:opacity-40"
                      onDragStart={(event) => {
                        setDragId(assignment.id);
                        event.dataTransfer.setData(
                          "text/plain",
                          String(assignment.id),
                        );
                        event.dataTransfer.effectAllowed = "move";
                      }}
                      onDragEnd={() => {
                        setDragId(undefined);
                        setDropIndex(undefined);
                      }}
                      onKeyDown={(event) => {
                        if (
                          event.key === "ArrowUp" ||
                          event.key === "ArrowDown"
                        ) {
                          event.preventDefault();
                          void move(
                            index,
                            index + (event.key === "ArrowUp" ? -1 : 1),
                          );
                        }
                      }}
                    >
                      <FontAwesomeIcon icon={faGripVertical} />
                    </button>
                    <span className="text-sm tabular-nums text-content-muted">
                      {index + 1}
                    </span>
                    <button
                      type="button"
                      disabled={locked}
                      aria-expanded={expanded}
                      aria-controls={`player-settings-${assignment.id}`}
                      onClick={() =>
                        void changeView(expanded ? undefined : assignment.id)
                      }
                      className="flex min-w-0 flex-1 cursor-pointer items-center gap-3 text-left"
                    >
                      {assignment.character.photoUrl && (
                        <img
                          draggable={false}
                          src={assignment.character.photoUrl}
                          alt=""
                          className="size-12 shrink-0 rounded-lg object-contain"
                        />
                      )}
                      <span className="min-w-0 flex-1">
                        <span className="block truncate font-semibold text-content">
                          {assignment.character.name}
                        </span>
                        <span className="block text-xs text-content-muted">
                          HP {assignment.maxHp} · MP {assignment.maxMp} ·{" "}
                          {assignment.isInitiallyActive
                            ? "Active at start"
                            : "Inactive at start"}
                        </span>
                      </span>
                      <FontAwesomeIcon
                        icon={faChevronDown}
                        className={`text-content-muted transition-transform ${expanded ? "rotate-180" : ""}`}
                      />
                    </button>
                    <div className="flex flex-col">
                      <Button
                        size="sm"
                        className="px-2 py-1"
                        aria-label={`Move ${assignment.character.name} up`}
                        disabled={locked || dirty || index === 0}
                        onClick={() => void move(index, index - 1)}
                      >
                        <FontAwesomeIcon icon={faArrowUp} />
                      </Button>
                      <Button
                        size="sm"
                        className="px-2 py-1"
                        aria-label={`Move ${assignment.character.name} down`}
                        disabled={
                          locked || dirty || index === roster.length - 1
                        }
                        onClick={() => void move(index, index + 1)}
                      >
                        <FontAwesomeIcon icon={faArrowDown} />
                      </Button>
                    </div>
                  </div>
                  {!expanded &&
                    status &&
                    (status.hasUpdates ||
                      status.requiresReview ||
                      !status.baseAvailable) && (
                      <div className="px-4 pb-3">
                        <CharacterSyncButton
                          kind="journey"
                          assignmentId={assignment.id}
                          name={assignment.character.name}
                          status={status}
                          disabled={locked || dirty}
                          onUpdated={refresh}
                        />
                      </div>
                    )}
                  <DialogAccordionPanel
                    id={`player-settings-${assignment.id}`}
                    open={expanded}
                  >
                    <div className="space-y-4 border-t border-border p-4">
                      {loading ? (
                        <p role="status">Loading player settings…</p>
                      ) : catalogError ? (
                        <div role="alert">
                          {catalogError}
                          <Button onClick={retryCatalog}>Retry</Button>
                        </div>
                      ) : (
                        expanded && (
                          <JourneyCharacterForm
                            disabled={locked}
                            key={assignment.id}
                            assignment={assignment}
                            characters={characters}
                            onDirtyChange={setDirty}
                            onBusyChange={setWorking}
                            onCancel={() => void changeView()}
                            onSyncUpdated={async () => {
                              await refresh();
                              setEditingId(undefined);
                            }}
                            onSave={async (request) => {
                              const updated = await updateJourneyCharacter(
                                assignment.id,
                                request,
                              );
                              onRosterChanged(
                                journeyCharacters.map((item) =>
                                  item.id === updated.id ? updated : item,
                                ),
                              );
                              setDirty(false);
                              setEditingId(undefined);
                              toast.success("Player settings saved.");
                            }}
                          />
                        )
                      )}
                      <Button
                        size="sm"
                        disabled={locked}
                        leftIcon={<FontAwesomeIcon icon={faTrash} />}
                        onClick={async () => {
                          if (
                            !(await confirm({
                              title: `Remove ${assignment.character.name}?`,
                              message:
                                "Remove this player and their journey-specific settings? Existing playthroughs and the base character are unchanged." +
                                (dirty
                                  ? " Unsaved edits will be discarded."
                                  : ""),
                              confirmLabel: "Remove player",
                              variant: "danger",
                            }))
                          )
                            return;
                          await mutate(async () => {
                            await deleteJourneyCharacter(assignment.id);
                            setEditingId(undefined);
                            setDirty(false);
                          }, "Player removed.");
                        }}
                      >
                        Remove player
                      </Button>
                    </div>
                  </DialogAccordionPanel>
                  {index === roster.length - 1 &&
                    dropIndex === roster.length && (
                      <div className="pointer-events-none absolute -bottom-2 right-0 left-0 h-1 rounded bg-add" />
                    )}
                </article>
              );
            })}
          </div>
        </>
      )}
    </div>
  );
}
