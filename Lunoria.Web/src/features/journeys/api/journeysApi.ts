import { apiClient } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";
import type {
  CreateJourneyInput,
  Journey,
  JourneyCharacterSpell,
  JourneyInput,
} from "@/features/journeys/types";

export interface ListJourneysParams {
  skip?: number;
  take?: number;
}

export interface UpdateJourneyCharacterRequest {
  hp: number;
  mp: number;
  isAlternateForm: boolean;
}

export async function listJourneys(
  params: ListJourneysParams = {},
): Promise<Journey[]> {
  const { data } = await apiClient.get<Journey[]>("/Journey", { params });
  return data;
}

export async function getJourney(id: number): Promise<Journey> {
  const { data } = await apiClient.get<Journey>(`/Journey/${id}`);
  return data;
}

export async function createJourney(
  input: CreateJourneyInput,
): Promise<Journey> {
  const { data } = await apiClient.post<Journey>(
    "/Journey",
    toFormData(input),
  );
  return data;
}

export async function updateJourney(
  id: number,
  input: JourneyInput,
): Promise<Journey> {
  const { data } = await apiClient.put<Journey>(
    `/Journey/${id}`,
    toFormData(input),
  );
  return data;
}

export async function deleteJourney(id: number): Promise<void> {
  await apiClient.delete(`/Journey/${id}`);
}

export async function replaceJourneyCharacters(
  journeyId: number,
  characterIds: number[],
): Promise<void> {
  await apiClient.put(`/JourneyCharacter/${journeyId}`, { characterIds });
}

export async function updateJourneyCharacter(
  journeyCharacterId: number,
  request: UpdateJourneyCharacterRequest,
): Promise<void> {
  await apiClient.patch(`/JourneyCharacter/${journeyCharacterId}`, request);
}

export async function deleteJourneyCharacter(
  journeyCharacterId: number,
): Promise<void> {
  await apiClient.delete(`/JourneyCharacter/${journeyCharacterId}`);
}

export async function addJourneyCharacterItem(
  journeyCharacterId: number,
  itemId: number,
): Promise<void> {
  await apiClient.post("/JourneyCharacterItem", { journeyCharacterId, itemId });
}

export async function useJourneyCharacterItem(
  journeyCharacterItemId: number,
): Promise<void> {
  await apiClient.patch("/JourneyCharacterItem", { journeyCharacterItemId });
}

export async function grantJourneyCharacterSpell(
  journeyCharacterId: number,
  spellId: number,
): Promise<JourneyCharacterSpell> {
  const { data } = await apiClient.post<JourneyCharacterSpell>(
    `/journey-characters/${journeyCharacterId}/spells`,
    { spellId },
  );
  return data;
}

export async function removeJourneyCharacterSpell(
  journeyCharacterId: number,
  spellId: number,
): Promise<void> {
  await apiClient.delete(
    `/journey-characters/${journeyCharacterId}/spells/${spellId}`,
  );
}
