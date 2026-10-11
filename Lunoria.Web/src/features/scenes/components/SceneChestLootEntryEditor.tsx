import { Button, FormField, Input, Select } from "@/components/ui";
import { ChestDieFace } from "@/features/scenes/components/ChestDieFace";
import type { SceneChestLootEntryInput } from "@/features/scenes/types";

interface Props {
  index: number;
  entry: SceneChestLootEntryInput;
  equipment: { id: number; name: string }[];
  consumables: { id: number; name: string }[];
  onChange: (entry: SceneChestLootEntryInput) => void;
  onRemove: () => void;
}

export function SceneChestLootEntryEditor({
  index,
  entry,
  equipment,
  consumables,
  onChange,
  onRemove,
}: Props) {
  const isEquipment = entry.equippableItemId !== null;
  const prefix = `chest-loot-${index}`;
  const choices = isEquipment ? equipment : consumables;
  const selectedId = entry.equippableItemId ?? entry.consumableItemId;
  return (
    <section className="space-y-3 rounded-xl border border-border bg-surface p-4">
      <div className="flex items-center justify-between gap-3">
        <h4 className="font-semibold text-content">Loot entry {index + 1}</h4>
        <Button size="sm" variant="danger" onClick={onRemove}>
          Remove
        </Button>
      </div>
      <FormField htmlFor={`${prefix}-type`} label="Item type">
        <Select
          id={`${prefix}-type`}
          value={isEquipment ? "equipment" : "consumable"}
          onChange={(event) =>
            onChange({
              ...entry,
              equippableItemId: event.target.value === "equipment" ? 0 : null,
              consumableItemId: event.target.value === "consumable" ? 0 : null,
            })
          }
        >
          <option value="consumable">Consumable</option>
          <option value="equipment">Equipment</option>
        </Select>
      </FormField>
      <FormField htmlFor={`${prefix}-item`} label="Item">
        <Select
          id={`${prefix}-item`}
          required
          value={selectedId || ""}
          onChange={(event) =>
            onChange({
              ...entry,
              equippableItemId: isEquipment ? Number(event.target.value) : null,
              consumableItemId: isEquipment ? null : Number(event.target.value),
            })
          }
        >
          <option value="" disabled>
            Select an item
          </option>
          {!!selectedId && !choices.some((item) => item.id === selectedId) && (
            <option value={selectedId} disabled>
              Unavailable item — select a replacement
            </option>
          )}
          {choices.map((item) => (
            <option key={item.id} value={item.id}>
              {item.name}
            </option>
          ))}
        </Select>
      </FormField>
      {(["Minimum roll", "Maximum roll"] as const).map((label, bound) => (
        <fieldset key={label} className="min-w-0">
          <legend className="mb-2 text-sm font-medium text-content-secondary">
            {label}
          </legend>
          <div className="flex flex-wrap gap-2">
            {[1, 2, 3, 4, 5, 6].map((value) => {
              const selected =
                value === (bound === 0 ? entry.rollMinimum : entry.rollMaximum);
              return (
                <button
                  key={value}
                  type="button"
                  aria-label={`${label}: ${value}`}
                  aria-pressed={selected}
                  className={`rounded-xl border-2 p-1.5 transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-amber-300 disabled:cursor-not-allowed disabled:opacity-40 ${selected ? "border-amber-300 bg-amber-300/10 text-amber-300" : "border-border text-content-secondary hover:border-amber-300/60"}`}
                  onClick={() =>
                    onChange(
                      bound === 0
                        ? {
                            ...entry,
                            rollMinimum: value,
                            rollMaximum: Math.min(
                              6,
                              Math.max(value, entry.rollMaximum),
                            ),
                          }
                        : {
                            ...entry,
                            rollMinimum: Math.max(
                              1,
                              Math.min(value, entry.rollMinimum),
                            ),
                            rollMaximum: value,
                          },
                    )
                  }
                >
                  <ChestDieFace value={value} />
                </button>
              );
            })}
          </div>
        </fieldset>
      ))}
      <FormField htmlFor={`${prefix}-quantity`} label="Quantity">
        <Input
          id={`${prefix}-quantity`}
          type="number"
          required
          min={1}
          max={2147483647}
          step={1}
          value={entry.quantity || ""}
          onChange={(event) =>
            onChange({ ...entry, quantity: Number(event.target.value) })
          }
        />
      </FormField>
    </section>
  );
}
