import { CardGrid } from "@/components/ui/CardGrid";
import { SpellCard } from "@/features/spells/components/SpellCard";
import type { Spell } from "@/features/spells/types";

interface SpellGridProps {
  spells: Spell[];
  onSelect?: (spell: Spell) => void;
  onDelete?: (spell: Spell) => void;
}

export function SpellGrid({ spells, onSelect, onDelete }: SpellGridProps) {
  return (
    <CardGrid>
      {spells.map((spell) => (
        <SpellCard
          key={spell.id}
          spell={spell}
          onSelect={onSelect}
          onDelete={onDelete}
        />
      ))}
    </CardGrid>
  );
}
