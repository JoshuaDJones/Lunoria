import {
  ActionTargetType,
  AdjustmentOperation,
  CharacterStatType,
  EventActionType,
  type SceneEventAction,
  type SceneEventActionInput,
} from "@/features/scenes/types";
import type { JourneyCharacter } from "@/features/journeys";
export const statLabels: Record<CharacterStatType, string> = {
  [CharacterStatType.CurrentHp]: "Current HP",
  [CharacterStatType.CurrentMp]: "Current MP",
  [CharacterStatType.MaxHp]: "Max HP",
  [CharacterStatType.MaxMp]: "Max MP",
  [CharacterStatType.Movement]: "Movement",
  [CharacterStatType.MeleeAttackDamage]: "Melee attack damage",
  [CharacterStatType.BowAttackDamage]: "Bow attack damage",
};

export const operationLabels: Record<AdjustmentOperation, string> = {
  [AdjustmentOperation.Add]: "Add",
  [AdjustmentOperation.Subtract]: "Subtract",
  [AdjustmentOperation.Set]: "Set",
  [AdjustmentOperation.Multiply]: "Multiply",
};

export interface SceneEventActionLabels {
  spellName?: string;
  itemName?: string;
  alternateName?: string;
}
export function actionToInput(
  action: SceneEventAction,
): SceneEventActionInput & { id?: number } {
  const payload =
    action.characterStatAdjustmentAction ??
    action.characterAddSpellAction ??
    action.characterGiveItemAction ??
    action.characterChangeAlternateFormAction ??
    action.characterInAlternateFormAction;
  return {
    id: action.id > 0 ? action.id : undefined,
    name: action.name,
    actionTargetType: action.actionTargetType,
    eventActionType: action.eventActionType,
    characterId: payload?.characterId ?? null,
    characterStatType: action.characterStatAdjustmentAction?.characterStatType,
    adjustmentOperation:
      action.characterStatAdjustmentAction?.adjustmentOperation,
    value: action.characterStatAdjustmentAction?.value,
    spellId: action.characterAddSpellAction?.spellId,
    consumableItemId: action.characterGiveItemAction?.consumableItemId,
    equippableItemId: action.characterGiveItemAction?.equippableItemId,
    quantity: action.characterGiveItemAction?.quantity,
    alternateFormId: action.characterChangeAlternateFormAction?.alternateFormId,
  };
}
export function draftAction(
  input: SceneEventActionInput,
  id: number,
  labels: SceneEventActionLabels,
  characters: JourneyCharacter[],
): SceneEventAction {
  const character =
    characters.find((item) => item.characterId === input.characterId)
      ?.character ?? null;
  const target = {
    characterId: input.characterId ?? null,
    characterName: character?.name ?? null,
  };
  return {
    id,
    name: input.name,
    sortOrder: 0,
    sceneEventId: 0,
    actionTargetType: input.actionTargetType,
    eventActionType: input.eventActionType,
    characterStatAdjustmentAction:
      input.eventActionType === EventActionType.CharacterStatAdjustment
        ? {
            id: 0,
            characterId: target.characterId,
            character,
            characterStatType: input.characterStatType!,
            adjustmentOperation: input.adjustmentOperation!,
            value: input.value!,
          }
        : null,
    characterAddSpellAction:
      input.eventActionType === EventActionType.CharacterAddSpell
        ? {
            ...target,
            spellId: input.spellId!,
            spellName: labels.spellName ?? "Selected spell",
          }
        : null,
    characterGiveItemAction:
      input.eventActionType === EventActionType.CharacterGiveItem
        ? {
            ...target,
            consumableItemId: input.consumableItemId ?? null,
            equippableItemId: input.equippableItemId ?? null,
            quantity: input.quantity!,
            itemName: labels.itemName ?? "Selected item",
          }
        : null,
    characterChangeAlternateFormAction: [
      EventActionType.CharacterChangeAlternateForm,
      EventActionType.CharacterClearAlternateForm,
    ].includes(input.eventActionType)
      ? {
          ...target,
          alternateFormId: input.alternateFormId ?? null,
          alternateFormName: labels.alternateName ?? null,
        }
      : null,
    characterInAlternateFormAction:
      input.eventActionType === EventActionType.CharacterInAlternateForm
        ? target
        : null,
  };
}
export function actionSummary(
  action: SceneEventAction,
  characters: JourneyCharacter[],
): string {
  const input = actionToInput(action);
  const target =
    action.actionTargetType === ActionTargetType.AllJourneyCharacters
      ? "All players"
      : (characters.find((item) => item.characterId === input.characterId)
          ?.character.name ??
        action.characterAddSpellAction?.characterName ??
        action.characterGiveItemAction?.characterName ??
        action.characterChangeAlternateFormAction?.characterName ??
        action.characterInAlternateFormAction?.characterName ??
        action.characterStatAdjustmentAction?.character?.name ??
        "Unavailable player");
  if (action.characterGiveItemAction)
    return `${target} → ${action.characterGiveItemAction.itemName} ×${action.characterGiveItemAction.quantity}`;
  if (action.characterAddSpellAction)
    return `${target} → learn ${action.characterAddSpellAction.spellName}`;
  if (action.eventActionType === EventActionType.CharacterClearAlternateForm)
    return `${target} → clear alternate form`;
  if (action.eventActionType === EventActionType.CharacterInAlternateForm)
    return `${target} → enter alternate form`;
  if (action.characterChangeAlternateFormAction)
    return `${target} → alternate form: ${action.characterChangeAlternateFormAction.alternateFormName ?? "Unavailable character"}`;
  if (action.characterStatAdjustmentAction) {
    const stat = action.characterStatAdjustmentAction;
    return `${target} → ${statLabels[stat.characterStatType]}: ${operationLabels[stat.adjustmentOperation]} ${stat.value}`;
  }
  return action.name;
}
export function enumEntries<T extends number>(
  labels: Record<T, string>,
): [T, string][] {
  return Object.entries(labels).map(([id, label]) => [
    Number(id) as T,
    label as string,
  ]);
}
