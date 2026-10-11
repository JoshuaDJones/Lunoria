import { useId, useRef, useState, type FormEvent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faGripVertical,
  faChevronDown,
  faPlus,
  faSpinner,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import { Button, FormField, Input, Textarea } from "@/components/ui";
import type { JourneyCharacter } from "@/features/journeys";
import { SceneEventActionEditor } from "@/features/scenes/components/SceneEventActionEditor";
import {
  actionSummary,
  actionToInput,
  draftAction,
} from "@/features/scenes/utils/sceneEventDraft";
import {
  ActionTargetType,
  EventActionType,
  type SceneEvent,
  type SceneEventAction,
  type SceneEventDraftInput,
} from "@/features/scenes/types";
import { getApiError } from "@/lib/apiClient";

interface Props {
  event: SceneEvent | null;
  journeyCharacters: JourneyCharacter[];
  onSave: (input: SceneEventDraftInput) => Promise<void>;
  onCancel: () => void;
  onBusyChange?: (busy: boolean) => void;
}

export function SceneEventDraftEditor({
  event,
  journeyCharacters,
  onSave,
  onCancel,
  onBusyChange,
}: Props) {
  const formId = useId();
  const nextId = useRef(-1);
  const [name, setName] = useState(event?.name ?? "");
  const [description, setDescription] = useState(event?.description ?? "");
  const [actions, setActions] = useState<SceneEventAction[]>(() =>
    [...(event?.sceneEventActions ?? [])].sort(
      (a, b) => a.sortOrder - b.sortOrder,
    ),
  );
  const [editingId, setEditingId] = useState<number>();
  const [newId, setNewId] = useState<number>();
  const [draggedId, setDraggedId] = useState<number>();
  const [saving, setSaving] = useState(false);
  const savingRef = useRef(false);
  const [error, setError] = useState("");

  const move = (from: number, to: number) => {
    if (
      saving ||
      editingId !== undefined ||
      from < 0 ||
      to < 0 ||
      to >= actions.length
    )
      return;
    setActions((current) => {
      const next = [...current];
      const [item] = next.splice(from, 1);
      next.splice(to, 0, item);
      return next;
    });
  };
  const addAction = () => {
    const id = nextId.current--;
    setActions((current) => [
      ...current,
      draftAction(
        {
          name: "",
          actionTargetType: ActionTargetType.AllJourneyCharacters,
          eventActionType: EventActionType.CharacterStatAdjustment,
        },
        id,
        {},
        journeyCharacters,
      ),
    ]);
    setNewId(id);
    setEditingId(id);
  };
  const submit = async (submitEvent: FormEvent) => {
    submitEvent.preventDefault();
    if (savingRef.current) return;
    if (editingId !== undefined) {
      setError("Apply or cancel the open action before saving the event.");
      return;
    }
    if (!name.trim() || actions.length === 0) {
      setError("Enter an event name and add at least one action.");
      return;
    }
    savingRef.current = true;
    setSaving(true);
    setError("");
    onBusyChange?.(true);
    try {
      await onSave({
        name: name.trim(),
        description: description.trim() || null,
        actions: actions.map(actionToInput),
      });
    } catch (reason) {
      setError(getApiError(reason).message);
    } finally {
      savingRef.current = false;
      setSaving(false);
      onBusyChange?.(false);
    }
  };
  return (
    <section className="space-y-5" aria-busy={saving}>
      <h3 className="text-2xl font-semibold text-content">
        {event ? "Edit Event" : "Create Event"}
      </h3>
      <fieldset disabled={saving} className="min-w-0 space-y-5">
        <form
          id={formId}
          onSubmit={(submitEvent) => void submit(submitEvent)}
          className="space-y-4"
        >
          <FormField htmlFor={`${formId}-name`} label="Name">
            <Input
              id={`${formId}-name`}
              required
              maxLength={200}
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
          </FormField>
          <FormField htmlFor={`${formId}-description`} label="Description">
            <Textarea
              id={`${formId}-description`}
              maxLength={2000}
              rows={3}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </FormField>
        </form>
        <div className="space-y-3">
          <div>
            <h4 className="font-semibold text-content">Actions</h4>
            <p className="mt-1 text-sm text-content-muted">
              Actions run in this order at scene start. Changes remain a draft
              until you save the event.
            </p>
          </div>
          {actions.length === 0 && (
            <p className="rounded-xl border border-dashed border-border p-5 text-sm text-content-secondary">
              Add an action to choose what happens in this event.
            </p>
          )}
          {actions.map((action, index) => {
            const expanded = editingId === action.id;
            const locked = saving || editingId !== undefined;
            return (
              <article
                key={action.id}
                className={`rounded-xl border bg-surface ${expanded ? "border-brand" : "border-border"} ${draggedId === action.id ? "opacity-50" : ""}`}
                onDragOver={(e) => {
                  if (!locked) e.preventDefault();
                }}
                onDrop={(e) => {
                  e.preventDefault();
                  if (draggedId !== undefined)
                    move(
                      actions.findIndex((item) => item.id === draggedId),
                      index,
                    );
                  setDraggedId(undefined);
                }}
              >
                <div className="flex items-center gap-2 p-3">
                  <button
                    type="button"
                    disabled={locked || actions.length < 2}
                    draggable={!locked && actions.length > 1}
                    aria-label={`Reorder ${action.name || "action"}. Use up and down arrow keys to move.`}
                    title="Drag to reorder"
                    className="cursor-grab p-2 text-content-muted active:cursor-grabbing disabled:opacity-40"
                    onDragStart={(event) => {
                      event.dataTransfer.effectAllowed = "move";
                      event.dataTransfer.setData(
                        "text/plain",
                        String(action.id),
                      );
                      const card = event.currentTarget.closest("article");
                      if (card) event.dataTransfer.setDragImage(card, 20, 20);
                      setDraggedId(action.id);
                    }}
                    onDragEnd={() => setDraggedId(undefined)}
                    onKeyDown={(event) => {
                      if (
                        event.key === "ArrowUp" ||
                        event.key === "ArrowDown"
                      ) {
                        event.preventDefault();
                        move(index, index + (event.key === "ArrowUp" ? -1 : 1));
                      }
                    }}
                  >
                    <FontAwesomeIcon icon={faGripVertical} aria-hidden="true" />
                  </button>
                  <button
                    type="button"
                    disabled={locked}
                    aria-expanded={expanded}
                    aria-controls={`${formId}-action-${action.id}`}
                    onClick={() => setEditingId(action.id)}
                    className="min-w-0 flex-1 text-left disabled:cursor-default"
                  >
                    <span className="block break-words font-semibold text-content">
                      {action.name || "New action"}
                    </span>
                    {action.name && (
                      <span className="mt-1 block break-words text-sm text-content-secondary">
                        {actionSummary(action, journeyCharacters)}
                      </span>
                    )}
                  </button>
                  <button
                    type="button"
                    disabled={locked}
                    aria-label={`Edit ${action.name || "action"}`}
                    aria-expanded={expanded}
                    aria-controls={`${formId}-action-${action.id}`}
                    onClick={() => setEditingId(action.id)}
                    className="shrink-0 cursor-pointer rounded p-2 text-content-muted hover:text-content focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand disabled:cursor-default disabled:opacity-40"
                  >
                    <FontAwesomeIcon
                      icon={faChevronDown}
                      aria-hidden="true"
                      className={`transition-transform ${expanded ? "rotate-180" : ""}`}
                    />
                  </button>
                </div>
                {expanded ? (
                  <div
                    id={`${formId}-action-${action.id}`}
                    className="border-t border-border p-4"
                  >
                    <SceneEventActionEditor
                      action={newId === action.id ? null : action}
                      journeyCharacters={journeyCharacters}
                      onCancel={() => {
                        if (newId === action.id)
                          setActions((current) =>
                            current.filter((item) => item.id !== action.id),
                          );
                        setEditingId(undefined);
                        setNewId(undefined);
                      }}
                      onSave={async (input, labels) => {
                        setActions((current) =>
                          current.map((item) =>
                            item.id === action.id
                              ? draftAction(
                                  input,
                                  item.id,
                                  labels,
                                  journeyCharacters,
                                )
                              : item,
                          ),
                        );
                        setEditingId(undefined);
                        setNewId(undefined);
                        setError("");
                      }}
                    />
                  </div>
                ) : (
                  <div className="flex flex-wrap justify-end gap-2 px-4 pb-4">
                    <Button
                      size="sm"
                      variant="danger"
                      disabled={locked}
                      leftIcon={<FontAwesomeIcon icon={faTrash} />}
                      onClick={() =>
                        setActions((current) =>
                          current.filter((item) => item.id !== action.id),
                        )
                      }
                    >
                      Remove
                    </Button>
                    <Button
                      size="sm"
                      disabled={locked}
                      onClick={() => setEditingId(action.id)}
                    >
                      Edit Action
                    </Button>
                  </div>
                )}
              </article>
            );
          })}
          <Button
            disabled={editingId !== undefined || actions.length >= 100}
            leftIcon={<FontAwesomeIcon icon={faPlus} />}
            onClick={addAction}
          >
            Add Action
          </Button>
        </div>
        {error && (
          <p role="alert" className="text-sm text-danger">
            {error}
          </p>
        )}
        <div className="flex flex-wrap justify-end gap-3 border-t border-border pt-4">
          <Button onClick={onCancel}>Cancel</Button>
          <Button
            type="submit"
            form={formId}
            variant="primary"
            disabled={saving || editingId !== undefined}
            aria-busy={saving}
            leftIcon={
              saving ? <FontAwesomeIcon icon={faSpinner} spin /> : undefined
            }
          >
            {event ? "Save Changes" : "Create Event"}
          </Button>
        </div>
      </fieldset>
    </section>
  );
}
