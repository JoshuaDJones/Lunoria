import { useEffect, useRef, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faBolt,
  faBars,
  faPlus,
  faSpinner,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import { useConfirmDialog, useToast } from "@/app/providers";
import { Button } from "@/components/ui";
import type { JourneyCharacter } from "@/features/journeys";
import {
  deleteSceneEvent,
  listSceneEvents,
  reorderSceneEvents,
  saveSceneEventDraft,
} from "@/features/scenes/api/scenesApi";
import { SceneEventDraftEditor } from "@/features/scenes/components/SceneEventDraftEditor";
import { SceneEventOrderEditor } from "@/features/scenes/components/SceneEventOrderEditor";
import { actionSummary } from "@/features/scenes/utils/sceneEventDraft";
import type { Scene, SceneEvent } from "@/features/scenes/types";
import { getApiError } from "@/lib/apiClient";

interface SceneEventManagerProps {
  scene: Scene;
  journeyCharacters: JourneyCharacter[];
  onBusyChange?: (busy: boolean) => void;
}
export function SceneEventManager({
  scene,
  journeyCharacters,
  onBusyChange,
}: SceneEventManagerProps) {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [events, setEvents] = useState<SceneEvent[]>([]);
  const [editing, setEditing] = useState<SceneEvent | null>();
  const [ordering, setOrdering] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [reload, setReload] = useState(0);
  const [deletingId, setDeletingId] = useState<number>();
  const deletingRef = useRef(false);
  useEffect(() => {
    let current = true;
    void listSceneEvents(scene.id)
      .then((result) => {
        if (current) {
          setEvents([...result].sort((a, b) => a.sortOrder - b.sortOrder));
          setError("");
        }
      })
      .catch((reason: unknown) => {
        if (current) setError(getApiError(reason).message);
      })
      .finally(() => {
        if (current) setLoading(false);
      });
    return () => {
      current = false;
    };
  }, [scene.id, reload]);
  const remove = async (event: SceneEvent) => {
    if (deletingRef.current) return;
    deletingRef.current = true;
    try {
      if (
        !(await confirm({
          title: `Delete event "${event.name}"?`,
          message: "Its actions will also be deleted. This cannot be undone.",
          confirmLabel: "Delete",
          variant: "danger",
        }))
      )
        return;
      setDeletingId(event.id);
      onBusyChange?.(true);
      await deleteSceneEvent(scene.id, event.id);
      setEvents((current) => current.filter((item) => item.id !== event.id));
      toast.success(`Event "${event.name}" was deleted.`);
    } catch (reason) {
      toast.error(getApiError(reason).message, "Unable to delete event");
    } finally {
      deletingRef.current = false;
      setDeletingId(undefined);
      onBusyChange?.(false);
    }
  };
  if (editing !== undefined)
    return (
      <SceneEventDraftEditor
        key={editing?.id ?? "new"}
        event={editing}
        journeyCharacters={journeyCharacters}
        onBusyChange={onBusyChange}
        onCancel={() => setEditing(undefined)}
        onSave={async (input) => {
          const saved = await saveSceneEventDraft(scene.id, editing?.id, input);
          setEvents((current) =>
            (editing
              ? current.map((item) => (item.id === saved.id ? saved : item))
              : [...current, saved]
            ).sort((a, b) => a.sortOrder - b.sortOrder),
          );
          setEditing(undefined);
          toast.success(
            `Event "${saved.name}" was ${editing ? "updated" : "created"}.`,
          );
        }}
      />
    );
  if (ordering)
    return (
      <SceneEventOrderEditor
        title="event"
        items={events}
        onBusyChange={onBusyChange}
        onCancel={() => setOrdering(false)}
        onSave={async (ordered) => {
          const updated = ordered.map((item, sortOrder) => ({
            ...item,
            sortOrder,
          }));
          await reorderSceneEvents(
            scene.id,
            updated.map(({ id, sortOrder }) => ({ id, sortOrder })),
          );
          setEvents(updated);
          setOrdering(false);
          toast.success("Event order updated.");
        }}
      />
    );
  if (loading)
    return (
      <p role="status" className="text-content-secondary">
        <FontAwesomeIcon icon={faSpinner} spin className="mr-2" />
        Loading events…
      </p>
    );
  if (error)
    return (
      <div className="space-y-3">
        <p role="alert" className="text-danger">
          {error}
        </p>
        <Button
          onClick={() => {
            setLoading(true);
            setReload((value) => value + 1);
          }}
        >
          Try again
        </Button>
      </div>
    );
  return (
    <div className="space-y-5">
      <div className="flex flex-wrap justify-end gap-3">
        {events.length > 1 && (
          <Button
            disabled={deletingId !== undefined}
            leftIcon={<FontAwesomeIcon icon={faBars} />}
            onClick={() => setOrdering(true)}
          >
            Event Order
          </Button>
        )}
        <Button
          variant="primary"
          disabled={deletingId !== undefined}
          leftIcon={<FontAwesomeIcon icon={faPlus} />}
          onClick={() => setEditing(null)}
        >
          Add Event
        </Button>
      </div>
      {events.length === 0 ? (
        <p className="rounded-xl border border-dashed border-border p-6 text-center text-content-secondary">
          No events yet. Create an event and configure its actions before
          saving.
        </p>
      ) : (
        events.map((event) => (
          <article
            key={event.id}
            className="rounded-xl border border-border bg-surface p-4"
          >
            <div className="flex items-center gap-3">
              <FontAwesomeIcon icon={faBolt} className="text-amber-300" />
              <h3 className="min-w-0 break-words font-semibold text-content">
                {event.name}
              </h3>
            </div>
            {event.description && (
              <p className="mt-2 whitespace-pre-wrap break-words text-sm text-content-secondary">
                {event.description}
              </p>
            )}
            {event.sceneEventActions.length ? (
              <ol className="mt-3 space-y-2">
                {[...event.sceneEventActions]
                  .sort((a, b) => a.sortOrder - b.sortOrder)
                  .map((action, i) => (
                    <li
                      key={action.id}
                      className="flex gap-2 text-sm text-content-secondary"
                    >
                      <span className="text-content-muted">{i + 1}.</span>
                      <span className="min-w-0 break-words">
                        {actionSummary(action, journeyCharacters)}
                      </span>
                    </li>
                  ))}
              </ol>
            ) : (
              <p className="mt-3 text-sm text-content-muted">
                No actions configured.
              </p>
            )}
            <div className="mt-4 flex flex-wrap justify-end gap-2 border-t border-border pt-3">
              <Button
                size="sm"
                variant="danger"
                disabled={deletingId !== undefined}
                leftIcon={
                  <FontAwesomeIcon
                    icon={deletingId === event.id ? faSpinner : faTrash}
                    spin={deletingId === event.id}
                  />
                }
                onClick={() => void remove(event)}
              >
                Delete
              </Button>
              <Button
                size="sm"
                variant="primary"
                disabled={deletingId !== undefined}
                onClick={() => setEditing(event)}
              >
                Edit Event
              </Button>
            </div>
          </article>
        ))
      )}
    </div>
  );
}
