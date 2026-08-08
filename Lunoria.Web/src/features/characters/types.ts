import type { Spell } from "@/features/spells/types";

export enum CharacterType {
  Any = 0,
  Player = 1,
  NPC = 2,
  Enemy = 3,
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
  characterType: CharacterType;
  createdAt: string;
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
  characterType: CharacterType;
  alternateFormId?: number | null;
}

export type CreateCharacterInput = CharacterInput & { photo: File };
