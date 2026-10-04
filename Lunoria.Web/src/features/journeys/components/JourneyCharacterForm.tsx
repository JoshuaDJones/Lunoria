import { useEffect, useState, type FormEvent } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSpinner } from "@fortawesome/free-solid-svg-icons";
import { Button, FormField, Input, Select } from "@/components/ui";
import type { Character } from "@/features/characters";
import type { UpdateJourneyCharacterRequest } from "@/features/journeys/api/journeysApi";
import { CharacterSyncButton } from "@/features/characterSync/components/CharacterSyncButton";
import type { JourneyCharacter } from "@/features/journeys/types";
import { getApiError } from "@/lib/apiClient";

export function JourneyCharacterForm({
  assignment,
  characters,
  onSave,
  onCancel,
  onSyncUpdated,
  onDirtyChange,
  onBusyChange,
  disabled = false,
}: {
  assignment: JourneyCharacter;
  characters: Character[];
  onSave: (request: UpdateJourneyCharacterRequest) => Promise<void>;
  onCancel: () => void;
  onSyncUpdated: () => Promise<void>;
  onDirtyChange: (dirty: boolean) => void;
  onBusyChange: (busy: boolean) => void;
  disabled?: boolean;
}) {
  const [values, setValues] = useState({
    melee: text(assignment.meleeAttackDamage),
    bow: text(assignment.bowAttackDamage),
    movement: String(assignment.movement),
    consumables: String(assignment.maxConsumableInventory),
    equipment: String(assignment.maxEquippableInventory),
    hp: String(assignment.maxHp),
    mp: String(assignment.maxMp),
    active: assignment.isInitiallyActive,
    alternate: String(assignment.alternateForm?.id ?? ""),
  });
  const [saving, setSaving] = useState(false);
  const [initialValues] = useState(values);
  const hasUnsavedChanges =
    JSON.stringify(values) !== JSON.stringify(initialValues);
  useEffect(() => {
    onDirtyChange(hasUnsavedChanges);
    return () => onDirtyChange(false);
  }, [hasUnsavedChanges, onDirtyChange]);
  const [error, setError] = useState("");
  const set = (key: keyof typeof values, value: string | boolean) =>
    setValues((current) => ({ ...current, [key]: value }));
  const submit = async (event: FormEvent) => {
    event.preventDefault();
    if (saving || disabled) return;
    setSaving(true);
    onBusyChange(true);
    setError("");
    try {
      await onSave({
        meleeAttackDamage: numberOrNull(values.melee),
        bowAttackDamage: numberOrNull(values.bow),
        movement: Number(values.movement),
        maxConsumableInventory: Number(values.consumables),
        maxEquippableInventory: Number(values.equipment),
        maxHp: Number(values.hp),
        maxMp: Number(values.mp),
        isInitiallyActive: values.active,
        alternateFormId: numberOrNull(values.alternate),
      });
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setSaving(false);
      onBusyChange(false);
    }
  };
  const alternates = characters.filter(
    (character) => character.id !== assignment.characterId,
  );
  return (
    <form onSubmit={(event) => void submit(event)}>
      <fieldset disabled={saving || disabled} className="space-y-5">
        <div className="space-y-2">
          <CharacterSyncButton
            kind="journey"
            assignmentId={assignment.id}
            name={assignment.character.name}
            status={assignment.syncStatus}
            disabled={disabled || saving || hasUnsavedChanges}
            onUpdated={onSyncUpdated}
          />
          {hasUnsavedChanges && (
            <p className="text-xs text-content-muted">
              Save your edits or cancel back to the list before comparing with
              the base.
            </p>
          )}
        </div>
        <div className="grid gap-4 sm:grid-cols-2">
          <NumberField
            id="jc-hp"
            label="Max HP"
            value={values.hp}
            change={(v) => set("hp", v)}
            min={1}
          />
          <NumberField
            id="jc-mp"
            label="Max MP"
            value={values.mp}
            change={(v) => set("mp", v)}
          />
          <NumberField
            id="jc-melee"
            label="Melee damage"
            value={values.melee}
            change={(v) => set("melee", v)}
            optional
          />
          <NumberField
            id="jc-bow"
            label="Bow damage"
            value={values.bow}
            change={(v) => set("bow", v)}
            optional
          />
          <NumberField
            id="jc-movement"
            label="Movement"
            value={values.movement}
            change={(v) => set("movement", v)}
          />
          <NumberField
            id="jc-consumables"
            label="Consumable slots"
            value={values.consumables}
            change={(v) => set("consumables", v)}
          />
          <NumberField
            id="jc-equipment"
            label="Equipment slots"
            value={values.equipment}
            change={(v) => set("equipment", v)}
          />
        </div>
        <FormField htmlFor="jc-alternate" label="Alternate form">
          <Select
            id="jc-alternate"
            value={values.alternate}
            onChange={(event) => set("alternate", event.target.value)}
          >
            <option value="">Use character's default alternate form</option>
            {alternates.map((character) => (
              <option key={character.id} value={character.id}>
                {character.name}
              </option>
            ))}
          </Select>
        </FormField>
        <label className="flex items-center gap-3 text-content">
          <input
            type="checkbox"
            checked={values.active}
            onChange={(event) => set("active", event.target.checked)}
            className="size-4"
          />
          Initially active
        </label>
        {error && (
          <p className="text-sm text-danger" role="alert">
            {error}
          </p>
        )}
        <div className="flex justify-end gap-3 border-t border-border pt-4">
          <Button onClick={onCancel} disabled={saving}>
            Cancel
          </Button>
          <Button type="submit" disabled={saving} variant="primary">
            {saving && (
              <FontAwesomeIcon icon={faSpinner} spin aria-hidden="true" />
            )}{" "}
            Save stats
          </Button>
        </div>
      </fieldset>
    </form>
  );
}

function NumberField({
  id,
  label,
  value,
  change,
  optional,
  min = 0,
}: {
  id: string;
  label: string;
  value: string;
  change: (value: string) => void;
  optional?: boolean;
  min?: number;
}) {
  return (
    <FormField htmlFor={id} label={label}>
      <Input
        id={id}
        type="number"
        min={min}
        value={value}
        onChange={(event) => change(event.target.value)}
        required={!optional}
      />
    </FormField>
  );
}
function numberOrNull(value: string): number | null {
  return value === "" ? null : Number(value);
}
function text(value: number | null): string {
  return value === null ? "" : String(value);
}
