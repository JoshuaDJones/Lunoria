import type { Character } from "@/features/characters/types";
import type { Item } from "@/features/items/types";
import type { JourneyCharacter } from "@/features/journeys/types";
import type { Spell } from "@/features/spells/types";

export enum SceneProgressStatus {
  NotStarted = 0,
  InProgress = 1,
  Completed = 2,
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
  characterId: number | null;
  character: Character | null;
  sceneCharacterSpells: SceneCharacterSpell[];
}

export interface SceneCharacterSpell {
  id: number; 
  sceneCharacter: SceneCharacter;
  spell: Spell;
}

export interface Scene {
  id: number;
  journeyId: number;
  name: string;
  description: string;
  photoUrl: string;
  gridUrl: string;
  createDate: string;
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
