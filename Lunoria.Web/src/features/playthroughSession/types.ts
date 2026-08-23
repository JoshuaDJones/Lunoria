import type { CharacterType } from "@/features/characters";

export interface PlaythroughJoinSession {
  token: string;
  expiresAt: string;
}

export interface PublicPlaythroughSpell {
  id: number;
  name: string;
  description: string;
  photoUrl: string | null;
  spellType: string;
  range: number;
  isRadius: boolean;
  mpCost: number;
  damageEffect: number | null;
  healthEffect: number | null;
  magicEffect: number | null;
}

export interface PublicPlaythroughConsumableItem {
  id: number;
  name: string;
  description: string;
  photoUrl: string | null;
  hpEffect: number;
  mpEffect: number;
  isUsed: boolean;
}

export interface PublicPlaythroughEquippableItem {
  id: number;
  name: string;
  description: string;
  photoUrl: string | null;
  isEquipped: boolean;
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
  affectedSpellType: string | null;
  spellDamageModifier: number | null;
  addedSpells: PublicPlaythroughSpell[];
}

export interface PublicPlaythroughCharacter {
  id: number;
  isSceneCharacter: boolean;
  characterType: CharacterType;
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
  maxConsumableInventory: number;
  maxEquippableInventory: number;
  isActive: boolean;
  isDown: boolean;
  isDead: boolean;
  isInAlternateForm: boolean;
  spells: PublicPlaythroughSpell[];
  consumableItems: PublicPlaythroughConsumableItem[];
  equippableItems: PublicPlaythroughEquippableItem[];
}

export interface PublicPlaythroughEventLog {
  id: number;
  message: string;
  eventTime: string;
}

export interface PublicPlaythroughSnapshot {
  name: string;
  activeSceneName: string | null;
  journeyCharacters: PublicPlaythroughCharacter[];
  sceneCharacters: PublicPlaythroughCharacter[];
  eventLogs: PublicPlaythroughEventLog[];
}
