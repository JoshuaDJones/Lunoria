import { apiClient } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";
import type { CreateItemInput, Item, ItemInput } from "@/features/items/types";

export interface ListItemsParams {
  skip?: number;
  take?: number;
}

export async function listItems(params: ListItemsParams = {}): Promise<Item[]> {
  const { data } = await apiClient.get<Item[]>("/ConsumableItem", { params });
  return data;
}

export async function getItem(id: number): Promise<Item> {
  const { data } = await apiClient.get<Item>(`/ConsumableItem/${id}`);
  return data;
}

export async function createItem(input: CreateItemInput): Promise<Item> {
  const { data } = await apiClient.post<Item>(
    "/ConsumableItem",
    toFormData(input),
  );
  return data;
}

export async function updateItem(id: number, input: ItemInput): Promise<Item> {
  const { data } = await apiClient.put<Item>(
    `/ConsumableItem/${id}`,
    toFormData(input),
  );
  return data;
}

export async function deleteItem(id: number): Promise<void> {
  await apiClient.delete(`/ConsumableItem/${id}`);
}
