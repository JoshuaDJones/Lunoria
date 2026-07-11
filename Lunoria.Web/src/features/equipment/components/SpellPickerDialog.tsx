import { useEffect, useState } from "react";
import { listSpells, type Spell } from "@/features/spells";
import { getApiError } from "@/lib/apiClient";
import { Button } from "@/components/ui";

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
          <div className="grid gap-3 sm:grid-cols-2">
            {spells.map((spell) => (
              <label
                key={spell.id}
                className="flex cursor-pointer items-start gap-3 rounded-xl border border-border bg-surface p-3 transition hover:border-brand-subtle/60"
              >
                <input
                  type="checkbox"
                  checked={selection.has(spell.id)}
                  onChange={() => toggleSpell(spell.id)}
                  className="mt-1 size-4 shrink-0 accent-brand"
                />
                {(spell.photoUrl || spell.spellType?.photoUrl) && (
                  <img
                    src={spell.photoUrl || spell.spellType?.photoUrl}
                    alt=""
                    className="h-12 w-12 shrink-0 rounded-md object-cover"
                  />
                )}
                <span className="min-w-0">
                  <span className="block font-semibold text-content">
                    {spell.name}
                  </span>
                  <span className="mt-1 line-clamp-2 block text-xs text-content-muted">
                    {spell.description}
                  </span>
                </span>
              </label>
            ))}
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
