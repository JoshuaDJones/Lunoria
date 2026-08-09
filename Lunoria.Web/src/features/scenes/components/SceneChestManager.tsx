import { useEffect, useState, type FormEvent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faBoxOpen,
  faChevronRight,
  faPen,
  faPlus,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import { useConfirmDialog, useToast } from "@/app/providers";
import { Button, FormField, Input, Select } from "@/components/ui";
import { listEquipment, type EquippableItem } from "@/features/equipment";
import { listItems, type Item } from "@/features/items";
import {
  createSceneChest,
  createSceneChestLootEntry,
  deleteSceneChest,
  deleteSceneChestLootEntry,
  listSceneChests,
  listSceneChestLootEntries,
  updateSceneChest,
  updateSceneChestLootEntry,
} from "@/features/scenes/api/scenesApi";
import type {
  Scene,
  SceneChest,
  SceneChestInput,
  SceneChestLootEntry,
  SceneChestLootEntryInput,
} from "@/features/scenes/types";
import { getApiError } from "@/lib/apiClient";

interface SceneChestManagerProps {
  scene: Scene;
}

type View = "chests" | "chest-form" | "loot" | "loot-form";

export function SceneChestManager({ scene }: SceneChestManagerProps) {
  const { confirm } = useConfirmDialog();
  const toast = useToast();
  const [chests, setChests] = useState<SceneChest[]>([]);
  const [selectedChestId, setSelectedChestId] = useState<number>();
  const [editingChest, setEditingChest] = useState<SceneChest | null>();
  const [editingEntry, setEditingEntry] = useState<SceneChestLootEntry | null>();
  const [equipment, setEquipment] = useState<EquippableItem[]>([]);
  const [items, setItems] = useState<Item[]>([]);
  const [view, setView] = useState<View>("chests");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const selectedChest = chests.find((chest) => chest.id === selectedChestId);

  const load = async () => {
    setIsLoading(true);
    setError("");
    try {
      const loadedChests = await listSceneChests(scene.id);
      setChests(loadedChests);
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    let isCurrent = true;

    void listSceneChests(scene.id)
      .then((loadedChests) => {
        if (isCurrent) {
          setChests(loadedChests);
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

  const openLoot = async (chest: SceneChest) => {
    setSelectedChestId(chest.id);
    setView("loot");
    try {
      const entries = await listSceneChestLootEntries(scene.id, chest.id);
      updateChest(chest.id, (current) => ({ ...current, lootEntries: entries }));
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to load chest loot");
    }
  };

  const openLootForm = async (entry: SceneChestLootEntry | null) => {
    try {
      if (equipment.length === 0 && items.length === 0) {
        const [loadedEquipment, loadedItems] = await Promise.all([
          listEquipment({ take: 500 }),
          listItems({ take: 500 }),
        ]);
        setEquipment(loadedEquipment);
        setItems(loadedItems);
      }
      setEditingEntry(entry);
      setView("loot-form");
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to load items");
    }
  };

  const updateChest = (chestId: number, update: (chest: SceneChest) => SceneChest) => {
    setChests((current) =>
      current.map((chest) => (chest.id === chestId ? update(chest) : chest)),
    );
  };

  const removeChest = async (chest: SceneChest) => {
    const confirmed = await confirm({
      title: `Delete chest "${chest.name}"?`,
      message: "Its loot entries will also be deleted. This cannot be undone.",
      confirmLabel: "Delete",
      variant: "danger",
    });
    if (!confirmed) return;

    try {
      await deleteSceneChest(scene.id, chest.id);
      setChests((current) => current.filter((item) => item.id !== chest.id));
      toast.success(`Chest "${chest.name}" was deleted.`);
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to delete chest");
    }
  };

  const removeEntry = async (entry: SceneChestLootEntry) => {
    if (!selectedChest) return;
    const confirmed = await confirm({
      title: `Delete ${lootName(entry)}?`,
      message: "This loot entry cannot be recovered.",
      confirmLabel: "Delete",
      variant: "danger",
    });
    if (!confirmed) return;

    try {
      await deleteSceneChestLootEntry(scene.id, selectedChest.id, entry.id);
      updateChest(selectedChest.id, (chest) => ({
        ...chest,
        lootEntries: chest.lootEntries.filter((item) => item.id !== entry.id),
      }));
      toast.success("Loot entry was deleted.");
    } catch (requestError) {
      toast.error(getApiError(requestError).message, "Unable to delete loot entry");
    }
  };

  if (isLoading) return <p className="text-content-secondary">Loading chests...</p>;

  if (error) {
    return (
      <div className="space-y-4">
        <p className="rounded-lg border border-danger/40 p-3 text-danger" role="alert">{error}</p>
        <Button onClick={() => void load()} variant="primary">Try again</Button>
      </div>
    );
  }

  if (view === "chest-form") {
    return (
      <ChestForm
        chest={editingChest}
        onCancel={() => setView("chests")}
        onSave={async (input) => {
          const saved = editingChest
            ? await updateSceneChest(scene.id, editingChest.id, input)
            : await createSceneChest(scene.id, input);
          setChests((current) =>
            editingChest
              ? current.map((chest) => (chest.id === saved.id ? saved : chest))
              : [...current, saved],
          );
          setView("chests");
          toast.success(`Chest "${saved.name}" was ${editingChest ? "updated" : "created"}.`);
        }}
      />
    );
  }

  if (view === "loot-form" && selectedChest) {
    return (
      <LootEntryForm
        chest={selectedChest}
        entry={editingEntry}
        equipment={equipment}
        items={items}
        onCancel={() => setView("loot")}
        onSave={async (input) => {
          const saved = editingEntry
            ? await updateSceneChestLootEntry(scene.id, selectedChest.id, editingEntry.id, input)
            : await createSceneChestLootEntry(scene.id, selectedChest.id, input);
          updateChest(selectedChest.id, (chest) => ({
            ...chest,
            lootEntries: editingEntry
              ? chest.lootEntries.map((entry) => (entry.id === saved.id ? saved : entry))
              : [...chest.lootEntries, saved],
          }));
          setView("loot");
          toast.success(`Loot entry was ${editingEntry ? "updated" : "created"}.`);
        }}
      />
    );
  }

  if (view === "loot" && selectedChest) {
    return (
      <div>
        <Button
          onClick={() => {
            setSelectedChestId(undefined);
            setView("chests");
          }}
          leftIcon={<FontAwesomeIcon icon={faArrowLeft} />}
          size="sm"
        >
          All chests
        </Button>
        <div className="mt-5 flex items-start justify-between gap-4">
          <div>
            <h3 className="text-2xl font-semibold text-content">{selectedChest.name}</h3>
            <p className="mt-1 text-sm text-content-secondary">Roll a d{selectedChest.dieSides} to select loot.</p>
          </div>
          <Button onClick={() => void openLootForm(null)} variant="add" leftIcon={<FontAwesomeIcon icon={faPlus} />} size="sm">
            Add loot
          </Button>
        </div>
        {selectedChest.lootEntries.length === 0 ? (
          <EmptyState title="No loot yet" message="Add the first loot entry for this chest." />
        ) : (
          <div className="mt-5 space-y-3">
            {[...selectedChest.lootEntries]
              .sort((a, b) => a.rollMinimum - b.rollMinimum)
              .map((entry) => (
                <article key={entry.id} className="rounded-xl border border-border bg-surface p-4">
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <h4 className="font-semibold text-content">{lootName(entry)}</h4>
                      <p className="mt-1 text-sm text-content-secondary">
                        Roll {entry.rollMinimum}{entry.rollMaximum !== entry.rollMinimum ? `-${entry.rollMaximum}` : ""} · Quantity {entry.quantity}
                      </p>
                    </div>
                    <div className="flex gap-2">
                      <Button onClick={() => void removeEntry(entry)} variant="danger" size="sm" leftIcon={<FontAwesomeIcon icon={faTrash} />}>Delete</Button>
                      <Button onClick={() => void openLootForm(entry)} variant="primary" size="sm" leftIcon={<FontAwesomeIcon icon={faPen} />}>Edit</Button>
                    </div>
                  </div>
                </article>
              ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <div>
      <div className="flex items-center justify-between gap-3">
        <p className="text-sm text-content-secondary">Chests available in {scene.name}.</p>
        <Button
          onClick={() => {
            setEditingChest(null);
            setView("chest-form");
          }}
          variant="add"
          leftIcon={<FontAwesomeIcon icon={faPlus} />}
          size="sm"
        >
          Add chest
        </Button>
      </div>
      {chests.length === 0 ? (
        <EmptyState title="No chests yet" message="Add the first chest for this scene." />
      ) : (
        <div className="mt-5 space-y-3">
          {chests.map((chest) => (
            <article key={chest.id} className="rounded-xl border border-border bg-surface p-4">
              <button type="button" onClick={() => void openLoot(chest)} className="flex w-full cursor-pointer items-center gap-3 text-left">
                <span className="flex size-10 shrink-0 items-center justify-center rounded-lg bg-brand/15 text-brand-hover">
                  <FontAwesomeIcon icon={faBoxOpen} />
                </span>
                <span className="min-w-0 flex-1">
                  <span className="block font-semibold text-content">{chest.name}</span>
                  <span className="mt-0.5 block text-sm text-content-secondary">
                    d{chest.dieSides} · {chest.lootEntries.length} {chest.lootEntries.length === 1 ? "loot entry" : "loot entries"}
                  </span>
                </span>
                <FontAwesomeIcon icon={faChevronRight} className="text-content-muted" />
              </button>
              <div className="mt-3 flex justify-end gap-2 border-t border-border pt-3">
                <Button onClick={() => void removeChest(chest)} variant="danger" size="sm" leftIcon={<FontAwesomeIcon icon={faTrash} />}>Delete</Button>
                <Button onClick={() => { setEditingChest(chest); setView("chest-form"); }} variant="primary" size="sm" leftIcon={<FontAwesomeIcon icon={faPen} />}>Edit</Button>
              </div>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

function ChestForm({ chest, onSave, onCancel }: {
  chest: SceneChest | null | undefined;
  onSave: (input: SceneChestInput) => Promise<void>;
  onCancel: () => void;
}) {
  const [name, setName] = useState(chest?.name ?? "");
  const [dieSides, setDieSides] = useState(String(chest?.dieSides ?? 6));
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");
  const submit = async (event: FormEvent) => {
    event.preventDefault(); setIsSaving(true); setError("");
    try { await onSave({ name, dieSides: Number(dieSides) }); }
    catch (requestError) { setError(getApiError(requestError).message); setIsSaving(false); }
  };
  return (
    <form onSubmit={(event) => void submit(event)} className="space-y-5">
      <h3 className="text-2xl font-semibold text-content">{chest ? "Edit chest" : "Add chest"}</h3>
      <FormField htmlFor="chest-name" label="Name"><Input id="chest-name" value={name} onChange={(e) => setName(e.target.value)} maxLength={250} required /></FormField>
      <FormField htmlFor="chest-die-sides" label="Die sides"><Input id="chest-die-sides" type="number" min={1} value={dieSides} onChange={(e) => setDieSides(e.target.value)} required /></FormField>
      <FormActions isSaving={isSaving} error={error} onCancel={onCancel} />
    </form>
  );
}

function LootEntryForm({ chest, entry, equipment, items, onSave, onCancel }: {
  chest: SceneChest;
  entry: SceneChestLootEntry | null | undefined;
  equipment: EquippableItem[];
  items: Item[];
  onSave: (input: SceneChestLootEntryInput) => Promise<void>;
  onCancel: () => void;
}) {
  const initialType = entry?.equippableItem ? "equipment" : "consumable";
  const [itemType, setItemType] = useState<"equipment" | "consumable">(initialType);
  const [itemId, setItemId] = useState(String(entry?.equippableItem?.id ?? entry?.consumableItem?.id ?? ""));
  const [rollMinimum, setRollMinimum] = useState(String(entry?.rollMinimum ?? 1));
  const [rollMaximum, setRollMaximum] = useState(String(entry?.rollMaximum ?? 1));
  const [quantity, setQuantity] = useState(String(entry?.quantity ?? 1));
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");
  const choices = itemType === "equipment" ? equipment : items;
  const submit = async (event: FormEvent) => {
    event.preventDefault(); setIsSaving(true); setError("");
    try {
      await onSave({
        rollMinimum: Number(rollMinimum), rollMaximum: Number(rollMaximum), quantity: Number(quantity),
        equippableItemId: itemType === "equipment" ? Number(itemId) : null,
        consumableItemId: itemType === "consumable" ? Number(itemId) : null,
      });
    } catch (requestError) { setError(getApiError(requestError).message); setIsSaving(false); }
  };
  return (
    <form onSubmit={(event) => void submit(event)} className="space-y-5">
      <h3 className="text-2xl font-semibold text-content">{entry ? "Edit loot entry" : "Add loot entry"}</h3>
      <FormField htmlFor="loot-type" label="Item type">
        <Select id="loot-type" value={itemType} onChange={(e) => { setItemType(e.target.value as "equipment" | "consumable"); setItemId(""); }}>
          <option value="consumable">Consumable</option><option value="equipment">Equipment</option>
        </Select>
      </FormField>
      <FormField htmlFor="loot-item" label="Item">
        <Select id="loot-item" value={itemId} onChange={(e) => setItemId(e.target.value)} required>
          <option value="" disabled>Select an item</option>
          {choices.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
        </Select>
      </FormField>
      <div className="grid gap-4 sm:grid-cols-2">
        <FormField htmlFor="loot-roll-min" label="Minimum roll"><Input id="loot-roll-min" type="number" min={1} max={chest.dieSides} value={rollMinimum} onChange={(e) => setRollMinimum(e.target.value)} required /></FormField>
        <FormField htmlFor="loot-roll-max" label="Maximum roll"><Input id="loot-roll-max" type="number" min={1} max={chest.dieSides} value={rollMaximum} onChange={(e) => setRollMaximum(e.target.value)} required /></FormField>
      </div>
      <FormField htmlFor="loot-quantity" label="Quantity"><Input id="loot-quantity" type="number" min={1} value={quantity} onChange={(e) => setQuantity(e.target.value)} required /></FormField>
      <FormActions isSaving={isSaving} error={error} onCancel={onCancel} />
    </form>
  );
}

function FormActions({ isSaving, error, onCancel }: { isSaving: boolean; error: string; onCancel: () => void }) {
  return <div className="border-t border-border pt-4">{error && <p className="mb-3 text-sm text-danger" role="alert">{error}</p>}<div className="flex justify-end gap-3"><Button onClick={onCancel} disabled={isSaving}>Cancel</Button><Button type="submit" disabled={isSaving} variant="primary">{isSaving ? "Saving..." : "Save"}</Button></div></div>;
}

function EmptyState({ title, message }: { title: string; message: string }) {
  return <div className="mt-5 rounded-xl border border-border bg-surface/60 p-8 text-center"><h3 className="text-xl font-semibold text-content">{title}</h3><p className="mt-2 text-sm text-content-secondary">{message}</p></div>;
}

function lootName(entry: SceneChestLootEntry): string {
  return entry.equippableItem?.name ?? entry.consumableItem?.name ?? "Unknown item";
}
