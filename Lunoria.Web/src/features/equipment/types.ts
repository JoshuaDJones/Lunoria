import type { Spell, SpellType } from "@/features/spells/types";

export interface EquippableItem {
  id: number;
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
  addedSpells: Spell[];
  meleeDamageReduction: number;
  bowDamageReduction: number;
  spellDamageReduction: number;
  affectedSpellTypeId: number | null;
  affectedSpellType: SpellType | null;
  spellDamageModifier: number;
}

export interface EquippableItemInput {
  name: string;
  description: string;
  photo?: File;
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
  affectedSpellTypeId?: number | null;
  spellDamageModifier: number;
  addedSpellIds: number[];
}

export type CreateEquippableItemInput = EquippableItemInput & { photo: File };
