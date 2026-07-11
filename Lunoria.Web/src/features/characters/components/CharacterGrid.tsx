import { CardGrid } from "@/components/ui/CardGrid";
import { CharacterCard } from "@/features/characters/components/CharacterCard";
import type { Character } from "@/features/characters/types";

interface CharacterGridProps {
  characters: Character[];
  onSelect?: (character: Character) => void;
  onDelete?: (character: Character) => void;
  onSpell?: (character: Character) => void;
  onViewSpells?: (character: Character) => void;
}

export function CharacterGrid({
  characters,
  onSelect,
  onDelete,
  onSpell,
  onViewSpells,
}: CharacterGridProps) {
  return (
    <CardGrid>
      {characters.map((character) => (
        <CharacterCard
          key={character.id}
          character={character}
          onSelect={onSelect}
          onDelete={onDelete}
          onSpell={onSpell}
          onViewSpells={onViewSpells}
        />
      ))}
    </CardGrid>
  );
}
