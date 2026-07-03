import { apiClient, unwrapApiResult, type ApiResult } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";
import type {
  CreateSceneInput,
  Scene,
  SceneDashboard,
  SceneDialog,
  SceneInput,
  SceneParticipant,
  SceneParticipantTurn,
  SceneProgress,
  SceneProgressStatus,
} from "@/features/scenes/types";

export interface ListScenesParams {
  journeyId: number;
  skip?: number;
  take?: number;
}

export interface AddSceneParticipantRequest {
  journeyCharacterId?: number | null;
  sceneCharacterId?: number | null;
}

export interface SceneParticipantTurnPosition {
  turnId: number;
  turnPosition: number;
}

export interface CreateDialogPageSectionRequest {
  characterId?: number | null;
  orderNum: number;
  readingText: string;
  isNarrator: boolean;
}

export interface UpdateDialogPageSectionRequest {
  characterId?: number | null;
  orderNum?: number | null;
  readingText?: string | null;
  isNarrator?: boolean | null;
}

export async function listScenes(params: ListScenesParams): Promise<Scene[]> {
  const { data } = await apiClient.get<Scene[]>("/Scene", { params });
  return data;
}

export async function getScene(id: number): Promise<Scene> {
  const { data } = await apiClient.get<Scene>(`/Scene/${id}`);
  return data;
}

export async function getLegacySceneDashboard(
  sceneId: number,
  journeyId: number,
): Promise<SceneDashboard> {
  const { data } = await apiClient.get<SceneDashboard>(
    `/Scene/${sceneId}/dashboard`,
    { params: { journeyId } },
  );
  return data;
}

export async function createScene(input: CreateSceneInput): Promise<Scene> {
  const { data } = await apiClient.post<Scene>("/Scene", toFormData(input));
  return data;
}

export async function updateScene(
  id: number,
  input: SceneInput,
): Promise<Scene> {
  const { data } = await apiClient.put<Scene>(
    `/Scene/${id}`,
    toFormData(input),
  );
  return data;
}

export async function deleteScene(
  id: number,
  journeyId: number,
): Promise<void> {
  await apiClient.delete(`/Scene/${id}`, { params: { journeyId } });
}

export async function addSceneCharacter(
  sceneId: number,
  characterId: number,
): Promise<void> {
  await apiClient.post("/SceneCharacter", { sceneId, characterId });
}

export async function updateSceneCharacter(
  sceneCharacterId: number,
  hp: number,
  mp: number,
): Promise<void> {
  await apiClient.patch(`/SceneCharacter/${sceneCharacterId}`, { hp, mp });
}

export async function deleteSceneCharacter(
  sceneCharacterId: number,
): Promise<void> {
  await apiClient.delete(`/SceneCharacter/${sceneCharacterId}`);
}

export async function addSceneCharacterItem(
  sceneCharacterId: number,
  itemId: number,
): Promise<void> {
  await apiClient.post("/SceneCharacterItem", { sceneCharacterId, itemId });
}

export async function useSceneCharacterItem(
  sceneCharacterItemId: number,
): Promise<void> {
  await apiClient.patch("/SceneCharacterItem", { sceneCharacterItemId });
}

export async function listSceneDialogs(sceneId: number): Promise<SceneDialog[]> {
  const { data } = await apiClient.get<ApiResult<SceneDialog[]>>(
    `/SceneDialog/${sceneId}`,
  );
  return unwrapApiResult(data);
}

export async function createSceneDialog(
  sceneId: number,
  title: string,
): Promise<void> {
  await apiClient.post(`/SceneDialog/${sceneId}`, { title });
}

export async function updateSceneDialog(
  sceneDialogId: number,
  title: string,
): Promise<void> {
  await apiClient.patch(`/SceneDialog/${sceneDialogId}`, { title });
}

export async function deleteSceneDialog(sceneDialogId: number): Promise<void> {
  await apiClient.delete(`/SceneDialog/${sceneDialogId}`);
}

