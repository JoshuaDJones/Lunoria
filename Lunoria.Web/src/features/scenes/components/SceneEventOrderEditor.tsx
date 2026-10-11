import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBars, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { Button } from "@/components/ui";
import { getApiError } from "@/lib/apiClient";
function sortItems<T extends { sortOrder: number }>(items: T[]): T[] {
  return [...items].sort((a, b) => a.sortOrder - b.sortOrder);
}
export function SceneEventOrderEditor<
  T extends { id: number; name: string; sortOrder: number },
>({
  title,
  items,
  onSave,
  onCancel,
  onBusyChange,
}: {
  onBusyChange?: (busy: boolean) => void;
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
    if (isSaving || draggedId === undefined || draggedId === targetId) return;
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
    if (isSaving) return;
    setIsSaving(true);
    onBusyChange?.(true);
    setError("");
    try {
      await onSave(ordered);
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsSaving(false);
      onBusyChange?.(false);
    }
  };

  return (
    <div>
      <h3 className="text-2xl font-semibold text-content">Sort {title}s</h3>
      <p className="mt-1 text-sm text-content-secondary">
        Drag items into the order they should run.
      </p>
      <ol className="mt-5 space-y-3">
        {ordered.map((item, index) => (
          <li
            key={item.id}
            draggable={!isSaving}
            onDragStart={() => setDraggedId(item.id)}
            onDragOver={(e) => {
              e.preventDefault();
              move(item.id);
            }}
            onDragEnd={() => setDraggedId(undefined)}
            className={`flex cursor-grab items-center gap-4 rounded-xl border bg-surface p-4 ${draggedId === item.id ? "border-brand opacity-50" : "border-border"}`}
          >
            <FontAwesomeIcon icon={faBars} className="text-content-muted" />
            <span className="w-6 text-center text-sm text-content-muted">
              {index + 1}
            </span>
            <span className="font-semibold text-content">{item.name}</span>
          </li>
        ))}
      </ol>
      <FormActions
        isSaving={isSaving}
        error={error}
        onCancel={onCancel}
        onSave={() => void save()}
      />
    </div>
  );
}

function FormActions({
  isSaving,
  error,
  onCancel,
  onSave,
}: {
  isSaving: boolean;
  error: string;
  onCancel: () => void;
  onSave?: () => void;
}) {
  return (
    <div className="border-t border-border pt-4">
      {error && (
        <p className="mb-3 text-sm text-danger" role="alert">
          {error}
        </p>
      )}
      <div className="flex justify-end gap-3">
        <Button onClick={onCancel} disabled={isSaving}>
          Cancel
        </Button>
        <Button
          type={onSave ? "button" : "submit"}
          onClick={onSave}
          disabled={isSaving}
          variant="primary"
        >
          {isSaving && (
            <FontAwesomeIcon icon={faSpinner} spin className="mr-2" />
          )}
          Save Order
        </Button>
      </div>
    </div>
  );
}
