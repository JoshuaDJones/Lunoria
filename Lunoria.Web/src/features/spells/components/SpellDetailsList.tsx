import { Card } from "@/components/ui";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Spell } from "@/features/spells/types";

interface SpellDetailsListProps {
  spells: Spell[];
  emptyMessage: string;
}

export function SpellDetailsList({
  spells,
  emptyMessage,
}: SpellDetailsListProps) {
  if (spells.length === 0) {
    return <p className="text-content-muted">{emptyMessage}</p>;
  }

  return (
    <div className="space-y-4">
      {spells.map((spell) => (
        <Card key={spell.id} className="overflow-hidden">
          {(spell.photoUrl || spell.spellType?.photoUrl) && (
            <img
              src={spell.photoUrl || spell.spellType?.photoUrl}
              alt=""
              className="max-h-64 w-full bg-surface object-contain"
            />
          )}
          <div className="p-4">
            <h3 className="text-xl font-semibold text-content">{spell.name}</h3>
            <p className="mt-1 text-sm text-content-secondary">
              {spell.description}
            </p>
            <StatGrid className="mt-4">
              <Stat
                label="Spell type"
                value={spell.spellType?.name ?? "Unknown"}
              />
              <Stat label="MP cost" value={spell.mpCost} />
              <Stat label="Range" value={spell.range} />
              <Stat label="Area effect" value={spell.isRadius ? "Yes" : "No"} />
              <Stat label="Damage effect" value={spell.damageEffect ?? "None"} />
              <Stat label="Health effect" value={spell.healthEffect ?? "None"} />
              <Stat label="Magic effect" value={spell.magicEffect ?? "None"} />
            </StatGrid>
          </div>
        </Card>
      ))}
    </div>
  );
}
