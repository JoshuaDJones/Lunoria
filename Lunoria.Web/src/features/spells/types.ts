export interface SpellType {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
}

export interface Spell {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  range: number;
  isRadius: boolean;
  mpCost: number;
  damageEffect: number | null;
  healthEffect: number | null;
  magicEffect: number | null;
  createDate: string;
  spellTypeId: number;
  spellType: SpellType | null;
}

export interface SpellInput {
  name: string;
  description: string;
  photo?: File;
  range: number;
  isRadius: boolean;
  mpCost: number;
  damageEffect?: number | null;
  healthEffect?: number | null;
  magicEffect?: number | null;
  spellTypeId: number;
}

export type CreateSpellInput = SpellInput & { photo: File };

export interface SpellTypeInput {
  name: string;
  description: string;
  photo?: File;
}

export type CreateSpellTypeInput = SpellTypeInput & { photo: File };
