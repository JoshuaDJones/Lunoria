import { apiClient } from "@/lib/apiClient";
import type { JourneyPlaythrough } from "@/features/playthroughs/types";

export interface ListPlaythroughsParams {
  skip?: number;
  take?: number;
}

function playthroughUrl(journeyId: number): string {
  return `/journeys/${journeyId}/playthroughs`;
}

export async function startPlaythrough(
  journeyId: number,
): Promise<JourneyPlaythrough> {
  const { data } = await apiClient.post<JourneyPlaythrough>(
    playthroughUrl(journeyId),
  );
  return data;
}

export async function getActivePlaythrough(
  journeyId: number,
): Promise<JourneyPlaythrough> {
  const { data } = await apiClient.get<JourneyPlaythrough>(
    `${playthroughUrl(journeyId)}/active`,
  );
  return data;
}

export async function listPreviousPlaythroughs(
  journeyId: number,
  params: ListPlaythroughsParams = {},
): Promise<JourneyPlaythrough[]> {
  const { data } = await apiClient.get<JourneyPlaythrough[]>(
    playthroughUrl(journeyId),
    { params },
  );
  return data;
}

export async function completePlaythrough(
  journeyId: number,
  playthroughId: number,
): Promise<JourneyPlaythrough> {
  const { data } = await apiClient.post<JourneyPlaythrough>(
    `${playthroughUrl(journeyId)}/${playthroughId}/complete`,
  );
  return data;
}

export async function deactivatePlaythrough(
  journeyId: number,
  playthroughId: number,
): Promise<JourneyPlaythrough> {
  const { data } = await apiClient.post<JourneyPlaythrough>(
    `${playthroughUrl(journeyId)}/${playthroughId}/deactivate`,
  );
  return data;
}
