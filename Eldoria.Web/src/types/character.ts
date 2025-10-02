import { SpellDto } from "./spell";

export interface CharacterDto {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  maxHp: number;
  maxMp: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  maxInventory: number;
  isPlayer: boolean;
  isNPC: boolean;
  isEnemy: boolean;
  createDate: string;
  alternateFormId: number | null;
  alternateForm: CharacterDto | null;
  characterSpells: CharacterSpellDto[] | null;
}

export interface CharacterSpellDto {
  id: number;
  characterId: number;
  spell: SpellDto;
}
