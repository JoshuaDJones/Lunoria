import { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faSpinner,
  faXmark,
} from "@fortawesome/free-solid-svg-icons";
import { ApiLoadError, Button, Input } from "@/components/ui";
import type { Character } from "@/features/characters";

interface Props {
  characters: Character[];
  loading: boolean;
  error: string;
  busy: boolean;
  onRetry: () => void;
  onDirtyChange: (dirty: boolean) => void;
  onBack: () => void;
  onAdd: (ids: number[]) => Promise<void>;
}

export function JourneyPlayerSelection({
  characters,
  loading,
  error,
  busy,
  onRetry,
  onDirtyChange,
  onBack,
  onAdd,
}: Props) {
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<number[]>([]);
  useEffect(() => {
    onDirtyChange(selected.length > 0);
    return () => onDirtyChange(false);
  }, [selected, onDirtyChange]);
  const filtered = characters.filter((character) =>
    `${character.name} ${character.description ?? ""}`
      .toLowerCase()
      .includes(search.trim().toLowerCase()),
  );
  return (
    <div className="space-y-4">
      <Button
        disabled={busy}
        size="sm"
        onClick={onBack}
        leftIcon={<FontAwesomeIcon icon={faArrowLeft} />}
      >
        Players
      </Button>
      <h3 className="text-lg font-semibold text-content">Add players</h3>
      <p className="text-sm text-content-secondary">
        Choose players from your library. New players are added at the end of
        the turn order. Alternate-only characters remain available in player
        settings.
      </p>
      <div className="sticky top-0 z-10 bg-surface py-2">
        <div className="relative">
          <Input
            aria-label="Search players"
            placeholder="Search players…"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            className="pr-10"
          />
          {search && (
            <button
              type="button"
              aria-label="Clear search"
              className="absolute top-1/2 right-3 -translate-y-1/2 text-content-muted"
              onClick={() => setSearch("")}
            >
              <FontAwesomeIcon icon={faXmark} />
            </button>
          )}
        </div>
      </div>
      {loading ? (
        <p role="status">
          <FontAwesomeIcon icon={faSpinner} spin /> Loading players
        </p>
      ) : error ? (
        <ApiLoadError error={error} onRetry={onRetry} />
      ) : (
        <div className="space-y-2">
          {filtered.length === 0 && (
            <p className="py-4 text-content-muted">
              {search
                ? "No players match your search."
                : "No more eligible players to add."}
            </p>
          )}
          {filtered.map((character) => (
            <label
              key={character.id}
              className={`flex cursor-pointer items-center gap-3 rounded-xl border p-3 ${selected.includes(character.id) ? "border-add bg-add/10" : "border-border bg-surface"}`}
            >
              <input
                type="checkbox"
                className="size-4"
                disabled={busy}
                checked={selected.includes(character.id)}
                onChange={() =>
                  setSelected((current) =>
                    current.includes(character.id)
                      ? current.filter((id) => id !== character.id)
                      : [...current, character.id],
                  )
                }
              />
              {character.photoUrl && (
                <img
                  src={character.photoUrl}
                  alt=""
                  className="size-12 shrink-0 rounded-lg object-contain"
                />
              )}
              <span className="min-w-0">
                <span className="block font-semibold text-content">
                  {character.name}
                </span>
                <span className="line-clamp-2 text-sm text-content-muted">
                  {character.description}
                </span>
              </span>
            </label>
          ))}
        </div>
      )}
      <div className="sticky bottom-0 flex items-center justify-between gap-3 border-t border-border bg-surface py-4">
        <span className="text-sm text-content-muted" aria-live="polite">
          {selected.length} selected
        </span>
        <Button
          variant="primary"
          disabled={busy || loading || !!error || selected.length === 0}
          onClick={() => void onAdd(selected)}
        >
          {busy && <FontAwesomeIcon icon={faSpinner} spin />} Add selected
          players
        </Button>
      </div>
    </div>
  );
}
