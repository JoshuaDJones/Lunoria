import { useCallback, useState } from "react";
import { CollectionPage } from "@/components/layout/CollectionPage";
import {
  CharacterGrid,
  CharacterType,
  listCharacters,
} from "@/features/characters";

export function CharactersPage() {
  const [typeFilter, setTypeFilter] = useState(CharacterType.Any);

  const loadCharacters = useCallback(
    () => listCharacters({ typeFilter }),
    [typeFilter],
  );

  return (
    <CollectionPage
      key={typeFilter}
      title="Characters"
      itemName="character"
      loadItems={loadCharacters}
      toolbar={
        <div className="flex items-center gap-3">
          <label
            htmlFor="character-type-filter"
            className="text-sm font-semibold text-content-secondary"
          >
            Character type
          </label>
          <select
            id="character-type-filter"
            value={typeFilter}
            onChange={(event) =>
              setTypeFilter(Number(event.target.value) as CharacterType)
            }
            className="rounded-lg border border-border bg-surface-raised px-3 py-2 text-content outline-none transition focus:border-brand-hover focus:ring-2 focus:ring-brand-hover/20"
          >
            <option value={CharacterType.Any}>All characters</option>
            <option value={CharacterType.Player}>Playable characters</option>
            <option value={CharacterType.NPC}>NPCs</option>
            <option value={CharacterType.Enemy}>Enemies</option>
          </select>
        </div>
      }
      renderItems={(characters) => <CharacterGrid characters={characters} />}
    />
  );
}
