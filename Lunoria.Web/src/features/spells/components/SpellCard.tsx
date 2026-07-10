import { Button } from "@/components/ui";
import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Spell } from "@/features/spells/types";
import { faPen, faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

interface SpellCardProps {
  spell: Spell;
  onSelect?: (spell: Spell) => void;
  onDelete?: (spell: Spell) => void;
}

export function SpellCard({ spell, onSelect, onDelete }: SpellCardProps) {
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
      <div className="mt-auto border-t border-border px-4 py-3 flex gap-2 justify-end">
        <Button
          onClick={(event) => {
            event.stopPropagation();
            onDelete?.(spell);
          }}
          variant="danger"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faTrash} />}
        >
          Delete
        </Button>
        <Button
          variant="primary"
          size="md"
          inverted
          leftIcon={<FontAwesomeIcon icon={faPen} />}
        >
          Edit
        </Button>
      </div>
    </MediaCard>
  );
}
