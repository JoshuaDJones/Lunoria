import { useEffect, useState } from "react";
import { listSpells, type Spell } from "@/features/spells";
import { getApiError } from "@/lib/apiClient";

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

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        onClose();
      }
    };

    document.addEventListener("keydown", handleKeyDown);

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
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [onClose]);

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
    <div
      data-nested-dialog="true"
      className="fixed inset-0 z-60 flex items-center justify-center bg-canvas/80 p-4 backdrop-blur-xs"
      onMouseDown={(event) => {
        if (event.target === event.currentTarget) {
          onClose();
        }
      }}
    >
      <section
        role="dialog"
        aria-modal="true"
        aria-labelledby="spell-picker-title"
        className="flex max-h-[85vh] w-full max-w-2xl flex-col rounded-2xl border border-border bg-surface-raised shadow-2xl"
      >
        <header className="flex items-center justify-between border-b border-border px-6 py-5">
          <div>
            <h2
              id="spell-picker-title"
              className="text-2xl font-semibold text-content"
            >
              Select spells
            </h2>
            <p className="mt-1 text-sm text-content-muted">
              {selection.size} selected
            </p>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="rounded-lg border border-border px-3 py-2 text-content-secondary hover:border-brand-hover hover:text-brand-hover"
          >
            Close
          </button>
        </header>

        <div className="flex-1 overflow-y-auto p-6">
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
                  {spell.photoUrl && (
                    <img
                      src={spell.photoUrl}
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

        <footer className="flex justify-end gap-3 border-t border-border px-6 py-4">
          <button
            type="button"
            onClick={onClose}
            className="rounded-lg border border-border px-5 py-2.5 text-content-secondary transition hover:border-brand-hover"
          >
            Cancel
          </button>
          <button
            type="button"
            onClick={() => onApply(Array.from(selection))}
            className="rounded-lg bg-brand px-5 py-2.5 font-semibold text-on-brand transition hover:bg-brand-hover"
          >
            Apply selection
          </button>
        </footer>
      </section>
    </div>
  );
}
