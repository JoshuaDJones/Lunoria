import { apiClient } from "@/lib/apiClient";
import type {
  CharacterSyncKind,
  CharacterSyncPreview,
  CharacterSyncRequest,
} from "@/features/characterSync/types";

const path = (kind: CharacterSyncKind, id: number) =>
  `/${kind === "journey" ? "JourneyCharacter" : "SceneCharacter"}/${id}`;

export async function getCharacterSyncPreview(
  kind: CharacterSyncKind,
  id: number,
  signal?: AbortSignal,
) {
  const { data } = await apiClient.get<CharacterSyncPreview>(
    `${path(kind, id)}/sync-preview`,
    { signal },
  );
  return data;
}

export async function applyCharacterSync(
  kind: CharacterSyncKind,
  id: number,
  request: CharacterSyncRequest,
  acknowledgeOnly: boolean,
) {
  const action = acknowledgeOnly ? "acknowledge-base-changes" : "sync";
  const { data } = await apiClient.post<CharacterSyncPreview>(
    `${path(kind, id)}/${action}`,
    request,
  );
  return data;
}
