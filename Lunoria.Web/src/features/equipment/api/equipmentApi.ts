import { apiClient } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";
import type {
  CreateEquippableItemInput,
  EquippableItem,
  EquippableItemInput,
} from "@/features/equipment/types";
import type { JourneyCharacterEquippableItem } from "@/features/journeys/types";

export interface ListEquipmentParams {
  skip?: number;
  take?: number;
}

export async function listEquipment(
  params: ListEquipmentParams = {},
): Promise<EquippableItem[]> {
  const { data } = await apiClient.get<EquippableItem[]>("/EquippableItem", {
    params,
  });
  return data;
}

export async function getEquipment(id: number): Promise<EquippableItem> {
  const { data } = await apiClient.get<EquippableItem>(
    `/EquippableItem/${id}`,
  );
  return data;
}

export async function createEquipment(
  input: CreateEquippableItemInput,
): Promise<EquippableItem> {
  const { data } = await apiClient.post<EquippableItem>(
    "/EquippableItem",
    toFormData(input),
  );
  return data;
}

export async function updateEquipment(
  id: number,
  input: EquippableItemInput,
): Promise<EquippableItem> {
  const { data } = await apiClient.put<EquippableItem>(
    `/EquippableItem/${id}`,
    toFormData(input),
  );
  return data;
}

export async function deleteEquipment(id: number): Promise<void> {
  await apiClient.delete(`/EquippableItem/${id}`);
}

export async function addJourneyCharacterEquipment(
  journeyCharacterId: number,
  equippableItemId: number,
): Promise<JourneyCharacterEquippableItem> {
  const { data } = await apiClient.post<JourneyCharacterEquippableItem>(
    "/JourneyCharacterEquipment",
    { journeyCharacterId, equippableItemId },
  );
  return data;
}

export async function setJourneyCharacterEquipmentState(
  assignmentId: number,
  isEquipped: boolean,
): Promise<JourneyCharacterEquippableItem> {
  const { data } = await apiClient.patch<JourneyCharacterEquippableItem>(
    `/JourneyCharacterEquipment/${assignmentId}`,
    { isEquipped },
  );
  return data;
}

export async function removeJourneyCharacterEquipment(
  assignmentId: number,
): Promise<void> {
  await apiClient.delete(`/JourneyCharacterEquipment/${assignmentId}`);
}
