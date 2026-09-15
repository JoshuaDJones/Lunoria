import type { Character } from "@/features/characters/types";
import type { Item } from "@/features/items/types";
import type { DialogPageType, Scene } from "@/features/scenes/types";
import type { Spell } from "@/features/spells/types";

export enum IntroPageType {
  ImageTopContentBottom = 1,
  ImageLeftContentRight = 2,
  ImageRightContentLeft = 3,
  ImageCenterOverlayCenterText = 4,
  CharacterShowcase = 5,
}

export interface IntroPage {
  id: number;
  journeyId: number;
  sortOrder: number;
  type: IntroPageType;
  config: string;
  previewPhotoUrl: string | null;
}

export interface JourneyCharacterItem {
  id: number;
  journeyCharacterId: number;
  isUsed: boolean;
  itemId: number;
  item: Item;
}

export interface JourneyCharacterSpell {
  id: number;
  journeyCharacterId: number;
  spellId: number;
  spell: Spell;
}

export interface SpellDamageModifier {
  spellTypeId: number | null;
  spellTypeName: string | null;
  modifier: number;
}

export interface JourneyCharacter {
  sortOrder: number;
  id: number;
  journeyId: number;
  characterId: number;
  currentHp: number;
  currentMp: number;
  maxHp: number;
  maxMp: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  maxConsumableInventory: number;
  maxEquippableInventory: number;
  isInitiallyActive: boolean;
  isDown: boolean;
  alternateFormId: number | null;
  isAlternateForm: boolean;
  alternateForm: Character | null;
  character: Character;
  journeyCharacterItems: JourneyCharacterItem[];
  journeyCharacterSpells: JourneyCharacterSpell[];
  effectiveMaxHp: number;
  effectiveMaxMp: number;
  effectiveMeleeAttackDamage: number | null;
  effectiveBowAttackDamage: number | null;
  effectiveMovement: number;
  effectiveMaxConsumableInventory: number;
  effectiveMaxEquippableInventory: number;
  effectiveMeleeDamageReduction: number;
  effectiveBowDamageReduction: number;
  effectiveSpellDamageReduction: number;
  effectiveSpellDamageModifiers: SpellDamageModifier[];
  effectiveSpells: Spell[];
}

export interface ScenePlaythroughJourneyCharacterOption {
  id: number;
  playthroughCharacterId: number;
  name: string;
  photoUrl: string | null;
  portraitUrl: string | null;
  isActive: boolean;
  isParticipant: boolean;
}

export interface ScenePlaythroughCharacterOption {
  id: number;
  name: string;
  description: string;
  photoUrl: string | null;
  portraitUrl: string | null;
  characterType: number;
}

export interface Journey {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  createdAt: string;
  scenes: Scene[] | null;
  journeyCharacters: JourneyCharacter[] | null;
  introPages: IntroPage[] | null;
}

export interface PlaythroughSummary {
  id: number;
  sourceJourneyId: number;
  name: string;
  description: string;
  photoUrl: string;
  startedAt: string;
  completedAt: string | null;
  isCompleted: boolean;
}

export enum ScenePlaythroughStatus {
  NotStarted = 1,
  InProgress = 2,
  Completed = 3,
}

export interface PlaythroughSceneSummary {
  hasPendingInventory: boolean;
  id: number;
  name: string;
  description: string | null;
  photoUrl: string | null;
  gridUrl: string | null;
  sortOrder: number;
  status: ScenePlaythroughStatus;
  roundNumber: number;
  startedAt: string | null;
  endedAt: string | null;
}

export interface SceneStartInventory {
  resolutionToken: string;
  characterName: string;
  eventName: string;
  rewardName: string;
  isEquippable: boolean;
  remainingQuantity: number;
  inventoryCount: number;
  inventoryCapacity: number;
  items: { id: number; name: string; recipientIds: number[] }[];
  rewardRecipientIds: number[];
  recipients: { id: number; name: string; availableSlots: number }[];
}

export interface SceneStartResult {
  started: boolean;
  pendingInventory: SceneStartInventory | null;
}

export interface SceneInventoryResolutionInput {
  resolutionToken: string;
  inventoryItemId: number | null;
  targetJourneyCharacterId: number | null;
}

export interface PlaythroughIntroPage {
  id: number;
  sortOrder: number;
  type: IntroPageType;
  config: string;
  previewPhotoUrl: string | null;
}

export interface PlaythroughEventLog {
  id: number;
  message: string;
  eventTime: string;
}

export interface PlaythroughCreated {
  id: number;
}

export interface PlaythroughDetails {
  playthrough: PlaythroughSummary;
  scenes: PlaythroughSceneSummary[];
  introPages: PlaythroughIntroPage[];
  eventLogs: PlaythroughEventLog[];
}

export enum ParticipantType {
  Player = 1,
  NPC = 2,
  Enemy = 3,
}

export enum SceneAttackType {
  Melee = 1,
  Range = 2,
  Spell = 3,
}

export interface ScenePlaythroughSpell {
  range: number;
  isRadius: boolean;
  isUtility: boolean;
  healthEffect: number | null;
  magicEffect: number | null;
  isSupport: boolean;
  id: number;
  name: string;
  description: string;
  mpCost: number;
  damageEffect: number | null;
}

export interface ScenePlaythroughInventoryItem {
  inventoryItemId: number;
  isEquippable: boolean;
  isEquipped: boolean;
  item: ScenePlaythroughLootItem;
}

