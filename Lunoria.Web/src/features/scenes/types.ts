import type { Character } from "@/features/characters/types";
import type { EquippableItem } from "@/features/equipment/types";
import type { Item } from "@/features/items/types";
import type { JourneyCharacter } from "@/features/journeys/types";
import type { Spell } from "@/features/spells/types";

export enum SceneProgressStatus {
  NotStarted = 0,
  InProgress = 1,
  Completed = 2,
}

export enum ActionTargetType {
  AllJourneyCharacters = 1,
  SingleJourneyCharacter = 2,
}

export enum EventActionType {
  CharacterStatAdjustment = 1,
}

export enum CharacterStatType {
  CurrentHp = 1,
  CurrentMp = 2,
  MaxHp = 3,
  MaxMp = 4,
  Movement = 5,
  MeleeAttackDamage = 6,
  BowAttackDamage = 7,
}

export enum AdjustmentOperation {
  Add = 1,
  Subtract = 2,
  Set = 3,
  Multiply = 4,
}

export interface CharacterStatAdjustmentAction {
  id: number;
  characterStatType: CharacterStatType;
  adjustmentOperation: AdjustmentOperation;
  value: number;
  characterId: number | null;
  character: Character | null;
}

export interface SceneEventAction {
  id: number;
  name: string;
  sortOrder: number;
  actionTargetType: ActionTargetType;
  eventActionType: EventActionType;
  sceneEventId: number;
  characterStatAdjustmentAction: CharacterStatAdjustmentAction | null;
}

export interface SceneEvent {
  id: number;
  name: string;
  description: string | null;
  sortOrder: number;
  sceneId: number;
  sceneEventActions: SceneEventAction[];
}

export interface SceneEventInput {
  name: string;
  description?: string | null;
}

export interface SceneChestLootEntry {
  id: number;
  rollMinimum: number;
  rollMaximum: number;
  quantity: number;
  equippableItem: EquippableItem | null;
  consumableItem: Item | null;
  sceneChestId: number;
}

export interface SceneChest {
  id: number;
  name: string;
  dieSides: number;
  sceneId: number;
  lootEntries: SceneChestLootEntry[];
}

export interface SceneChestInput {
  name: string;
  dieSides: number;
}

export interface SceneChestLootEntryInput {
  rollMinimum: number;
  rollMaximum: number;
  quantity: number;
  equippableItemId: number | null;
  consumableItemId: number | null;
}

export interface SceneEventActionInput {
  name: string;
  actionTargetType: ActionTargetType;
  eventActionType: EventActionType;
  characterStatType: CharacterStatType;
  adjustmentOperation: AdjustmentOperation;
  value: number;
  characterId?: number | null;
}

export interface DialogPageSection {
  id: number;
  orderNum: number;
  readingText: string;
  isNarrator: boolean;
  character: Character | null;
}

export interface DialogPage {
  id: number;
  orderNum: number;
  photoUrl: string | null;
  dialogPageSections: DialogPageSection[] | null;
}

export interface SceneDialog {
  id: number;
  title: string;
  dialogPages: DialogPage[] | null;
}

export interface SceneCharacterItem {
  id: number;
  isUsed: boolean;
  sceneCharacterId: number;
  itemId: number;
  item: Item;
}

export interface SceneCharacter {
  id: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  maxConsumableInventory: number;
  maxEquippableInventory: number;
  maxHp: number;
  maxMp: number;
  isInitiallyActive: boolean;
  sceneId: number;
  alternateFormId: number | null;
  isAlternateForm: boolean;
  alternateForm: Character | null;
  characterId: number;
  character: Character;
  sceneCharacterSpells: SceneCharacterSpell[];
}

export interface SceneCharacterSpell {
  id: number;
  sceneCharacterId: number;
  spell: Spell;
}

export interface SceneCharacterInput {
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  maxConsumableInventory: number;
  maxEquippableInventory: number;
  maxHp: number;
  maxMp: number;
  isInitiallyActive: boolean;
  alternateFormId: number | null;
}

export interface Scene {
  id: number;
  journeyId: number;
  name: string;
  description: string;
  photoUrl: string;
  gridUrl: string;
  sortOrder: number;
  createdAt: string;
  sceneDialogs: SceneDialog[] | null;
  sceneCharacters: SceneCharacter[] | null;
}

export interface SceneDashboard {
  scene: Scene;
  players: JourneyCharacter[];
}

export interface SceneInput {
  journeyId: number;
  name: string;
  description: string;
  photo?: File;
  gridUrl: string;
}

export type CreateSceneInput = SceneInput & { photo: File };

export interface SceneParticipantTurn {
  id: number;
  sceneProgressId: number;
  sceneParticipantId: number;
  turnPosition: number;
}

export interface SceneParticipant {
  id: number;
  sceneProgressId: number;
  journeyCharacterId: number | null;
  sceneCharacterId: number | null;
  turns: SceneParticipantTurn[];
}

export interface SceneProgress {
  id: number;
  sceneId: number;
  journeyPlaythroughId: number;
  status: SceneProgressStatus;
  participants: SceneParticipant[];
  turns: SceneParticipantTurn[];
}
