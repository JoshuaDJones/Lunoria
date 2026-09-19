import { apiClient, ApiResult, unwrapApiResult } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";
import type {
  CreateJourneyInput,
  Journey,
  JourneyCharacterSpell,
  JourneyCharacter,
  JourneyInput,
  PlaythroughCreated,
  PlaythroughDetails,
  PlaythroughSummary,
  SceneAttackResult,
  SceneAttackType,
  SceneOpenChestResult,
  ScenePlaythroughDetails,
  SceneUseConsumableResult,
  SceneStartResult,
  SceneInventoryResolutionInput,
} from "@/features/journeys/types";
import { SceneDialog } from "@/features/scenes/types";

export interface ListJourneysParams {
  skip?: number;
  take?: number;
}

export interface UpdateJourneyCharacterRequest {
  sortOrder: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
  movement: number;
  maxConsumableInventory: number;
  maxEquippableInventory: number;
  maxHp: number;
  maxMp: number;
  isInitiallyActive: boolean;
  alternateFormId: number | null;
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

export async function listJourneyPlaythroughs(
  journeyId: number,
): Promise<PlaythroughSummary[]> {
  const { data } = await apiClient.get<PlaythroughSummary[]>(
    `/journeys/${journeyId}/playthroughs`,
  );
  return data;
}

export async function startJourneyPlaythrough(
  journeyId: number,
): Promise<PlaythroughCreated> {
  const { data } = await apiClient.post<PlaythroughCreated>(
    `/journeys/${journeyId}/playthroughs`,
  );
  return data;
}

export async function getPlaythrough(
  playthroughId: number,
): Promise<PlaythroughDetails> {
  const { data } = await apiClient.get<PlaythroughDetails>(
    `/playthroughs/${playthroughId}`,
  );
  return data;
}

export async function startScenePlaythrough(
  playthroughId: number,
  sceneId: number,
): Promise<SceneStartResult> {
  const { data } = await apiClient.post<SceneStartResult>(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/start`,
  );
  return data;
}

export async function getSceneStartInventory(
  playthroughId: number,
  sceneId: number,
): Promise<SceneStartResult> {
  const { data } = await apiClient.get<SceneStartResult>(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/start-inventory`,
  );
  return data;
}

export async function resolveSceneStartInventory(
  playthroughId: number,
  sceneId: number,
  input: SceneInventoryResolutionInput,
): Promise<SceneStartResult> {
  const { data } = await apiClient.post<SceneStartResult>(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/start-inventory/resolve`,
    input,
  );
  return data;
}

export async function endScenePlaythrough(
  playthroughId: number,
  sceneId: number,
): Promise<void> {
  await apiClient.post(`/playthroughs/${playthroughId}/scenes/${sceneId}/end`);
}

export async function getScenePlaythrough(
  playthroughId: number,
  sceneId: number,
): Promise<ScenePlaythroughDetails> {
  const { data } = await apiClient.get<ScenePlaythroughDetails>(
    `/playthroughs/${playthroughId}/scenes/${sceneId}`,
  );
  return data;
}

export async function addSceneCharacterInstance(
  playthroughId: number,
  sceneId: number,
  scenePlaythroughCharacterId: number,
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/scene-characters/${scenePlaythroughCharacterId}`,
  );
}

export async function activateSceneJourneyCharacter(
  playthroughId: number,
  sceneId: number,
  journeyPlaythroughCharacterId: number,
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/journey-characters/${journeyPlaythroughCharacterId}/activate`,
  );
}

export async function addPlaythroughCharacterToScene(
  playthroughId: number,
  sceneId: number,
  playthroughCharacterId: number,
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/playthrough-characters/${playthroughCharacterId}`,
  );
}

export async function removeSceneParticipant(
  playthroughId: number,
  sceneId: number,
  participantId: number,
): Promise<void> {
  await apiClient.delete(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}`,
  );
}

export interface AddSceneChestLootEntryInput {
  rollMinimum: number;
  rollMaximum: number;
  quantity: number;
  playthroughEquippableItemId: number | null;
  playthroughConsumableItemId: number | null;
}

export interface AddSceneChestInput {
  name: string;
  dieSides: number;
  lootEntries: AddSceneChestLootEntryInput[];
}

export async function addChestToScene(
  playthroughId: number,
  sceneId: number,
  input: AddSceneChestInput,
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/chests`,
    input,
  );
}

