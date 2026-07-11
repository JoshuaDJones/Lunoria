import { useEffect, useState } from "react";
import { Button, Card } from "@/components/ui";
import { Stat, StatGrid } from "@/components/ui/StatGrid";
import { replaceCharacterSpells } from "@/features/characters/api/charactersApi";
import { listSpells, type Spell } from "@/features/spells";
import { getApiError } from "@/lib/apiClient";

interface CharacterSpellDrawerProps {
  characterId: number;
  selectedIds: number[];
  onChanged: () => void;
}

export function CharacterSpellDrawer({
  characterId,
  selectedIds,
  onChanged,
}: CharacterSpellDrawerProps) {
  const [spells, setSpells] = useState<Spell[]>([]);
  const [selection, setSelection] = useState(() => new Set(selectedIds));
  const [isLoading, setIsLoading] = useState(true);
  const [updatingId, setUpdatingId] = useState<number>();
  const [error, setError] = useState("");

  useEffect(() => {
    let isCurrent = true;

    void listSpells()
      .then((loadedSpells) => {
        if (isCurrent) setSpells(loadedSpells);
      })
      .catch((requestError: unknown) => {
        if (isCurrent) setError(getApiError(requestError).message);
      })
      .finally(() => {
        if (isCurrent) setIsLoading(false);
      });

    return () => {
      isCurrent = false;
    };
  }, []);

  const toggleSpell = async (spellId: number) => {
    const next = new Set(selection);

    if (next.has(spellId)) next.delete(spellId);
    else next.add(spellId);

    setError("");
    setUpdatingId(spellId);

    try {
      await replaceCharacterSpells(characterId, Array.from(next));
      setSelection(next);
      onChanged();
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setUpdatingId(undefined);
    }
  };

  if (isLoading) {
    return <p className="text-content-secondary">Loading spells...</p>;
  }

  return (
    <div className="space-y-4">
      {error && (
        <p className="text-sm text-danger" role="alert">
          {error}
        </p>
      )}

      {spells.length === 0 ? (
        <p className="text-content-muted">No spells are available.</p>
      ) : (
        spells.map((spell) => {
          const isAttached = selection.has(spell.id);

          return (
            <Card key={spell.id} className="overflow-hidden">
              {spell.photoUrl && (
                <img
                  src={spell.photoUrl}
                  alt=""
                  className="max-h-64 w-full bg-surface object-contain"
                />
              )}

              <div className="p-4">
                <h3 className="text-xl font-semibold text-content">
                  {spell.name}
                </h3>
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
                  <Stat
                    label="Area effect"
                    value={spell.isRadius ? "Yes" : "No"}
                  />
                  <Stat
                    label="Damage effect"
                    value={spell.damageEffect ?? "None"}
                  />
                  <Stat
                    label="Health effect"
                    value={spell.healthEffect ?? "None"}
                  />
                  <Stat
                    label="Magic effect"
                    value={spell.magicEffect ?? "None"}
                  />
                </StatGrid>

                <Button
                  onClick={() => void toggleSpell(spell.id)}
                  disabled={updatingId !== undefined}
                  variant={isAttached ? "danger" : "primary"}
                  size="md"
                  className="mt-4 w-full"
                >
                  {updatingId === spell.id
                    ? isAttached
                      ? "Removing..."
                      : "Attaching..."
                    : isAttached
                      ? "Remove"
                      : "Attach"}
                </Button>
              </div>
            </Card>
          );
        })
      )}
    </div>
  );
}
