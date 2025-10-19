import SceneDashboard from "../pages/authenticated/SceneDashboard";
import { CharacterDto } from "./character";
import { ItemDto } from "./item";
import { JourneyCharacterDto } from "./journey";

export interface SceneDto {
  id: number;
  journeyId: string;
  name: string;
  description: string;
  photoUrl: string;
  gridUrl: string;
  createDate: string;
  sceneDialogs: SceneDialogDto[];
  sceneCharacters: SceneCharacterDto[];
}

export interface SceneDashboardDto {
  scene: SceneDto;
  players: JourneyCharacterDto[];
}

export interface SceneDialogDto {
  id: number;
  sceneId: number;
  orderNum: number;
  photoUrl: string;
  dialog: string;
  createDate: string;
  characterId: number;
  character: CharacterDto;
}

export interface SceneCharacterDto {
  id: number;
  currentHp: number;
  currentMp: number;
  isDown: boolean;
  isAlternateForm: boolean;
  sceneId: number;
  characterId: number;
  character: CharacterDto;
  sceneCharacterItems: SceneCharacterItemDto[];
}

export interface SceneCharacterItemDto {
  id: number;
  isUsed: boolean;
  sceneCharacterId: number;
  itemId: number;
  item: ItemDto;
}
