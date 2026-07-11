import { useEffect, useState } from "react";
import { listSpells, type Spell } from "@/features/spells";
import { getApiError } from "@/lib/apiClient";
import { Button, Card } from "@/components/ui";
import { Stat, StatGrid } from "@/components/ui/StatGrid";

interface SpellPickerDialogProps {
  selectedIds: number[];
  onApply: (selectedIds: number[]) => void;
  onClose: () => void;
}

export function SpellPickerDialog({
  selectedIds,
  onApply,
  onClose,
}: SpellPickerDialogProps) {
  const [spells, setSpells] = useState<Spell[]>([]);
  const [selection, setSelection] = useState(() => new Set(selectedIds));
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let isCurrent = true;

    void listSpells()
      .then((loadedSpells) => {
        if (isCurrent) {
          setSpells(loadedSpells);
        }
      })
      .catch((requestError: unknown) => {
        if (isCurrent) {
          setError(getApiError(requestError).message);
        }
      })
      .finally(() => {
        if (isCurrent) {
          setIsLoading(false);
        }
      });

    return () => {
      isCurrent = false;
    };
  }, []);

  const toggleSpell = (spellId: number) => {
    setSelection((current) => {
      const next = new Set(current);

      if (next.has(spellId)) {
        next.delete(spellId);
      } else {
        next.add(spellId);
      }

      return next;
    });
  };

  return (
    <div className="flex min-h-0 flex-col">
      <p className="mb-4 text-sm text-content-muted">
        {selection.size} selected
      </p>

      <div className="min-h-0 flex-1 overflow-y-auto">
        {isLoading && (
          <p className="text-content-secondary" role="status">
            Loading spells...
          </p>
        )}

        {error && (
          <p className="text-danger" role="alert">
            {error}
          </p>
        )}

        {!isLoading && !error && spells.length === 0 && (
          <p className="text-content-muted">No spells are available.</p>
        )}

        {!isLoading && !error && spells.length > 0 && (
          <div className="space-y-4">
            {spells.map((spell) => {
              const isAttached = selection.has(spell.id);

              return (
                <Card key={spell.id} className="overflow-hidden">
                  {(spell.photoUrl || spell.spellType?.photoUrl) && (
                    <img
                      src={spell.photoUrl || spell.spellType?.photoUrl}
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
                      onClick={() => toggleSpell(spell.id)}
                      variant={isAttached ? "danger" : "primary"}
                      size="md"
                      className="mt-4 w-full"
                    >
                      {isAttached ? "Remove" : "Attach"}
                    </Button>
                  </div>
                </Card>
              );
            })}
          </div>
        )}
      </div>

      <footer className="mt-6 flex justify-end gap-3 border-t border-border pt-4">
        <Button onClick={onClose} size="lg" className="py-2.5">
          Cancel
        </Button>
        <Button
          onClick={() => onApply(Array.from(selection))}
          variant="primary"
          size="lg"
          className="py-2.5"
        >
          Apply selection
        </Button>
      </footer>
    </div>
  );
}
