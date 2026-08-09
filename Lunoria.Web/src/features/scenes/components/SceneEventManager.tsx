import { useEffect, useState, type FormEvent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faBars,
  faBolt,
  faChevronRight,
  faPen,
  faPlus,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import { useConfirmDialog, useToast } from "@/app/providers";
import { Button, FormField, Input, Select, Textarea } from "@/components/ui";
import type { JourneyCharacter } from "@/features/journeys";
import {
  createSceneEvent,
  createSceneEventAction,
  deleteSceneEvent,
  deleteSceneEventAction,
  listSceneEvents,
  reorderSceneEventActions,
  reorderSceneEvents,
  updateSceneEvent,
  updateSceneEventAction,
} from "@/features/scenes/api/scenesApi";
import {
  ActionTargetType,
  AdjustmentOperation,
  CharacterStatType,
  EventActionType,
  type Scene,
  type SceneEvent,
  type SceneEventAction,
  type SceneEventActionInput,
  type SceneEventInput,
} from "@/features/scenes/types";
import { getApiError } from "@/lib/apiClient";

interface SceneEventManagerProps {
  scene: Scene;
  journeyCharacters: JourneyCharacter[];
}

type View = "events" | "actions" | "event-form" | "action-form" | "order";

const statLabels: Record<CharacterStatType, string> = {
  [CharacterStatType.CurrentHp]: "Current HP",
  [CharacterStatType.CurrentMp]: "Current MP",
  [CharacterStatType.MaxHp]: "Max HP",
  [CharacterStatType.MaxMp]: "Max MP",
  [CharacterStatType.Movement]: "Movement",
  [CharacterStatType.MeleeAttackDamage]: "Melee attack damage",
  [CharacterStatType.BowAttackDamage]: "Bow attack damage",
};

const operationLabels: Record<AdjustmentOperation, string> = {
  [AdjustmentOperation.Add]: "Add",
  [AdjustmentOperation.Subtract]: "Subtract",
  [AdjustmentOperation.Set]: "Set",
  [AdjustmentOperation.Multiply]: "Multiply",
};

export function SceneEventManager({
  scene,
  journeyCharacters,
}: SceneEventManagerProps) {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [events, setEvents] = useState<SceneEvent[]>([]);
  const [selectedEventId, setSelectedEventId] = useState<number>();
  const [editingEvent, setEditingEvent] = useState<SceneEvent | null>();
  const [editingAction, setEditingAction] = useState<SceneEventAction | null>();
  const [view, setView] = useState<View>("events");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const selectedEvent = events.find((item) => item.id === selectedEventId);

  const load = async () => {
    setIsLoading(true);
    setError("");

    try {
      const loadedEvents = await listSceneEvents(scene.id);
      setEvents(sortItems(loadedEvents));
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    let isCurrent = true;

    void listSceneEvents(scene.id)
      .then((loadedEvents) => {
        if (isCurrent) {
          setEvents(sortItems(loadedEvents));
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
  }, [scene.id]);

  const openActions = (sceneEvent: SceneEvent) => {
    setSelectedEventId(sceneEvent.id);
    setView("actions");
  };

  const removeEvent = async (sceneEvent: SceneEvent) => {
    const confirmed = await confirm({
      title: `Delete event "${sceneEvent.name}"?`,
      message: "Its actions will also be deleted. This cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteSceneEvent(scene.id, sceneEvent.id);
      setEvents((current) => current.filter((item) => item.id !== sceneEvent.id));
      toast.success(`Event "${sceneEvent.name}" was deleted.`);
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to delete event");
    }
  };

  const removeAction = async (action: SceneEventAction) => {
    if (!selectedEvent) return;

    const confirmed = await confirm({
      title: `Delete action "${action.name}"?`,
      message: "This action cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });

    if (!confirmed) return;

    try {
      await deleteSceneEventAction(scene.id, selectedEvent.id, action.id);
      updateSelectedActions((actions) =>
        actions.filter((item) => item.id !== action.id),
      );
      toast.success(`Action "${action.name}" was deleted.`);
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to delete action");
    }
  };

  const updateSelectedActions = (
    update: (actions: SceneEventAction[]) => SceneEventAction[],
  ) => {
    setEvents((current) =>
      current.map((item) =>
        item.id === selectedEventId
          ? { ...item, sceneEventActions: update(item.sceneEventActions) }
          : item,
      ),
    );
  };

  if (isLoading) {
    return <p className="text-content-secondary">Loading events...</p>;
  }

  if (error) {
    return (
      <div className="space-y-4">
        <p className="rounded-lg border border-danger/40 p-3 text-danger" role="alert">
          {error}
        </p>
        <Button onClick={() => void load()} variant="primary">Try again</Button>
      </div>
    );
  }

  if (view === "event-form") {
    return (
      <EventForm
        event={editingEvent}
        onCancel={() => setView("events")}
        onSave={async (input) => {
          const saved = editingEvent
            ? await updateSceneEvent(scene.id, editingEvent.id, input)
            : await createSceneEvent(scene.id, input);

          setEvents((current) =>
            sortItems(
              editingEvent
                ? current.map((item) => (item.id === saved.id ? saved : item))
                : [...current, saved],
            ),
          );
          setView("events");
          toast.success(`Event "${saved.name}" was ${editingEvent ? "updated" : "created"}.`);
        }}
      />
    );
  }

  if (view === "action-form" && selectedEvent) {
    return (
      <ActionForm
        action={editingAction}
        journeyCharacters={journeyCharacters}
        onCancel={() => setView("actions")}
        onSave={async (input) => {
          const saved = editingAction
            ? await updateSceneEventAction(
                scene.id,
                selectedEvent.id,
                editingAction.id,
                input,
              )
            : await createSceneEventAction(scene.id, selectedEvent.id, input);

          updateSelectedActions((actions) =>
            sortItems(
              editingAction
                ? actions.map((item) => (item.id === saved.id ? saved : item))
                : [...actions, saved],
            ),
          );
          setView("actions");
          toast.success(`Action "${saved.name}" was ${editingAction ? "updated" : "created"}.`);
        }}
      />
    );
  }

  if (view === "order") {
    if (selectedEvent) {
      return (
        <OrderEditor
          title="action"
          items={selectedEvent.sceneEventActions}
          onCancel={() => setView("actions")}
          onSave={async (orderedActions) => {
            const order = orderedActions.map((item, sortOrder) => ({
              id: item.id,
              sortOrder,
            }));

            await reorderSceneEventActions(scene.id, selectedEvent.id, order);
            updateSelectedActions(() =>
              orderedActions.map((item, sortOrder) => ({
                ...item,
                sortOrder,
              })),
            );
            setView("actions");
            toast.success("Action order was updated.");
          }}
        />
      );
    }

    return (
      <OrderEditor
        title="event"
        items={events}
        onCancel={() => setView("events")}
        onSave={async (orderedEvents) => {
          const order = orderedEvents.map((item, sortOrder) => ({
            id: item.id,
            sortOrder,
          }));

          await reorderSceneEvents(scene.id, order);
          setEvents(
            orderedEvents.map((item, sortOrder) => ({ ...item, sortOrder })),
          );
          setView("events");
          toast.success("Event order was updated.");
        }}
      />
    );
  }

  if (view === "actions" && selectedEvent) {
    const actions = sortItems(selectedEvent.sceneEventActions);

    return (
      <div>
        <Button
          onClick={() => {
            setSelectedEventId(undefined);
            setView("events");
          }}
          leftIcon={<FontAwesomeIcon icon={faArrowLeft} />}
          size="sm"
        >
          All events
        </Button>

        <div className="mt-5 flex items-start justify-between gap-4">
          <div>
            <h3 className="text-2xl font-semibold text-content">{selectedEvent.name}</h3>
            {selectedEvent.description && (
              <p className="mt-1 text-sm text-content-secondary">{selectedEvent.description}</p>
            )}
          </div>
          <div className="flex shrink-0 gap-2">
            <Button
              onClick={() => setView("order")}
              disabled={actions.length < 2}
              leftIcon={<FontAwesomeIcon icon={faBars} />}
              size="sm"
            >
              Sort
            </Button>
            <Button
              onClick={() => {
                setEditingAction(null);
                setView("action-form");
              }}
              variant="add"
              leftIcon={<FontAwesomeIcon icon={faPlus} />}
              size="sm"
            >
              Add action
            </Button>
          </div>
        </div>

        {actions.length === 0 ? (
          <EmptyState title="No actions yet" message="Add the first action for this event." />
        ) : (
          <div className="mt-5 space-y-3">
            {actions.map((action) => (
              <ActionCard
                key={action.id}
                action={action}
                onEdit={() => {
                  setEditingAction(action);
                  setView("action-form");
                }}
                onDelete={() => void removeAction(action)}
              />
            ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <div>
      <div className="flex items-center justify-between gap-3">
        <p className="text-sm text-content-secondary">
          Events run in the order shown for {scene.name}.
        </p>
        <div className="flex shrink-0 gap-2">
          <Button
            onClick={() => setView("order")}
            disabled={events.length < 2}
            leftIcon={<FontAwesomeIcon icon={faBars} />}
            size="sm"
          >
            Sort
          </Button>
          <Button
            onClick={() => {
              setEditingEvent(null);
              setView("event-form");
            }}
            variant="add"
            leftIcon={<FontAwesomeIcon icon={faPlus} />}
            size="sm"
          >
            Add event
          </Button>
        </div>
      </div>

      {events.length === 0 ? (
        <EmptyState title="No events yet" message="Add the first event for this scene." />
      ) : (
        <div className="mt-5 space-y-3">
          {events.map((sceneEvent) => (
            <article key={sceneEvent.id} className="rounded-xl border border-border bg-surface p-4">
              <button
                type="button"
                onClick={() => openActions(sceneEvent)}
                className="flex w-full cursor-pointer items-center gap-3 text-left"
              >
                <span className="flex size-10 shrink-0 items-center justify-center rounded-lg bg-utility/15 text-utility-hover">
                  <FontAwesomeIcon icon={faBolt} />
                </span>
                <span className="min-w-0 flex-1">
                  <span className="block font-semibold text-content">{sceneEvent.name}</span>
                  <span className="mt-0.5 block text-sm text-content-secondary">
                    {sceneEvent.sceneEventActions.length} {sceneEvent.sceneEventActions.length === 1 ? "action" : "actions"}
                  </span>
                </span>
                <FontAwesomeIcon icon={faChevronRight} className="text-content-muted" />
              </button>
              {sceneEvent.description && (
                <p className="mt-3 border-t border-border pt-3 text-sm text-content-secondary">
                  {sceneEvent.description}
                </p>
              )}
              <div className="mt-3 flex justify-end gap-2">
                <Button
                  onClick={() => void removeEvent(sceneEvent)}
                  variant="danger"
                  leftIcon={<FontAwesomeIcon icon={faTrash} />}
                  size="sm"
                >
                  Delete
                </Button>
                <Button
                  onClick={() => {
                    setEditingEvent(sceneEvent);
                    setView("event-form");
                  }}
                  variant="primary"
                  leftIcon={<FontAwesomeIcon icon={faPen} />}
                  size="sm"
                >
                  Edit
                </Button>
              </div>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

function EventForm({ event, onSave, onCancel }: {
  event: SceneEvent | null | undefined;
  onSave: (input: SceneEventInput) => Promise<void>;
  onCancel: () => void;
}) {
  const [name, setName] = useState(event?.name ?? "");
  const [description, setDescription] = useState(event?.description ?? "");
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const submit = async (submitEvent: FormEvent) => {
    submitEvent.preventDefault();
    setIsSaving(true);
    setError("");
    try {
      await onSave({ name, description: description || null });
    } catch (requestError) {
      setError(getApiError(requestError).message);
      setIsSaving(false);
    }
  };

  return (
    <form onSubmit={(submitEvent) => void submit(submitEvent)} className="space-y-5">
      <h3 className="text-2xl font-semibold text-content">{event ? "Edit event" : "Add event"}</h3>
      <FormField htmlFor="event-name" label="Name">
        <Input id="event-name" value={name} onChange={(e) => setName(e.target.value)} maxLength={200} required />
      </FormField>
      <FormField htmlFor="event-description" label="Description">
        <Textarea id="event-description" value={description} onChange={(e) => setDescription(e.target.value)} maxLength={2000} rows={5} />
      </FormField>
      <FormActions isSaving={isSaving} error={error} onCancel={onCancel} />
    </form>
  );
}

function ActionForm({ action, journeyCharacters, onSave, onCancel }: {
  action: SceneEventAction | null | undefined;
  journeyCharacters: JourneyCharacter[];
  onSave: (input: SceneEventActionInput) => Promise<void>;
  onCancel: () => void;
}) {
  const adjustment = action?.characterStatAdjustmentAction;
  const [name, setName] = useState(action?.name ?? "");
  const [targetType, setTargetType] = useState(action?.actionTargetType ?? ActionTargetType.AllJourneyCharacters);
  const [statType, setStatType] = useState(adjustment?.characterStatType ?? CharacterStatType.CurrentHp);
  const [operation, setOperation] = useState(adjustment?.adjustmentOperation ?? AdjustmentOperation.Add);
  const [value, setValue] = useState(String(adjustment?.value ?? 0));
  const [characterId, setCharacterId] = useState(adjustment?.characterId ? String(adjustment.characterId) : "");
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const submit = async (submitEvent: FormEvent) => {
    submitEvent.preventDefault();
    setIsSaving(true);
    setError("");
    try {
      await onSave({
        name,
        actionTargetType: targetType,
        eventActionType: EventActionType.CharacterStatAdjustment,
        characterStatType: statType,
        adjustmentOperation: operation,
        value: Number(value),
        characterId: targetType === ActionTargetType.SingleJourneyCharacter ? Number(characterId) : null,
      });
    } catch (requestError) {
      setError(getApiError(requestError).message);
      setIsSaving(false);
    }
  };

  return (
    <form onSubmit={(submitEvent) => void submit(submitEvent)} className="space-y-5">
      <h3 className="text-2xl font-semibold text-content">{action ? "Edit action" : "Add action"}</h3>
      <FormField htmlFor="action-name" label="Name">
        <Input id="action-name" value={name} onChange={(e) => setName(e.target.value)} maxLength={200} required />
      </FormField>
      <FormField htmlFor="action-target" label="Target">
        <Select id="action-target" value={targetType} onChange={(e) => setTargetType(Number(e.target.value) as ActionTargetType)}>
          <option value={ActionTargetType.AllJourneyCharacters}>All journey characters</option>
          <option value={ActionTargetType.SingleJourneyCharacter}>One journey character</option>
        </Select>
      </FormField>
      {targetType === ActionTargetType.SingleJourneyCharacter && (
        <FormField htmlFor="action-character" label="Character">
          <Select id="action-character" value={characterId} onChange={(e) => setCharacterId(e.target.value)} required>
            <option value="" disabled>Select a character</option>
            {journeyCharacters.map((item) => (
              <option key={item.characterId} value={item.characterId}>{item.character.name}</option>
            ))}
          </Select>
        </FormField>
      )}
      <div className="grid gap-4 sm:grid-cols-2">
        <FormField htmlFor="action-stat" label="Stat">
          <Select id="action-stat" value={statType} onChange={(e) => setStatType(Number(e.target.value) as CharacterStatType)}>
            {enumEntries(statLabels).map(([id, label]) => <option key={id} value={id}>{label}</option>)}
          </Select>
        </FormField>
        <FormField htmlFor="action-operation" label="Operation">
          <Select id="action-operation" value={operation} onChange={(e) => setOperation(Number(e.target.value) as AdjustmentOperation)}>
            {enumEntries(operationLabels).map(([id, label]) => <option key={id} value={id}>{label}</option>)}
          </Select>
        </FormField>
      </div>
      <FormField htmlFor="action-value" label="Value">
        <Input id="action-value" type="number" value={value} onChange={(e) => setValue(e.target.value)} required />
      </FormField>
      <FormActions isSaving={isSaving} error={error} onCancel={onCancel} />
    </form>
  );
}

function ActionCard({ action, onEdit, onDelete }: { action: SceneEventAction; onEdit: () => void; onDelete: () => void }) {
  const adjustment = action.characterStatAdjustmentAction;
  const target = action.actionTargetType === ActionTargetType.AllJourneyCharacters
    ? "All journey characters"
    : adjustment?.character?.name ?? "Selected character";

  return (
    <article className="rounded-xl border border-border bg-surface p-4">
      <h4 className="font-semibold text-content">{action.name}</h4>
      {adjustment && (
        <p className="mt-1 text-sm text-content-secondary">
          {operationLabels[adjustment.adjustmentOperation]} {adjustment.value} to {statLabels[adjustment.characterStatType]} · {target}
        </p>
      )}
      <div className="mt-3 flex justify-end gap-2">
        <Button onClick={onDelete} variant="danger" size="sm" leftIcon={<FontAwesomeIcon icon={faTrash} />}>Delete</Button>
        <Button onClick={onEdit} variant="primary" size="sm" leftIcon={<FontAwesomeIcon icon={faPen} />}>Edit</Button>
      </div>
    </article>
  );
}

function OrderEditor<T extends { id: number; name: string; sortOrder: number }>({ title, items, onSave, onCancel }: {
  title: string;
  items: T[];
  onSave: (items: T[]) => Promise<void>;
  onCancel: () => void;
}) {
  const [ordered, setOrdered] = useState(() => sortItems(items));
  const [draggedId, setDraggedId] = useState<number>();
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const move = (targetId: number) => {
    if (draggedId === undefined || draggedId === targetId) return;
    setOrdered((current) => {
      const from = current.findIndex((item) => item.id === draggedId);
      const to = current.findIndex((item) => item.id === targetId);
      if (from < 0 || to < 0) return current;
      const next = [...current];
      const [dragged] = next.splice(from, 1);
      next.splice(to, 0, dragged);
      return next;
    });
  };

  const save = async () => {
    setIsSaving(true);
    setError("");
    try { await onSave(ordered); } catch (requestError) {
      setError(getApiError(requestError).message);
      setIsSaving(false);
    }
  };

  return (
    <div>
      <h3 className="text-2xl font-semibold text-content">Sort {title}s</h3>
      <p className="mt-1 text-sm text-content-secondary">Drag items into the order they should run.</p>
      <ol className="mt-5 space-y-3">
        {ordered.map((item, index) => (
          <li key={item.id} draggable={!isSaving} onDragStart={() => setDraggedId(item.id)} onDragOver={(e) => { e.preventDefault(); move(item.id); }} onDragEnd={() => setDraggedId(undefined)} className={`flex cursor-grab items-center gap-4 rounded-xl border bg-surface p-4 ${draggedId === item.id ? "border-brand opacity-50" : "border-border"}`}>
            <FontAwesomeIcon icon={faBars} className="text-content-muted" />
            <span className="w-6 text-center text-sm text-content-muted">{index + 1}</span>
            <span className="font-semibold text-content">{item.name}</span>
          </li>
        ))}
      </ol>
      <FormActions isSaving={isSaving} error={error} onCancel={onCancel} onSave={() => void save()} />
    </div>
  );
}

function FormActions({ isSaving, error, onCancel, onSave }: { isSaving: boolean; error: string; onCancel: () => void; onSave?: () => void }) {
  return (
    <div className="border-t border-border pt-4">
      {error && <p className="mb-3 text-sm text-danger" role="alert">{error}</p>}
      <div className="flex justify-end gap-3">
        <Button onClick={onCancel} disabled={isSaving}>Cancel</Button>
        <Button type={onSave ? "button" : "submit"} onClick={onSave} disabled={isSaving} variant="primary">
          {isSaving ? "Saving..." : "Save"}
        </Button>
      </div>
    </div>
  );
}

function EmptyState({ title, message }: { title: string; message: string }) {
  return (
    <div className="mt-5 rounded-xl border border-border bg-surface/60 p-8 text-center">
      <h3 className="text-xl font-semibold text-content">{title}</h3>
      <p className="mt-2 text-sm text-content-secondary">{message}</p>
    </div>
  );
}

function sortItems<T extends { sortOrder: number }>(items: T[]): T[] {
  return [...items].sort((a, b) => a.sortOrder - b.sortOrder);
}

function enumEntries<T extends number>(labels: Record<T, string>): [T, string][] {
  return Object.entries(labels).map(([id, label]) => [Number(id) as T, label as string]);
}
