import { apiClient } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";
import type { IntroPage, IntroPageType } from "@/features/journeys/types";

export interface IntroPageInput {
  journeyId: number;
  type: IntroPageType;
  config: string;
  image?: File;
}

export async function listIntroPages(journeyId: number): Promise<IntroPage[]> {
  const { data } = await apiClient.get<IntroPage[]>("/JourneyIntroPage", {
    params: { journeyId },
  });
  return data;
}

export async function createIntroPage(
  input: IntroPageInput & { image: File },
): Promise<IntroPage> {
  const { data } = await apiClient.post<IntroPage>(
    "/JourneyIntroPage",
    toFormData(input),
  );
  return data;
}

export async function updateIntroPage(
  id: number,
  input: IntroPageInput,
): Promise<IntroPage> {
  const { data } = await apiClient.put<IntroPage>(
    `/JourneyIntroPage/${id}`,
    toFormData(input),
  );
  return data;
}

export async function deleteIntroPage(
  id: number,
  journeyId: number,
): Promise<void> {
  await apiClient.delete(`/JourneyIntroPage/${id}`, {
    params: { journeyId },
  });
}

export async function reorderIntroPages(
  journeyId: number,
  pages: Array<{ id: number; sortOrder: number }>,
): Promise<void> {
  await apiClient.put(
    "/JourneyIntroPage/order",
    { pages },
    { params: { journeyId } },
  );
}