export interface UpdateSceneParticipantStatsInput {
  currentHp: number;
  maxHp: number;
  currentMp: number;
  maxMp: number;
  movement: number;
  meleeAttackDamage: number | null;
  bowAttackDamage: number | null;
}

export async function updateSceneParticipantStats(
  playthroughId: number,
  sceneId: number,
  participantId: number,
  input: UpdateSceneParticipantStatsInput,
): Promise<void> {
  await apiClient.put(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/stats`,
    input,
  );
}

export interface SceneMovementResult {
  movement: number;
}

export async function recordSceneParticipantMovement(
  playthroughId: number,
  sceneId: number,
  participantId: number,
  roll: number,
): Promise<SceneMovementResult> {
  const { data } = await apiClient.post<SceneMovementResult>(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/movement`,
    { roll },
  );
  return data;
}

export async function transformSceneParticipant(
  playthroughId: number,
  sceneId: number,
  participantId: number,
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/transform`,
  );
}

export async function forfeitSceneParticipantAction(
  playthroughId: number,
  sceneId: number,
  participantId: number,
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/forfeit-action`,
  );
}

export async function replenishAtSceneCampfire(
  playthroughId: number,
  sceneId: number,
  participantId: number,
  resource: "hp" | "mp",
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/campfire`,
    { resource },
  );
}

export interface ResolveSceneAttackInput {
  isCounterattack?: boolean;
  counterattackToken?: string;
  targetParticipantId: number | null;
  attackType: SceneAttackType;
  roll: number;
  playthroughSpellId: number | null;
}

export async function passSceneCounterattack(
  playthroughId: number,
  sceneId: number,
  token: string,
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/counterattack/pass`,
    JSON.stringify(token),
    {
      headers: { "Content-Type": "application/json" },
    },
  );
}

export async function resolveSceneParticipantAttack(
  playthroughId: number,
  sceneId: number,
  participantId: number,
  input: ResolveSceneAttackInput,
): Promise<SceneAttackResult> {
  const { data } = await apiClient.post<SceneAttackResult>(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/attack`,
    input,
  );
  return data;
}

export async function openSceneParticipantChest(
  playthroughId: number,
  sceneId: number,
  participantId: number,
  chestId: number,
  roll: number,
): Promise<SceneOpenChestResult> {
  const { data } = await apiClient.post<SceneOpenChestResult>(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/chests/${chestId}/open`,
    { roll },
  );
  return data;
}

export async function useSceneParticipantConsumable(
  playthroughId: number,
  sceneId: number,
  participantId: number,
  inventoryItemId: number,
): Promise<SceneUseConsumableResult> {
  const { data } = await apiClient.post<SceneUseConsumableResult>(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/consumables/${inventoryItemId}/use`,
  );
  return data;
}

export interface TradeSceneParticipantItemInput {
  targetParticipantId: number;
  inventoryItemId: number;
  isEquippable: boolean;
}

export async function tradeSceneParticipantItem(
  playthroughId: number,
  sceneId: number,
  participantId: number,
  input: TradeSceneParticipantItemInput,
): Promise<void> {
  await apiClient.post(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/participants/${participantId}/trade`,
    input,
  );
}

export async function createJourney(
  input: CreateJourneyInput,
): Promise<Journey> {
  const { data } = await apiClient.post<Journey>("/Journey", toFormData(input));
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
): Promise<JourneyCharacter> {
  const { data } = await apiClient.put<JourneyCharacter>(
    `/JourneyCharacter/assignment/${journeyCharacterId}`,
    request,
  );
  return data;
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

export async function tempGetAllDialogs(
  journeyId: number,
): Promise<SceneDialog[]> {
  const { data } = await apiClient.get<ApiResult<SceneDialog[]>>(
    `/SceneDialog/give-me-all/${journeyId}`,
  );
  return unwrapApiResult(data);
}
