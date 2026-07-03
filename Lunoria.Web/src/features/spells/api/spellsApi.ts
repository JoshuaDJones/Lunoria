import { apiClient } from "@/lib/apiClient";
import { toFormData } from "@/lib/formData";
import type {
  CreateSpellInput,
  CreateSpellTypeInput,
  Spell,
  SpellInput,
  SpellType,
  SpellTypeInput,
} from "@/features/spells/types";

export interface ListCatalogParams {
  skip?: number;
  take?: number;
}

export async function listSpells(
  params: ListCatalogParams = {},
): Promise<Spell[]> {
  const { data } = await apiClient.get<Spell[]>("/Spell", { params });
  return data;
}

export async function getSpell(id: number): Promise<Spell> {
  const { data } = await apiClient.get<Spell>(`/Spell/${id}`);
  return data;
}

export async function createSpell(input: CreateSpellInput): Promise<Spell> {
  const { data } = await apiClient.post<Spell>("/Spell", toFormData(input));
  return data;
}

export async function updateSpell(
  id: number,
  input: SpellInput,
): Promise<Spell> {
  const { data } = await apiClient.put<Spell>(
    `/Spell/${id}`,
    toFormData(input),
  );
  return data;
}

export async function deleteSpell(id: number): Promise<void> {
  await apiClient.delete(`/Spell/${id}`);
}

export async function listSpellTypes(
  params: ListCatalogParams = {},
): Promise<SpellType[]> {
  const { data } = await apiClient.get<SpellType[]>("/SpellType", { params });
  return data;
}

export async function getSpellType(id: number): Promise<SpellType> {
  const { data } = await apiClient.get<SpellType>(`/SpellType/${id}`);
  return data;
}

export async function createSpellType(
  input: CreateSpellTypeInput,
): Promise<SpellType> {
  const { data } = await apiClient.post<SpellType>(
    "/SpellType",
    toFormData(input),
  );
  return data;
}

export async function updateSpellType(
  id: number,
  input: SpellTypeInput,
): Promise<SpellType> {
  const { data } = await apiClient.put<SpellType>(
    `/SpellType/${id}`,
    toFormData(input),
  );
  return data;
}

export async function deleteSpellType(id: number): Promise<void> {
  await apiClient.delete(`/SpellType/${id}`);
}