export async function createDialogPage(
  sceneDialogId: number,
  orderNum: number,
  photo: File,
): Promise<void> {
  await apiClient.post(
    `/DialogPage/${sceneDialogId}`,
    toFormData({ orderNum, photo }),
  );
}

export async function updateDialogPage(
  dialogPageId: number,
  input: { orderNum?: number | null; photo?: File },
): Promise<void> {
  await apiClient.put(`/DialogPage/${dialogPageId}`, toFormData(input));
}

export async function deleteDialogPage(dialogPageId: number): Promise<void> {
  await apiClient.delete(`/DialogPage/${dialogPageId}`);
}

export async function createDialogPageSection(
  dialogPageId: number,
  request: CreateDialogPageSectionRequest,
): Promise<void> {
  await apiClient.post(`/DialogPageSection/${dialogPageId}`, request);
}

export async function updateDialogPageSection(
  dialogPageSectionId: number,
  request: UpdateDialogPageSectionRequest,
): Promise<void> {
  await apiClient.patch(
    `/DialogPageSection/${dialogPageSectionId}`,
    request,
  );
}

export async function deleteDialogPageSection(
  dialogPageSectionId: number,
): Promise<void> {
  await apiClient.delete(`/DialogPageSection/${dialogPageSectionId}`);
}

export async function createOrGetSceneProgress(
  journeyId: number,
  sceneId: number,
): Promise<SceneProgress> {
  const { data } = await apiClient.post<SceneProgress>(
    `/journeys/${journeyId}/scenes/${sceneId}/progress`,
  );
  return data;
}

export async function getActiveSceneProgress(
  journeyId: number,
  sceneId: number,
): Promise<SceneProgress> {
  const { data } = await apiClient.get<SceneProgress>(
    `/journeys/${journeyId}/scenes/${sceneId}/progress`,
  );
  return data;
}

export async function listSceneProgress(
  journeyId: number,
  playthroughId: number,
): Promise<SceneProgress[]> {
  const { data } = await apiClient.get<SceneProgress[]>(
    `/journeys/${journeyId}/playthroughs/${playthroughId}/scene-progress`,
  );
  return data;
}

export async function updateSceneProgressStatus(
  sceneProgressId: number,
  status: SceneProgressStatus,
): Promise<SceneProgress> {
  const { data } = await apiClient.patch<SceneProgress>(
    `/scene-progress/${sceneProgressId}/status`,
    { status },
  );
  return data;
}

export async function addSceneParticipant(
  sceneProgressId: number,
  request: AddSceneParticipantRequest,
): Promise<SceneParticipant> {
  const { data } = await apiClient.post<SceneParticipant>(
    `/scene-progress/${sceneProgressId}/participants`,
    request,
  );
  return data;
}

export async function removeSceneParticipant(
  sceneProgressId: number,
  participantId: number,
): Promise<void> {
  await apiClient.delete(
    `/scene-progress/${sceneProgressId}/participants/${participantId}`,
  );
}

export async function addSceneParticipantTurn(
  sceneProgressId: number,
  sceneParticipantId: number,
  turnPosition: number,
): Promise<SceneParticipantTurn> {
  const { data } = await apiClient.post<SceneParticipantTurn>(
    `/scene-progress/${sceneProgressId}/turns`,
    { sceneParticipantId, turnPosition },
  );
  return data;
}

export async function reorderSceneParticipantTurns(
  sceneProgressId: number,
  turns: SceneParticipantTurnPosition[],
): Promise<SceneParticipantTurn[]> {
  const { data } = await apiClient.put<SceneParticipantTurn[]>(
    `/scene-progress/${sceneProgressId}/turns/reorder`,
    { turns },
  );
  return data;
}

export async function removeSceneParticipantTurn(
  sceneProgressId: number,
  turnId: number,
): Promise<void> {
  await apiClient.delete(
    `/scene-progress/${sceneProgressId}/turns/${turnId}`,
  );
}
