import { useEffect, useState, type FormEvent } from "react";
import { Button, FormField, Input, Select } from "@/components/ui";
import type { JourneyCharacter } from "@/features/journeys";
import { listSpells } from "@/features/spells/api/spellsApi";
import { listItems } from "@/features/items/api/itemsApi";
import { listEquipment } from "@/features/equipment/api/equipmentApi";
import { listCharacters } from "@/features/characters/api/charactersApi";
import { CharacterType, type Character } from "@/features/characters/types";
import {
  ActionTargetType,
  AdjustmentOperation,
  CharacterStatType,
  EventActionType,
  type SceneEventAction,
  type SceneEventActionInput,
} from "@/features/scenes/types";
import {
  statLabels,
  operationLabels,
  enumEntries,
  type SceneEventActionLabels,
} from "@/features/scenes/utils/sceneEventDraft";
import { getApiError } from "@/lib/apiClient";
export function SceneEventActionEditor({
  action,
  journeyCharacters,
  onSave,
  onCancel,
}: {
  action: SceneEventAction | null | undefined;
  journeyCharacters: JourneyCharacter[];
  onSave: (
    input: SceneEventActionInput,
    labels: SceneEventActionLabels,
  ) => Promise<void>;
  onCancel: () => void;
}) {
  const adjustment = action?.characterStatAdjustmentAction;
  const change = action?.characterChangeAlternateFormAction;
  const inForm = action?.characterInAlternateFormAction;
  const spellGrant = action?.characterAddSpellAction;
  const itemGrant = action?.characterGiveItemAction;
  const [spellId, setSpellId] = useState(String(spellGrant?.spellId ?? ""));
  const [itemType, setItemType] = useState(
    itemGrant?.equippableItemId ? "equipment" : "consumable",
  );
  const [itemId, setItemId] = useState(
    String(itemGrant?.equippableItemId ?? itemGrant?.consumableItemId ?? ""),
  );
  const [quantity, setQuantity] = useState(String(itemGrant?.quantity ?? 1));
  const [grantOptions, setGrantOptions] = useState<
    { id: number; name: string }[]
  >([]);
  const [grantLoadError, setGrantLoadError] = useState("");
  const [grantsLoading, setGrantsLoading] = useState(
    action?.eventActionType === EventActionType.CharacterAddSpell ||
      action?.eventActionType === EventActionType.CharacterGiveItem,
  );
  const [grantReload, setGrantReload] = useState(0);
  const [actionType, setActionType] = useState(
    action?.eventActionType ?? EventActionType.CharacterStatAdjustment,
  );
  const [alternateFormId, setAlternateFormId] = useState(
    String(change?.alternateFormId ?? ""),
  );
  const [alternateCharacters, setAlternateCharacters] = useState<Character[]>(
    [],
  );
  const [alternateError, setAlternateError] = useState("");
  const changesAlternateForm =
    actionType === EventActionType.CharacterChangeAlternateForm;
  const clearsAlternateForm =
    actionType === EventActionType.CharacterClearAlternateForm;
  const entersAlternateForm =
    actionType === EventActionType.CharacterInAlternateForm;
  const givesSpell = actionType === EventActionType.CharacterAddSpell;
  const givesItem = actionType === EventActionType.CharacterGiveItem;
  const loadGrantOptions = () => {
    setGrantsLoading(true);
    setGrantLoadError("");
    setGrantReload((value) => value + 1);
  };
  useEffect(() => {
    if (!givesSpell && !givesItem) return;
    let current = true;
    const request = givesSpell
      ? listSpells()
      : itemType === "equipment"
        ? listEquipment()
        : listItems();
    void request
      .then((options) => {
        if (current) {
          setGrantOptions(options);
          setGrantLoadError("");
        }
      })
      .catch((requestError: unknown) => {
        if (current) setGrantLoadError(getApiError(requestError).message);
      })
      .finally(() => {
        if (current) setGrantsLoading(false);
      });
    return () => {
      current = false;
    };
  }, [givesSpell, givesItem, itemType, grantReload]);
  useEffect(() => {
    if (!changesAlternateForm) return;
    let isCurrent = true;
    void listCharacters({ typeFilter: CharacterType.Any })
      .then((characters) => {
        if (isCurrent) {
          setAlternateCharacters(characters);
          setAlternateError("");
        }
      })
      .catch((requestError: unknown) => {
        if (isCurrent) setAlternateError(getApiError(requestError).message);
      });
    return () => {
      isCurrent = false;
    };
  }, [changesAlternateForm]);
  const [name, setName] = useState(action?.name ?? "");
  const [targetType, setTargetType] = useState(
    action?.actionTargetType ?? ActionTargetType.AllJourneyCharacters,
  );
  const [statType, setStatType] = useState(
    adjustment?.characterStatType ?? CharacterStatType.CurrentHp,
  );
  const [operation, setOperation] = useState(
    adjustment?.adjustmentOperation ?? AdjustmentOperation.Add,
  );
  const [value, setValue] = useState(String(adjustment?.value ?? 0));
  const [characterId, setCharacterId] = useState(
    String(
      spellGrant?.characterId ??
        itemGrant?.characterId ??
        change?.characterId ??
        inForm?.characterId ??
        adjustment?.characterId ??
        "",
    ),
  );
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const submit = async (submitEvent: FormEvent) => {
    submitEvent.preventDefault();
    if (isSaving) return;
    if ((givesSpell || givesItem) && (grantsLoading || grantLoadError)) {
      setError(
        "Wait for the available items or spells to load before applying this action.",
      );
      return;
    }
    if (!name.trim()) {
      setError("An action name is required.");
      return;
    }
    setIsSaving(true);
    setError("");
    try {
      await onSave(
        {
          name: name.trim(),
          actionTargetType: targetType,
          eventActionType: actionType,
          ...(entersAlternateForm
            ? {}
            : clearsAlternateForm
              ? { alternateFormId: null }
              : changesAlternateForm
                ? { alternateFormId: Number(alternateFormId) }
                : givesSpell
                  ? { spellId: Number(spellId) }
                  : givesItem
                    ? {
                        quantity: Number(quantity),
                        consumableItemId:
                          itemType === "consumable" ? Number(itemId) : null,
                        equippableItemId:
                          itemType === "equipment" ? Number(itemId) : null,
                      }
                    : {
                        characterStatType: statType,
                        adjustmentOperation: operation,
                        value: Number(value),
                      }),
          characterId:
            targetType === ActionTargetType.SingleJourneyCharacter
              ? Number(characterId)
              : null,
        },
        {
          spellName:
            grantOptions.find((option) => option.id === Number(spellId))
              ?.name ?? spellGrant?.spellName,
          itemName:
            grantOptions.find((option) => option.id === Number(itemId))?.name ??
            itemGrant?.itemName,
          alternateName:
            alternateCharacters.find(
              (option) => option.id === Number(alternateFormId),
            )?.name ??
            change?.alternateFormName ??
            undefined,
        },
      );
    } catch (requestError) {
      setError(getApiError(requestError).message);
      setIsSaving(false);
    }
  };

  return (
    <form
      onSubmit={(submitEvent) => void submit(submitEvent)}
      className="space-y-5"
    >
      <FormField htmlFor="action-name" label="Name">
        <Input
          id="action-name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          maxLength={200}
          required
        />
      </FormField>
      <FormField htmlFor="action-type" label="Action type">
        <Select
          id="action-type"
          value={actionType}
          onChange={(e) => {
            setActionType(Number(e.target.value) as EventActionType);
            setGrantsLoading(
              [
                EventActionType.CharacterAddSpell,
                EventActionType.CharacterGiveItem,
              ].includes(Number(e.target.value)),
            );
            setGrantOptions([]);
            setGrantLoadError("");
          }}
        >
          <option value={EventActionType.CharacterStatAdjustment}>
            Adjust stat
          </option>
          <option value={EventActionType.CharacterChangeAlternateForm}>
            Change alternate form
          </option>
          <option value={EventActionType.CharacterClearAlternateForm}>
            Clear alternate form
          </option>
          <option value={EventActionType.CharacterInAlternateForm}>
            Enter alternate form
          </option>
          <option value={EventActionType.CharacterAddSpell}>Give spell</option>
          <option value={EventActionType.CharacterGiveItem}>Give item</option>
        </Select>
      </FormField>
      <FormField htmlFor="action-target" label="Target">
        <Select
          id="action-target"
          value={targetType}
          onChange={(e) =>
            setTargetType(Number(e.target.value) as ActionTargetType)
          }
        >
          <option value={ActionTargetType.AllJourneyCharacters}>
            All journey characters
          </option>
          <option value={ActionTargetType.SingleJourneyCharacter}>
            One journey character
          </option>
        </Select>
      </FormField>
      {targetType === ActionTargetType.SingleJourneyCharacter && (
        <FormField htmlFor="action-character" label="Character">
          <Select
            id="action-character"
            value={characterId}
            onChange={(e) => setCharacterId(e.target.value)}
            required
          >
            <option value="" disabled>
              Select a character
            </option>
            {journeyCharacters.map((item) => (
              <option key={item.characterId} value={item.characterId}>
                {item.character.name}
              </option>
            ))}
          </Select>
        </FormField>
      )}
      {givesSpell || givesItem ? (
        <>
          {givesItem && (
            <FormField htmlFor="grant-item-type" label="Item type">
              <Select
                id="grant-item-type"
                value={itemType}
                onChange={(event) => {
                  setItemType(event.target.value);
                  setItemId("");
                  setGrantOptions([]);
                  setGrantLoadError("");
                  setGrantsLoading(true);
                }}
              >
                <option value="consumable">Consumable</option>
                <option value="equipment">Equipment</option>
              </Select>
            </FormField>
          )}
          <FormField
            htmlFor="grant-selection"
            label={givesSpell ? "Spell" : "Item"}
          >
            <Select
              id="grant-selection"
              required
              value={givesSpell ? spellId : itemId}
              onChange={(event) =>
                givesSpell
                  ? setSpellId(event.target.value)
                  : setItemId(event.target.value)
              }
            >
              <option value="" disabled>
                Select {givesSpell ? "a spell" : "an item"}
              </option>
              {givesSpell &&
                spellGrant &&
                !grantOptions.some(
                  (option) => option.id === spellGrant.spellId,
                ) && (
                  <option value={spellGrant.spellId}>
                    {spellGrant.spellName} (existing assignment)
                  </option>
                )}
              {grantOptions.map((option) => (
                <option key={option.id} value={option.id}>
                  {option.name}
                </option>
              ))}
            </Select>
          </FormField>
          {grantLoadError && (
            <div role="alert" className="text-danger">
              {grantLoadError}
              <Button
                disabled={grantsLoading}
                onClick={() => void loadGrantOptions()}
              >
                Retry
              </Button>
            </div>
          )}
          {givesItem && (
            <>
              <FormField
                htmlFor="grant-quantity"
                label="Quantity per character"
              >
                <Input
                  id="grant-quantity"
                  type="number"
                  min={1}
                  max={1000}
                  step={1}
                  required
                  value={quantity}
                  onChange={(event) => setQuantity(event.target.value)}
                />
              </FormField>
              <p className="text-sm text-content-secondary">
                Granted at scene start. Full inventories require discarding or
                transferring an item before continuing. Equipment is equipped
                automatically.
              </p>
            </>
          )}
          {givesSpell && (
            <p className="text-sm text-content-secondary">
              Learned at scene start. Characters who already know this spell
              keep a single copy.
            </p>
          )}
        </>
      ) : entersAlternateForm ? (
        <p className="text-sm text-content-secondary">
          Transforms characters at scene start using their assigned alternate
          forms. When targeting all journey characters, those without an
          alternate form are skipped. A single target must have an alternate
          form assigned when this action runs.
        </p>
      ) : clearsAlternateForm ? (
        <p className="text-sm text-content-secondary">
          Removes the selected characters’ alternate form at scene start and
          returns them to normal form. Inventories and stats are unchanged.
        </p>
      ) : changesAlternateForm ? (
        <FormField
          htmlFor="action-alternate-form"
          label="New alternate character"
        >
          <Select
            id="action-alternate-form"
            value={alternateFormId}
            onChange={(e) => setAlternateFormId(e.target.value)}
            required
          >
            <option value="" disabled>
              Select an alternate character
            </option>
            {alternateCharacters
              .filter((candidate) =>
                targetType === ActionTargetType.SingleJourneyCharacter
                  ? candidate.id !== Number(characterId)
                  : !journeyCharacters.some(
                      (target) => target.characterId === candidate.id,
                    ),
              )
              .map((candidate) => (
                <option key={candidate.id} value={candidate.id}>
                  {candidate.name}
                </option>
              ))}
          </Select>
          <p className="mt-2 text-sm text-content-secondary">
            Changes the alternate form at scene start. Stats and the current
            transformation state are preserved.
          </p>
          {alternateError && (
            <p className="mt-2 text-sm text-danger" role="alert">
              {alternateError}
            </p>
          )}
        </FormField>
      ) : (
        <>
          <div className="grid gap-4 sm:grid-cols-2">
            <FormField htmlFor="action-stat" label="Stat">
              <Select
                id="action-stat"
                value={statType}
                onChange={(e) =>
                  setStatType(Number(e.target.value) as CharacterStatType)
                }
              >
                {enumEntries(statLabels).map(([id, label]) => (
                  <option key={id} value={id}>
                    {label}
                  </option>
                ))}
              </Select>
            </FormField>
            <FormField htmlFor="action-operation" label="Operation">
              <Select
                id="action-operation"
                value={operation}
                onChange={(e) =>
                  setOperation(Number(e.target.value) as AdjustmentOperation)
                }
              >
                {enumEntries(operationLabels).map(([id, label]) => (
                  <option key={id} value={id}>
                    {label}
                  </option>
                ))}
              </Select>
            </FormField>
          </div>
          <FormField htmlFor="action-value" label="Value">
            <Input
              id="action-value"
              type="number"
              value={value}
              onChange={(e) => setValue(e.target.value)}
              required
            />
          </FormField>
        </>
      )}
      {error && (
        <p role="alert" className="text-sm text-danger">
          {error}
        </p>
      )}
      <div className="flex justify-end gap-3 border-t border-border pt-4">
        <Button onClick={onCancel} disabled={isSaving}>
          Cancel Action
        </Button>
        <Button
          type="submit"
          variant="primary"
          disabled={
            isSaving ||
            grantsLoading ||
            !!grantLoadError ||
            (changesAlternateForm && !!alternateError)
          }
        >
          Apply Action
        </Button>
      </div>
    </form>
  );
}
