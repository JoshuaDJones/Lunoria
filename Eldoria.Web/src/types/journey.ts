import { CharacterDto } from "./character";
import { ItemDto } from "./item";
import { SceneDto } from "./scene";

export interface JourneyDto {
  id: number;
  name: string;
  description: string;
  photoUrl: string;
  createDate: string;
  scenes: SceneDto[];
  journeyCharacters: JourneyCharacterDto[];
}

export interface JourneyCharacterDto {
  id: number;
  journeyId: number;
  characterId: number;
  currentHp: number;
  currentMp: number;
  isDown: boolean;
  isAlternateForm: boolean;
  character: CharacterDto;
  journeyCharacterItems: JourneyCharacterItemDto[];
}

export interface JourneyCharacterItemDto {
  id: number;
  journeyCharacterId: number;
  isUsed: boolean;
  itemId: number;
  item: ItemDto;
}
