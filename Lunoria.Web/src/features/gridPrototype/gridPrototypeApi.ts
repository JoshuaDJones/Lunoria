import { apiClient } from "@/lib/apiClient";
import type { GridPrototypeCharacter } from "@/features/gridPrototype/types";

export async function listGridPrototypeCharacters(): Promise<
  GridPrototypeCharacter[]
> {
  const { data } = await apiClient.get<GridPrototypeCharacter[]>(
    "/grid-prototype/characters",
  );
  return data;
}
