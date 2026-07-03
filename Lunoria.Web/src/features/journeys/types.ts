import type { Character } from "@/features/characters/types";
import type { EquippableItem } from "@/features/equipment/types";
import type { Item } from "@/features/items/types";
import type { Scene } from "@/features/scenes/types";
import type { Spell } from "@/features/spells/types";

export enum IntroPageType {
  ImageTopContentBottom = 0,
  ImageLeftContentRight = 1,
  ImageRightContentLeft = 2,
  ImageCenterOverlayCenterText = 3,
  CharacterShowcase = 4,
}

export interface IntroPage {
  id: number;
  journeyId: number;
  order: number;
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

export interface JourneyCharacterEquippableItem {
  id: number;
  journeyCharacterId: number;
  equippableItemId: number;
  isEquipped: boolean;
  equippableItem: EquippableItem;
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
  isDown: boolean;
  alternateFormId: number | null;
  isAlternateForm: boolean;
  alternateForm: Character | null;
  character: Character;
  journeyCharacterItems: JourneyCharacterItem[];
  journeyCharacterEquippableItems: JourneyCharacterEquippableItem[];
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

export interface Journey {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  createDate: string;
  scenes: Scene[] | null;
  journeyCharacters: JourneyCharacter[] | null;
  introPages: IntroPage[] | null;
}

export interface JourneyInput {
  name: string;
  description: string;
  photo?: File;
}

export type CreateJourneyInput = JourneyInput & { photo: File };
