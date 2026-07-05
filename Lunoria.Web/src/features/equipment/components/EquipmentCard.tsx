import { MediaCard } from "@/components/ui/MediaCard";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { EquippableItem } from "@/features/equipment/types";

interface EquipmentCardProps {
  item: EquippableItem;
  onSelect?: (item: EquippableItem) => void;
}

function signed(value: number): string {
  return value > 0 ? `+${value}` : value.toString();
}

export function EquipmentCard({ item, onSelect }: EquipmentCardProps) {
  return (
    <MediaCard
      title={item.name}
      description={item.description}
      imageUrl={item.photoUrl}
      onClick={onSelect ? () => onSelect(item) : undefined}
    >
      <StatGrid className="mt-4 px-4">
        <Stat
          label="Melee damage modifier"
          value={signed(item.meleeAttackDamageModifier)}
        />
        <Stat
          label="Bow damage modifier"
          value={signed(item.bowAttackDamageModifier)}
        />
        <Stat label="Movement modifier" value={signed(item.movementModifier)} />
        <Stat label="Max HP modifier" value={signed(item.maxHpModifier)} />
        <Stat label="Max MP modifier" value={signed(item.maxMpModifier)} />
        <Stat
          label="Consumable capacity"
          value={signed(item.maxConsumableInventoryModifier)}
        />
        <Stat
          label="Equipment capacity"
          value={signed(item.maxEquippableInventoryModifier)}
        />
        <Stat
          label="Melee damage reduction"
          value={item.meleeDamageReduction}
        />
        <Stat label="Bow damage reduction" value={item.bowDamageReduction} />
        <Stat
          label="Spell damage reduction"
          value={item.spellDamageReduction}
        />
        <Stat
          label="Affected spell type"
          value={item.affectedSpellType?.name ?? "All"}
        />
        <Stat
          label="Spell damage modifier"
          value={signed(item.spellDamageModifier)}
        />
      </StatGrid>

      <div className="mt-4 border-t border-border px-4 pt-4 pb-4">
        <h3 className="text-sm font-semibold text-content-secondary">
          Added spells ({item.addedSpells.length})
        </h3>
        {item.addedSpells.length > 0 ? (
          <ul className="mt-2 flex flex-wrap gap-2">
            {item.addedSpells.map((spell) => (
              <li
                key={spell.id}
                className="rounded-md bg-surface-raised px-2.5 py-1 text-xs text-content"
              >
                {spell.name}
              </li>
            ))}
          </ul>
        ) : (
          <p className="mt-2 text-sm text-content-muted">No added spells</p>
        )}
      </div>
    </MediaCard>
  );
}
