import { apiClient } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";
import {
  Character,
  CharacterInput,
  CharacterType,
  CreateCharacterInput,
} from "@/features/characters/types";

export interface ListCharactersParams {
  skip?: number;
  take?: number;
  typeFilter?: CharacterType;
}

export async function listCharacters(
  params: ListCharactersParams = {},
): Promise<Character[]> {
  if (params.typeFilter === CharacterType.Any) {
    const characterGroups = await Promise.all(
      [CharacterType.Player, CharacterType.NPC, CharacterType.Enemy].map(
        (typeFilter) => listCharacters({ ...params, typeFilter }),
      ),
    );

    return characterGroups.flat();
  }

  const { data } = await apiClient.get<Character[]>("/Character", { params });
  return data;
}

export async function getCharacter(id: number): Promise<Character> {
  const { data } = await apiClient.get<Character>(`/Character/${id}`);
  return data;
}

export async function createCharacter(
  input: CreateCharacterInput,
): Promise<Character> {
  const { data } = await apiClient.post<Character>(
    "/Character",
    toFormData(input),
  );
  return data;
}

export async function updateCharacter(
  id: number,
  input: CharacterInput,
): Promise<Character> {
  const { data } = await apiClient.put<Character>(
    `/Character/${id}`,
    toFormData(input),
  );
  return data;
}

export async function deleteCharacter(id: number): Promise<void> {
  await apiClient.delete(`/Character/${id}`);
}

export async function replaceCharacterSpells(
  characterId: number,
  spellIds: number[],
): Promise<void> {
  await apiClient.put(`/CharacterSpells/${characterId}`, { spellIds });
}
