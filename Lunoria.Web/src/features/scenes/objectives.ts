import { apiClient } from "@/lib/apiClient";

export interface SceneObjective {
  id: number;
  sortOrder: number;
  points: string[];
}

export async function getSceneObjectives(sceneId: number) {
  const { data } = await apiClient.get<SceneObjective[]>(
    `/scenes/${sceneId}/objectives`,
  );
  return data;
}

export async function saveSceneObjectives(
  sceneId: number,
  objectives: { points: string[] }[],
) {
  await apiClient.put(`/scenes/${sceneId}/objectives`, { objectives });
}

export async function selectSceneObjective(
  playthroughId: number,
  sceneId: number,
  index: number,
) {
  await apiClient.put(
    `/playthroughs/${playthroughId}/scenes/${sceneId}/objectives/current`,
    { index },
  );
}
