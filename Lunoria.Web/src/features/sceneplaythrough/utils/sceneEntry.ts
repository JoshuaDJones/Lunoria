import { getScenePlaythrough } from "@/features/journeys/api/journeysApi";
import type { ScenePlaythroughDetails } from "@/features/journeys/types";

// A short-lived handoff, not a persistent scene cache. Scene actions still reload the API.
let prepared: { scene: ScenePlaythroughDetails; expiresAt: number } | undefined;

export async function prepareSceneEntry(
  playthroughId: number,
  sceneId: number,
) {
  prepared = undefined;
  const scene = await getScenePlaythrough(playthroughId, sceneId);
  const url = scene.photoUrl?.trim();
  if (url) {
    await new Promise<void>((resolve) => {
      const image = new Image();
      const finish = () => {
        clearTimeout(timeout);
        image.onload = null;
        image.onerror = null;
        resolve();
      };
      const timeout = setTimeout(finish, 5000);
      image.onload = finish;
      image.onerror = finish;
      image.src = url;
      if (image.complete) finish();
    });
  }
  prepared = { scene, expiresAt: Date.now() + 15_000 };
}

export function getPreparedScene(playthroughId: number, sceneId: number) {
  return prepared &&
    prepared.expiresAt > Date.now() &&
    prepared.scene.playthroughId === playthroughId &&
    prepared.scene.id === sceneId
    ? prepared.scene
    : undefined;
}

export function clearPreparedScene(scene: ScenePlaythroughDetails) {
  if (prepared?.scene === scene) prepared = undefined;
}
