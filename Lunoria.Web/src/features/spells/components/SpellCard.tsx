import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Spell } from "@/features/spells/types";

interface SpellCardProps {
  spell: Spell;
  onSelect?: (spell: Spell) => void;
}

export function SpellCard({ spell, onSelect }: SpellCardProps) {
  return (
    <MediaCard
      title={spell.name}
      description={spell.description}
      imageUrl={spell.photoUrl}
      onClick={onSelect ? () => onSelect(spell) : undefined}
    >
      <StatGrid className="mt-4 px-4 pb-4">
        <Stat label="Spell type" value={spell.spellType?.name ?? "Unknown"} />
        <Stat label="MP cost" value={spell.mpCost} />
        <Stat label="Range" value={spell.range} />
        <Stat label="Area effect" value={spell.isRadius ? "Yes" : "No"} />
        <Stat label="Damage effect" value={spell.damageEffect ?? "None"} />
        <Stat label="Health effect" value={spell.healthEffect ?? "None"} />
        <Stat label="Magic effect" value={spell.magicEffect ?? "None"} />
      </StatGrid>
    </MediaCard>
  );
}
