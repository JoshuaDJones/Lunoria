import { Button } from "@/components/ui";
import { Card } from "@/components/ui/Card";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import type { Character } from "@/features/characters/types";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faPen,
  faTrash,
  faWandMagicSparkles,
} from "@fortawesome/free-solid-svg-icons";

interface CharacterCardProps {
  character: Character;
  onSelect?: (character: Character) => void;
  onDelete?: (character: Character) => void;
}

export function CharacterCard({
  character,
  onSelect,
  onDelete,
}: CharacterCardProps) {
  const roles = [
    character.isPlayer && "Player",
    character.isNPC && "NPC",
    character.isEnemy && "Enemy",
  ].filter((role): role is string => Boolean(role));

  return (
    <Card
      onKeyDown={(event) => {
        if (onSelect && (event.key === "Enter" || event.key === " ")) {
          event.preventDefault();
          onSelect(character);
        }
      }}
      role={onSelect ? "button" : undefined}
      tabIndex={onSelect ? 0 : undefined}
      className="flex flex-col p-4 transition"
    >
      <div className="flex items-start gap-4">
        <div className="min-w-0 flex-1">
          <h2 className="wrap-break-word text-4xl font-semibold text-content">
            {character.name}
          </h2>
          <p className="line-clamp-4 wrap-break-word text-lg text-content-secondary">
            {character.description}
          </p>
        </div>

        {character.photoUrl && (
          <img
            src={character.photoUrl}
            alt=""
            className="h-auto w-[39%] shrink-0 rounded-lg object-contain"
          />
        )}
      </div>

      <StatGrid className="mt-4">
        <Stat label="Max HP" value={character.maxHp} labelTone="text-health" />
        <Stat label="Max MP" value={character.maxMp} labelTone="text-mana" />
        <Stat
          label="Melee damage"
          value={character.meleeAttackDamage ?? "N/A"}
        />
        <Stat label="Bow damage" value={character.bowAttackDamage ?? "N/A"} />
        <Stat label="Movement" value={character.movement} />
        <Stat
          label="Consumable capacity"
          value={character.baseMaxConsumableInventory}
        />
        <Stat
          label="Equipment capacity"
          value={character.baseMaxEquippableInventory}
        />
        <Stat
          label="Alternate form"
          value={character.alternateForm?.name ?? "None"}
        />
      </StatGrid>

      <div className="mt-4">
        <h3 className="text-sm font-semibold text-content-secondary">Roles</h3>
        <div className="mt-2 flex flex-wrap gap-2">
          {roles.length > 0 ? (
            roles.map((role) => (
              <span
                key={role}
                className="rounded-full border border-brand-subtle/30 bg-brand/10 px-3 py-1 text-xs text-brand-hover"
              >
                {role}
              </span>
            ))
          ) : (
            <span className="text-sm text-content-muted">Unclassified</span>
          )}
        </div>
      </div>

      <div className="mt-4 border-t border-border pt-4">
        <h3 className="text-sm font-semibold text-content-secondary">
          Spells ({character.characterSpells?.length ?? 0})
        </h3>
        {character.characterSpells?.length ? (
          <ul className="mt-2 flex flex-wrap gap-2">
            {character.characterSpells.map(({ id, spell }) => (
              <li
                key={id}
                className="rounded-md bg-surface-raised px-2.5 py-1 text-xs text-content"
              >
                {spell.name}
              </li>
            ))}
          </ul>
        ) : (
          <p className="mt-2 text-sm text-content-muted">No assigned spells</p>
        )}
      </div>

      <div className="mt-4 flex items-center justify-end gap-2 border-t border-border pt-4">
        <Button
          variant="magic"
          inverted
          size="md"
          leftIcon={<FontAwesomeIcon icon={faWandMagicSparkles} />}
        >
          Spells
        </Button>
        <Button
          onClick={onDelete ? () => onDelete(character) : undefined}
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
          onClick={onSelect ? () => onSelect(character) : undefined}
          leftIcon={<FontAwesomeIcon icon={faPen} />}
        >
          Edit
        </Button>
      </div>
    </Card>
  );
}
