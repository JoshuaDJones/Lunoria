export interface JourneyPlaythrough {
  id: number;
  journeyId: number;
  revisionId: number;
  revisionNumber: number;
  snapshotSchemaVersion: number;
  snapshot: JourneySnapshot;
  startedAt: string;
  completedAt: string | null;
  isActive: boolean;
}

export interface JourneySnapshot {
  schemaVersion: number;
  journey: SnapshotJourney;
  characters: SnapshotCharacter[];
  spellTypes: SnapshotSpellType[];
  spells: SnapshotSpell[];
  consumables: SnapshotConsumable[];
  equipment: SnapshotEquipment[];
  scenes: SnapshotScene[];
}

export interface SnapshotJourney {
  sourceJourneyId: number;
  name: string;
  description: string;
  photoUrl: string;
  fileName: string;
  sortOrder: number;
  introPages: SnapshotIntroPage[];
  characters: SnapshotAssignedCharacter[];
  sceneKeys: string[];
}

export interface SnapshotCharacter {
  key: string;
  sourceCharacterId: number;
  name: string;
  description: string;
  photoUrl: string;
  fileName: string;
  portraitUrl: string | null;
  portraitFileName: string | null;
  baseMaxHp: number;
  baseMaxMp: number;
  baseMeleeAttackDamage: number | null;
  baseBowAttackDamage: number | null;
  baseMovement: number;
  baseMaxConsumableInventory: number;
  baseMaxEquippableInventory: number;
  characterType: number;
  baseAlternateFormCharacterKey: string | null;
  dialogSettings: { activeColor: string; inactiveColor: string } | null;
  spellKeys: string[];
}

export interface SnapshotAssignedCharacter {
  key: string;
  sourceJourneyCharacterId: number;
  characterKey: string;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  maxConsumableInventory: number;
  maxEquippableInventory: number;
  maxHp: number;
  maxMp: number;
  isInitiallyActive: boolean;
  alternateFormCharacterKey: string | null;
  spells: SnapshotAssignedSpell[];
}

export interface SnapshotAssignedSpell {
  key: string;
  sourceAssignmentId: number;
  spellKey: string;
}

export interface SnapshotSpellType {
  key: string;
  sourceSpellTypeId: number;
  name: string;
  description: string;
  photoUrl: string;
  fileName: string;
}

export interface SnapshotSpell {
  key: string;
  sourceSpellId: number;
  name: string;
  description: string;
  photoUrl: string | null;
  fileName: string | null;
  range: number;
  isRadius: boolean;
  mpCost: number;
  damageEffect: number | null;
  healthEffect: number | null;
  magicEffect: number | null;
  spellTypeKey: string;
}

export interface SnapshotConsumable {
  key: string;
  sourceConsumableId: number;
  name: string;
  description: string;
  photoUrl: string;
  fileName: string;
  hpEffect: number;
  mpEffect: number;
}

export interface SnapshotEquipment {
  key: string;
  sourceEquipmentId: number;
  name: string;
  description: string;
  photoUrl: string;
  fileName: string;
  meleeAttackDamageModifier: number;
  bowAttackDamageModifier: number;
  movementModifier: number;
  maxHpModifier: number;
  maxMpModifier: number;
  maxConsumableInventoryModifier: number;
  maxEquippableInventoryModifier: number;
  meleeDamageReduction: number;
  bowDamageReduction: number;
  spellDamageReduction: number;
  affectedSpellTypeKey: string | null;
  spellDamageModifier: number | null;
  addedSpellKeys: string[];
}

export interface SnapshotScene {
  key: string;
  sourceSceneId: number;
  name: string;
  description: string | null;
  photoUrl: string | null;
  fileName: string | null;
  gridUrl: string | null;
  sortOrder: number;
  grid: SnapshotGrid | null;
  introPages: SnapshotIntroPage[];
  characters: SnapshotSceneCharacter[];
  dialogs: SnapshotDialog[];
  events: SnapshotEvent[];
  chests: SnapshotChest[];
}

export interface SnapshotGrid {
  rows: number;
  columns: number;
  gridColor: string;
  backgroundImageUrl: string | null;
  backgroundFileName: string | null;
}

export interface SnapshotIntroPage {
  key: string;
  sourceId: number;
  sortOrder: number;
  type: number;
  config: string;
  previewPhotoUrl: string | null;
}

export interface SnapshotSceneCharacter extends Omit<SnapshotAssignedCharacter, "sourceJourneyCharacterId"> {
  sourceSceneCharacterId: number;
}

export interface SnapshotDialog {
  key: string;
  sourceDialogId: number;
  title: string;
  pages: Array<{
    key: string;
    sourcePageId: number;
    orderNumber: number;
    photoUrl: string | null;
    fileName: string | null;
    sections: Array<{
      key: string;
      sourceSectionId: number;
      orderNumber: number;
      readingText: string;
      isNarrator: boolean;
      characterKey: string | null;
    }>;
  }>;
}

export interface SnapshotEvent {
  key: string;
  sourceEventId: number;
  name: string;
  description: string | null;
  sortOrder: number;
  actions: Array<{
    key: string;
    sourceActionId: number;
    name: string;
    sortOrder: number;
    targetType: number;
    actionType: number;
    characterStatAdjustment: {
      statType: number;
      operation: number;
      value: number;
      characterKey: string | null;
    } | null;
  }>;
}

export interface SnapshotChest {
  key: string;
  sourceChestId: number;
  name: string;
  dieSides: number;
  lootEntries: Array<{
    key: string;
    sourceLootEntryId: number;
    rollMinimum: number;
    rollMaximum: number;
    quantity: number;
    equipmentKey: string | null;
    consumableKey: string | null;
  }>;
}
