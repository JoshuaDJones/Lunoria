import { apiClient } from "@/lib/apiClient";
import type {
  PlaythroughJoinSession,
  PublicPlaythroughSnapshot,
} from "./types";

export async function createPlaythroughJoinSession(
  playthroughId: number,
): Promise<PlaythroughJoinSession> {
  const { data } = await apiClient.post<PlaythroughJoinSession>(
    `/playthroughs/${playthroughId}/join-session`,
  );
  return data;
}

export async function revokePlaythroughJoinSession(
  playthroughId: number,
): Promise<void> {
  await apiClient.delete(`/playthroughs/${playthroughId}/join-session`);
}

export async function getPublicPlaythroughSnapshot(
  token: string,
): Promise<PublicPlaythroughSnapshot> {
  const { data } = await apiClient.get<PublicPlaythroughSnapshot>(
    `/playthrough-sessions/${encodeURIComponent(token)}`,
  );
  return data;
}
