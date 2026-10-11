import { useEffect, useRef, useState, type FormEvent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { Button, Input } from "@/components/ui";
import { InfoTooltip } from "@/components/ui/InfoTooltip";
import { listEquipment, type EquippableItem } from "@/features/equipment";
import { listItems, type Item } from "@/features/items";
import { SceneChestLootEntryEditor } from "@/features/scenes/components/SceneChestLootEntryEditor";
import { ChestDieFace } from "@/features/scenes/components/ChestDieFace";
import type {
  SceneChest,
  SceneChestDraftInput,
  SceneChestLootEntryInput,
} from "@/features/scenes/types";
import { getApiError } from "@/lib/apiClient";

interface Props {
  chest: SceneChest | null;
  onSave: (input: SceneChestDraftInput) => Promise<void>;
  onCancel: () => void;
  onBusyChange?: (busy: boolean) => void;
}

export function SceneChestDraftEditor({
  chest,
  onSave,
  onCancel,
  onBusyChange,
}: Props) {
  const [name, setName] = useState(chest?.name ?? "");
  const [entries, setEntries] = useState<SceneChestLootEntryInput[]>(
    () =>
      chest?.lootEntries.map((entry) => ({
        rollMinimum: entry.rollMinimum,
        rollMaximum: entry.rollMaximum,
        quantity: entry.quantity,
        equippableItemId: entry.equippableItem?.id ?? null,
        consumableItemId: entry.consumableItem?.id ?? null,
      })) ?? [
        {
          rollMinimum: 1,
          rollMaximum: 6,
          quantity: 1,
          equippableItemId: null,
          consumableItemId: 0,
        },
      ],
  );
  const [equipment, setEquipment] = useState<EquippableItem[]>([]);
  const [consumables, setConsumables] = useState<Item[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState("");
  const [reload, setReload] = useState(0);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const savingRef = useRef(false);

  useEffect(() => {
    let current = true;
    void Promise.all([listEquipment({ take: 500 }), listItems({ take: 500 })])
      .then(([loadedEquipment, loadedItems]) => {
        if (!current) return;
        setEquipment(loadedEquipment);
        setConsumables(loadedItems);
        setLoadError("");
      })
      .catch((reason: unknown) => {
        if (current) setLoadError(getApiError(reason).message);
      })
      .finally(() => {
        if (current) setLoading(false);
      });
    return () => {
      current = false;
    };
  }, [reload]);

  const itemName = (entry: SceneChestLootEntryInput) =>
    (entry.equippableItemId !== null ? equipment : consumables).find(
      (item) => item.id === (entry.equippableItemId ?? entry.consumableItemId),
    )?.name;
  const outcomes = Array.from({ length: 6 }, (_, index) => {
    const roll = index + 1;
    const matching = entries.filter(
      (entry) => entry.rollMinimum <= roll && entry.rollMaximum >= roll,
    );
    return { roll, matching };
  });
  const missing = outcomes
    .filter(({ matching }) => matching.length === 0)
    .map(({ roll }) => roll);
  const overlapping = outcomes
    .filter(({ matching }) => matching.length > 1)
    .map(({ roll }) => roll);
  const invalidEntries = entries.some(
    (entry) =>
      !Number.isInteger(entry.rollMinimum) ||
      !Number.isInteger(entry.rollMaximum) ||
      entry.rollMinimum < 1 ||
      entry.rollMaximum > 6 ||
      entry.rollMaximum < entry.rollMinimum ||
      !Number.isInteger(entry.quantity) ||
      entry.quantity < 1 ||
      entry.quantity > 2147483647 ||
      !itemName(entry),
  );

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    if (savingRef.current || loading || loadError) return;
    if (
      !name.trim() ||
      entries.length < 1 ||
      entries.length > 6 ||
      invalidEntries ||
      missing.length ||
      overlapping.length
    ) {
      setError(
        "Enter a name and configure exactly one valid loot outcome for each roll from 1 to 6.",
      );
      return;
    }
    savingRef.current = true;
    setSaving(true);
    setError("");
    onBusyChange?.(true);
    try {
      await onSave({ name: name.trim(), lootEntries: entries });
    } catch (reason) {
      setError(getApiError(reason).message);
    } finally {
      savingRef.current = false;
      setSaving(false);
      onBusyChange?.(false);
    }
  };

  return (
    <form
      onSubmit={(event) => void submit(event)}
      className="space-y-5"
      aria-busy={saving}
    >
      <div>
        <div className="flex items-center gap-2">
          <h3 className="text-2xl font-semibold text-content">
            {chest ? "Edit Chest" : "Create Chest"}
          </h3>
          <InfoTooltip
            plain
            label="Chest configuration information"
            text="Uses a six-sided die. Configure loot options below. Assign what die side will award the user with what item."
          />
        </div>
        {chest && chest.dieSides !== 6 && (
          <p className="mt-2 text-sm text-amber-200">
            This chest will use a d6 after saving. Update any ranges outside
            1–6.
          </p>
        )}
      </div>
      <fieldset disabled={saving} className="min-w-0 space-y-5">
        <div>
          <div className="mb-2 flex items-center gap-2">
            <label
              htmlFor="chest-name"
              className="text-sm font-medium text-content-secondary"
            >
              Name
            </label>
            <InfoTooltip
              plain
              label="Chest name information"
              text="Note: This name will be visible to the players when they are opening a chest."
            />
          </div>
          <Input
            id="chest-name"
            required
            maxLength={250}
            value={name}
            onChange={(event) => setName(event.target.value)}
          />
        </div>
        {loading ? (
          <p className="text-sm text-content-secondary" role="status">
            <FontAwesomeIcon icon={faSpinner} spin className="mr-2" />
            Loading items…
          </p>
        ) : loadError ? (
          <div className="space-y-2">
            <p role="alert" className="text-sm text-danger">
              {loadError}
            </p>
            <Button
              onClick={() => {
                setLoading(true);
                setReload((value) => value + 1);
              }}
            >
              Retry loading items
            </Button>
          </div>
        ) : (
          <>
            <section className="space-y-3" aria-label="Die roll outcomes">
              <h4 className="font-semibold text-content">Roll outcomes</h4>
              <div className="grid grid-cols-2 gap-2">
                {outcomes.map(({ roll, matching }) => {
                  const entry = matching[0];
                  const ready = matching.length === 1 && !!itemName(entry);
                  return (
                    <div
                      key={roll}
                      className={`rounded-lg border p-3 ${ready ? "border-border bg-surface" : "border-amber-400/40 bg-amber-400/5"}`}
                    >
                      <ChestDieFace
                        value={roll}
                        className="size-8 text-amber-300"
                      />
                      <p className="mt-1 break-words text-sm text-content-secondary">
                        {matching.length === 0
                          ? "No loot configured"
                          : matching.length > 1
                            ? "Overlapping entries"
                            : `${entry.quantity || "?"} × ${itemName(entry) ?? "Select an item"}`}
                      </p>
                    </div>
                  );
                })}
              </div>
              {(missing.length > 0 || overlapping.length > 0) && (
                <p className="text-sm text-amber-200">
                  {missing.length > 0 &&
                    `Missing rolls: ${missing.join(", ")}. `}
                  {overlapping.length > 0 &&
                    `Overlapping rolls: ${overlapping.join(", ")}. `}
                  Each roll needs exactly one loot entry before saving.
                </p>
              )}
            </section>
            <div className="space-y-3">
              {entries.map((entry, index) => (
                <SceneChestLootEntryEditor
                  key={index}
                  index={index}
                  entry={entry}
                  equipment={equipment}
                  consumables={consumables}
                  onChange={(updated) =>
                    setEntries((current) =>
                      current.map((value, position) =>
                        position === index ? updated : value,
                      ),
                    )
                  }
                  onRemove={() =>
                    setEntries((current) =>
                      current.filter((_, position) => position !== index),
                    )
                  }
                />
              ))}
              <Button
                disabled={entries.length >= 6}
                leftIcon={<FontAwesomeIcon icon={faPlus} />}
                onClick={() => {
                  const roll = missing[0] ?? 1;
                  setEntries((current) => [
                    ...current,
                    {
                      rollMinimum: roll,
                      rollMaximum: roll,
                      quantity: 1,
                      equippableItemId: null,
                      consumableItemId: 0,
                    },
                  ]);
                }}
              >
                Add Loot Entry
              </Button>
            </div>
          </>
        )}
        {error && (
          <p role="alert" className="text-sm text-danger">
            {error}
          </p>
        )}
        <div className="flex flex-wrap justify-end gap-3 border-t border-border pt-4">
          <Button onClick={onCancel}>Cancel</Button>
          <Button
            type="submit"
            variant="primary"
            disabled={saving || loading || !!loadError}
            aria-busy={saving}
            leftIcon={
              saving ? <FontAwesomeIcon icon={faSpinner} spin /> : undefined
            }
          >
            {chest ? "Save Changes" : "Create Chest"}
          </Button>
        </div>
      </fieldset>
    </form>
  );
}
