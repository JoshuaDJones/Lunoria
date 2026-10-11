import { useEffect, useRef, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faPen,
  faPlus,
  faSpinner,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import { useConfirmDialog, useToast } from "@/app/providers";
import { Button } from "@/components/ui";
import { TreasureChestIcon } from "@/components/ui/TreasureChestIcon";
import {
  deleteSceneChest,
  listSceneChests,
  saveSceneChestDraft,
} from "@/features/scenes/api/scenesApi";
import { SceneChestDraftEditor } from "@/features/scenes/components/SceneChestDraftEditor";
import { ChestDieFace } from "@/features/scenes/components/ChestDieFace";
import type { Scene, SceneChest } from "@/features/scenes/types";
import { getApiError } from "@/lib/apiClient";

interface SceneChestManagerProps {
  scene: Scene;
  onBusyChange?: (busy: boolean) => void;
}

export function SceneChestManager({
  scene,
  onBusyChange,
}: SceneChestManagerProps) {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [chests, setChests] = useState<SceneChest[]>([]);
  const [editing, setEditing] = useState<SceneChest | null>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [reload, setReload] = useState(0);
  const [deletingId, setDeletingId] = useState<number>();
  const deletingRef = useRef(false);

  useEffect(() => {
    let current = true;
    void listSceneChests(scene.id)
      .then((result) => {
        if (current) {
          setChests(result);
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

  const removeChest = async (chest: SceneChest) => {
    if (deletingRef.current) return;
    deletingRef.current = true;
    try {
      const confirmed = await confirm({
        title: `Delete chest "${chest.name}"?`,
        message:
          "Its loot entries will also be deleted. This cannot be undone.",
        confirmLabel: "Delete",
        variant: "danger",
      });
      if (!confirmed) return;
      setDeletingId(chest.id);
      onBusyChange?.(true);
      await deleteSceneChest(scene.id, chest.id);
      setChests((current) => current.filter((item) => item.id !== chest.id));
      toast.success(`Chest "${chest.name}" was deleted.`);
    } catch (reason) {
      toast.error(getApiError(reason).message, "Unable to delete chest");
    } finally {
      deletingRef.current = false;
      setDeletingId(undefined);
      onBusyChange?.(false);
    }
  };

  if (editing !== undefined)
    return (
      <SceneChestDraftEditor
        key={editing?.id ?? "new"}
        chest={editing}
        onBusyChange={onBusyChange}
        onCancel={() => setEditing(undefined)}
        onSave={async (input) => {
          const saved = await saveSceneChestDraft(scene.id, editing?.id, input);
          setChests((current) =>
            editing
              ? current.map((chest) => (chest.id === saved.id ? saved : chest))
              : [...current, saved],
          );
          setEditing(undefined);
          toast.success(
            `Chest "${saved.name}" was ${editing ? "updated" : "created"}.`,
          );
        }}
      />
    );
  if (loading)
    return (
      <p role="status" className="text-content-secondary">
        <FontAwesomeIcon icon={faSpinner} spin className="mr-2" />
        Loading chests…
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
      <div className="flex justify-end">
        <Button
          variant="primary"
          disabled={deletingId !== undefined}
          leftIcon={<FontAwesomeIcon icon={faPlus} />}
          onClick={() => setEditing(null)}
        >
          Add Chest
        </Button>
      </div>
      {chests.length === 0 ? (
        <div className="rounded-xl border border-border bg-surface/60 p-8 text-center">
          <h3 className="text-xl font-semibold text-content">No chests yet</h3>
          <p className="mt-2 text-sm text-content-secondary">
            Create a chest and configure its loot before saving.
          </p>
        </div>
      ) : (
        chests.map((chest) => (
          <article
            key={chest.id}
            className="rounded-xl border border-border bg-surface p-4"
          >
            <div className="flex items-center gap-3">
              <TreasureChestIcon className="size-7 shrink-0 text-amber-300" />
              <div className="min-w-0 flex-1">
                <h3 className="break-words font-semibold text-content">
                  {chest.name}
                </h3>
              </div>
            </div>
            {chest.lootEntries.length === 0 ? (
              <p className="mt-3 text-sm text-content-muted">
                No loot configured.
              </p>
            ) : (
              <ul className="mt-4 space-y-3" aria-label="Chest loot outcomes">
                {[...chest.lootEntries]
                  .sort((a, b) => a.rollMinimum - b.rollMinimum)
                  .map((entry) => (
                    <li
                      key={entry.id}
                      className="flex flex-wrap items-center gap-3"
                    >
                      <span className="inline-flex items-center gap-1.5 text-amber-300">
                        <ChestDieFace value={entry.rollMinimum} />
                        {entry.rollMaximum !== entry.rollMinimum && (
                          <>
                            <span className="text-content-muted">–</span>
                            <ChestDieFace value={entry.rollMaximum} />
                          </>
                        )}
                      </span>
                      <span className="min-w-0 break-words text-sm text-content-secondary">
                        {entry.equippableItem?.name ??
                          entry.consumableItem?.name ??
                          "Unavailable item"}{" "}
                        ×{entry.quantity}
                      </span>
                    </li>
                  ))}
              </ul>
            )}
            <div className="mt-3 flex flex-wrap justify-end gap-2 border-t border-border pt-3">
              <Button
                size="sm"
                variant="danger"
                disabled={deletingId !== undefined}
                leftIcon={
                  <FontAwesomeIcon
                    icon={deletingId === chest.id ? faSpinner : faTrash}
                    spin={deletingId === chest.id}
                  />
                }
                onClick={() => void removeChest(chest)}
              >
                Delete
              </Button>
              <Button
                size="sm"
                variant="primary"
                disabled={deletingId !== undefined}
                leftIcon={<FontAwesomeIcon icon={faPen} />}
                onClick={() => setEditing(chest)}
              >
                Edit Chest
              </Button>
            </div>
          </article>
        ))
      )}
    </div>
  );
}
