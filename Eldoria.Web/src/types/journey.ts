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
  introPages: IntroPageDto[];
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

export interface IntroPageDto {
  id?: number;
  journeyId: number;
  order: number;
  type: IntroPageType;
  config: string;
  previewPhotoUrl?: string;
}

export enum IntroPageType {
  ImageTop_ContentBottom = "ImageTop_ContentBottom",
  ImageLeft_ContentRight = "ImageLeft_ContentRight",
  ImageRight_ContentLeft = "ImageRight_ContentLeft",
  ImageCenter_OverlayCenterText = "ImageCenter_OverlayCenterText",
  CharacterShowcase = "CharacterShowcase",
}

export type IntroPage =
  | {
      id?: number;
      journeyId: number;
      order: number;
      previewPhotoUrl?: string;
      type: IntroPageType.ImageTop_ContentBottom;
      config: {
        imageUrl: string;
        content: string;
      };
    }
  | {
      id?: number;
      journeyId: number;
      order: number;
      previewPhotoUrl?: string;
      type: IntroPageType.ImageLeft_ContentRight;
      config: {
        imageUrl: string;
        content: string;
      };
    }
  | {
      id?: number;
      journeyId: number;
      order: number;
      previewPhotoUrl?: string;
      type: IntroPageType.ImageRight_ContentLeft;
      config: {
        imageUrl: string;
        content: string;
      };
    }
  | {
      id?: number;
      journeyId: number;
      order: number;
      previewPhotoUrl?: string;
      type: IntroPageType.ImageCenter_OverlayCenterText;
      config: {
        imageUrl: string;
        content: string;
      };
    }
  | {
      id?: number;
      journeyId: number;
      order: number;
      previewPhotoUrl?: string;
      type: IntroPageType.CharacterShowcase;
      config: {
        imageUrl: string;
        content: string;
        characterId: number;
      };
    };

export function fromDto(dto: IntroPageDto): IntroPage {
  return {
    ...dto,
    config: JSON.parse(dto.config),
  } as IntroPage;
}

export function toDto(page: IntroPage): IntroPageDto {
  return {
    ...page,
    config: JSON.stringify(page.config),
  };
}