export interface ScenePlaythroughParticipant {
  alternateForm: {
    id: number;
    name: string;
    description: string;
    maxHp: number;
    maxMp: number;
    movement: number;
    meleeAttackDamage: number | null;
    bowAttackDamage: number | null;
    maxConsumableInventory: number;
    maxEquippableInventory: number;
    spells: ScenePlaythroughSpell[];
  } | null;
  id: number;
  participantType: ParticipantType;
  sortOrderWithinType: number | null;
  isActive: boolean;
  isCurrentParticipant: boolean;
  attacksPerTurn: number;
  attacksRemaining: number;
  journeyPlaythroughCharacterId: number | null;
  scenePlaythroughCharacterId: number | null;
  playthroughCharacterId: number;
  displayedPlaythroughCharacterId: number;
  name: string;
  description: string;
  photoUrl: string | null;
  portraitUrl: string | null;
  currentHp: number;
  maxHp: number;
  currentMp: number;
  maxMp: number;
  movement: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  meleeDamageReduction: number;
  bowDamageReduction: number;
  spellDamageReduction: number;
  isDown: boolean;
  isDead: boolean;
  isInAlternateForm: boolean;
  canTransform: boolean;
  downedTurnsRemaining: number | null;
  maxConsumableInventory: number;
  maxEquippableInventory: number;
  spells: ScenePlaythroughSpell[];
  consumableItems: ScenePlaythroughInventoryItem[];
  equippableItems: ScenePlaythroughInventoryItem[];
}

export interface SceneAttackResult {
  isUtility: boolean;
  isSupport: boolean;
  healthRestored: number;
  magicRestored: number;
  damage: number;
  targetCurrentHp: number;
  targetDefeated: boolean;
  rewardStat: string | null;
  rewardAmount: number;
}

export interface SceneOpenChestResult {
  chestId: number;
  chestName: string;
  roll: number;
  quantity: number;
  isEquippable: boolean;
  awarded: boolean;
  item: ScenePlaythroughLootItem;
}

export interface SceneUseConsumableResult {
  inventoryItemId: number;
  itemId: number;
  itemName: string;
  hpRestored: number;
  mpRestored: number;
  currentHp: number;
  maxHp: number;
  currentMp: number;
  maxMp: number;
}

export enum ChestStatus {
  Unopened = 1,
  Opened = 2,
}

export interface ScenePlaythroughLootItem {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  hpEffect: number | null;
  mpEffect: number | null;
  meleeAttackDamageModifier: number | null;
  bowAttackDamageModifier: number | null;
  movementModifier: number | null;
  maxHpModifier: number | null;
  maxMpModifier: number | null;
  maxConsumableInventoryModifier: number | null;
  maxEquippableInventoryModifier: number | null;
  additionalAttacksPerTurn: number | null;
  meleeDamageReduction: number | null;
  bowDamageReduction: number | null;
  spellDamageReduction: number | null;
  affectedSpellType: string | null;
  spellDamageModifier: number | null;
  addedSpells: ScenePlaythroughSpell[];
}

export interface ScenePlaythroughChestLootEntry {
  id: number;
  rollMinimum: number;
  rollMaximum: number;
  quantity: number;
  equippableItem: ScenePlaythroughLootItem | null;
  consumableItem: ScenePlaythroughLootItem | null;
}

export interface ScenePlaythroughChest {
  id: number;
  name: string;
  dieSides: number;
  status: ChestStatus;
  rolledValue: number | null;
  openedAt: string | null;
  selectedLootEntryId: number | null;
  lootEntries: ScenePlaythroughChestLootEntry[];
}

export interface ScenePlaythroughDialogCharacter {
  id: number;
  name: string;
  photoUrl: string | null;
  portraitUrl: string | null;
  dialogActiveColor: string;
  dialogInActiveColor: string;
}

export interface ScenePlaythroughDialogSection {
  id: number;
  orderNum: number;
  readingText: string;
  isNarrator: boolean;
  character: ScenePlaythroughDialogCharacter | null;
}

export interface ScenePlaythroughDialogPage {
  id: number;
  orderNum: number;
  pageType: DialogPageType;
  mediaUrl: string;
  mediaContentType: string;
  dialogPageSections: ScenePlaythroughDialogSection[];
}

export interface ScenePlaythroughDialog {
  id: number;
  title: string;
  dialogPages: ScenePlaythroughDialogPage[];
}

export interface ScenePlaythroughDetails {
  counterattackerId: number | null;
  counterattackTargetId: number | null;
  counterattackToken: string | null;
  gridUrl: string | null;
  grid: {
    rows: number;
    columns: number;
    gridColor: string;
    backgroundImageUrl: string | null;
  } | null;
  id: number;
  playthroughId: number;
  name: string;
  description: string | null;
  photoUrl: string | null;
  status: ScenePlaythroughStatus;
  roundNumber: number;
  startedAt: string | null;
  endedAt: string | null;
  currentParticipantId: number | null;
  participants: ScenePlaythroughParticipant[];
  journeyCharacters: ScenePlaythroughJourneyCharacterOption[];
  playthroughCharacters: ScenePlaythroughCharacterOption[];
  availableConsumableItems: ScenePlaythroughLootItem[];
  availableEquippableItems: ScenePlaythroughLootItem[];
  chests: ScenePlaythroughChest[];
  dialogs: ScenePlaythroughDialog[];
  eventLogs: PlaythroughEventLog[];
}

export interface JourneyInput {
  name: string;
  description: string;
  photo?: File;
}

export type CreateJourneyInput = JourneyInput & {
  seriesId: number;
  photo: File;
};
