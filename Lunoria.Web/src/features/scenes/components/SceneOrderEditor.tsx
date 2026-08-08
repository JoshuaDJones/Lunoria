import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faGripVertical } from "@fortawesome/free-solid-svg-icons";
import { Button } from "@/components/ui";
import type { Scene } from "@/features/scenes/types";
import { getApiError } from "@/lib/apiClient";

interface SceneOrderEditorProps {
  scenes: Scene[];
  onSave: (scenes: Scene[]) => Promise<void>;
  onCancel: () => void;
}

export function SceneOrderEditor({
  scenes,
  onSave,
  onCancel,
}: SceneOrderEditorProps) {
  const [orderedScenes, setOrderedScenes] = useState(() =>
    [...scenes].sort((a, b) => a.sortOrder - b.sortOrder),
  );
  const [draggedSceneId, setDraggedSceneId] = useState<number>();
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const moveDraggedScene = (targetSceneId: number) => {
    if (draggedSceneId === undefined || draggedSceneId === targetSceneId)
      return;

    setOrderedScenes((current) => {
      const draggedIndex = current.findIndex(
        (scene) => scene.id === draggedSceneId,
      );
      const targetIndex = current.findIndex(
        (scene) => scene.id === targetSceneId,
      );

      if (draggedIndex < 0 || targetIndex < 0) return current;

      const next = [...current];
      const [draggedScene] = next.splice(draggedIndex, 1);
      next.splice(targetIndex, 0, draggedScene);
      return next;
    });
  };

  const save = async () => {
    setIsSaving(true);
    setError("");

    try {
      await onSave(orderedScenes);
    } catch (requestError) {
      setError(getApiError(requestError).message);
      setIsSaving(false);
    }
  };

  return (
    <div className="flex min-h-full flex-col">
      <p className="mb-5 text-sm text-content-secondary">
        Drag scenes into the order they should appear, then save your changes.
      </p>

      {error && (
        <p
          className="mb-4 rounded-lg border border-danger/40 p-3 text-danger"
          role="alert"
        >
          {error}
        </p>
      )}

      <ol className="flex-1 space-y-3">
        {orderedScenes.map((scene, index) => (
          <li
            key={scene.id}
            draggable={!isSaving}
            onDragStart={() => setDraggedSceneId(scene.id)}
            onDragOver={(event) => {
              event.preventDefault();
              moveDraggedScene(scene.id);
            }}
            onDragEnd={() => setDraggedSceneId(undefined)}
            className={`flex cursor-grab items-center gap-4 rounded-xl border bg-surface p-3 transition active:cursor-grabbing ${
              draggedSceneId === scene.id
                ? "border-brand opacity-50"
                : "border-border"
            }`}
          >
            <FontAwesomeIcon
              icon={faGripVertical}
              className="shrink-0 text-content-muted"
            />
            <span className="w-7 shrink-0 text-center text-sm text-content-muted">
              {index + 1}
            </span>
            {scene.photoUrl && (
              <img
                src={scene.photoUrl}
                alt=""
                className="size-12 shrink-0 rounded-lg object-cover"
              />
            )}
            <span className="min-w-0 flex-1 truncate font-semibold text-content">
              {scene.name}
            </span>
          </li>
        ))}
      </ol>

      <div className="mt-6 flex justify-end gap-3 border-t border-border pt-4">
        <Button onClick={onCancel} disabled={isSaving} size="lg">
          Cancel
        </Button>
        <Button
          onClick={() => void save()}
          disabled={isSaving}
          variant="primary"
          size="lg"
        >
          {isSaving ? "Saving..." : "Save Order"}
        </Button>
      </div>
    </div>
  );
}
