import type { Spell } from "@/features/spells/types";

export enum CharacterType {
  Any = 0,
  Enemy = 1,
  NPC = 2,
  Player = 3,
}

export interface CharacterDialogSettings {
  id: number;
  dialogActiveColor: string;
  dialogUnActiveColor: string;
}

export interface CharacterSpell {
  id: number;
  characterId: number;
  spell: Spell;
}

export interface Character {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  maxHp: number;
  maxMp: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  baseMaxConsumableInventory: number;
  baseMaxEquippableInventory: number;
  isPlayer: boolean;
  isNPC: boolean;
  isEnemy: boolean;
  createDate: string;
  alternateFormId: number | null;
  alternateForm: Character | null;
  characterSpells: CharacterSpell[] | null;
  characterDialogSettings: CharacterDialogSettings | null;
}

export interface CharacterInput {
  name: string;
  description: string;
  photo?: File;
  maxHp: number;
  maxMp: number;
  meleeAttackDamage?: number | null;
  bowAttackDamage?: number | null;
  movement: number;
  baseMaxConsumableInventory: number;
  baseMaxEquippableInventory: number;
  isPlayer: boolean;
  isNPC: boolean;
  isEnemy: boolean;
  alternateFormId?: number | null;
}

export type CreateCharacterInput = CharacterInput & { photo: File };
