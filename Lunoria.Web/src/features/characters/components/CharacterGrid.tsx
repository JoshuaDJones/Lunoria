import { CardGrid } from "@/components/ui/CardGrid";
import { CharacterCard } from "@/features/characters/components/CharacterCard";
import type { Character } from "@/features/characters/types";

interface CharacterGridProps {
  characters: Character[];
}

export function CharacterGrid({ characters }: CharacterGridProps) {
  return (
    <CardGrid>
      {characters.map((character) => (
        <CharacterCard key={character.id} character={character} />
      ))}
    </CardGrid>
  );
}
