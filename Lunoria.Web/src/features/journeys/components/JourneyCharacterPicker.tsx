import { useEffect, useState } from "react";
import { ApiLoadError, Button } from "@/components/ui";
import {
  CharacterType,
  listCharacters,
  type Character,
} from "@/features/characters";
import { getApiError } from "@/lib/apiClient";

interface JourneyCharacterPickerProps {
  selectedCharacterIds: number[];
  onSave: (characterIds: number[]) => Promise<void>;
  onCancel: () => void;
}

export function JourneyCharacterPicker({
  selectedCharacterIds,
  onSave,
  onCancel,
}: JourneyCharacterPickerProps) {
  const [characters, setCharacters] = useState<Character[]>([]);
  const [selectedIds, setSelectedIds] = useState(
    () => new Set(selectedCharacterIds),
  );
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState("");

  const load = async () => {
    setIsLoading(true);
    setError("");

    try {
      setCharacters(await listCharacters({ typeFilter: CharacterType.Player }));
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    let isCurrent = true;

    void listCharacters({ typeFilter: CharacterType.Player })
      .then((loadedCharacters) => {
        if (isCurrent) {
          setCharacters(loadedCharacters);
          setError("");
        }
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

  const toggle = (characterId: number) => {
    setSelectedIds((current) => {
      const next = new Set(current);
      if (next.has(characterId)) next.delete(characterId);
      else next.add(characterId);
      return next;
    });
  };

  const save = async () => {
    setIsSaving(true);
    setError("");

    try {
      await onSave(Array.from(selectedIds));
    } catch (requestError) {
      setError(getApiError(requestError).message);
      setIsSaving(false);
    }
  };

  return (
    <div className="flex min-h-full flex-col">
      <p className="mb-5 text-sm text-content-secondary">
        Active characters will be available when a new playthrough starts.
      </p>

      {isLoading && (
        <p className="text-content-secondary" role="status">
          Loading playable characters...
        </p>
      )}

      {!isLoading && error && <ApiLoadError error={error} onRetry={load} />}

      {!isLoading && !error && characters.length === 0 && (
        <p className="rounded-xl border border-border p-5 text-content-muted">
          No playable characters are available. Create a character with the
          Player type first.
        </p>
      )}

      {!isLoading && !error && characters.length > 0 && (
        <div className="flex-1 space-y-3">
          {characters.map((character) => {
            const isSelected = selectedIds.has(character.id);

            return (
              <button
                key={character.id}
                type="button"
                aria-pressed={isSelected}
                onClick={() => toggle(character.id)}
                disabled={isSaving}
                className={`flex w-full cursor-pointer items-center gap-4 rounded-xl border p-4 text-left transition disabled:cursor-not-allowed disabled:opacity-60 ${
                  isSelected
                    ? "border-add bg-add/10"
                    : "border-border bg-surface hover:border-content-muted"
                }`}
              >
                {character.photoUrl && (
                  <img
                    src={character.photoUrl}
                    alt=""
                    className="size-16 shrink-0 rounded-lg object-cover"
                  />
                )}
                <span className="min-w-0 flex-1">
                  <span className="block truncate text-lg font-semibold text-content">
                    {character.name}
                  </span>
                  <span className="mt-1 line-clamp-2 block text-sm text-content-secondary">
                    {character.description}
                  </span>
                </span>
                <span
                  className={`shrink-0 rounded-full px-3 py-1 text-xs font-semibold ${
                    isSelected
                      ? "bg-add text-on-add"
                      : "bg-surface-raised text-content-muted"
                  }`}
                >
                  {isSelected ? "Active" : "Inactive"}
                </span>
              </button>
            );
          })}
        </div>
      )}

      <div className="mt-6 flex justify-end gap-3 border-t border-border pt-4">
        <Button onClick={onCancel} disabled={isSaving} size="lg">
          Cancel
        </Button>
        <Button
          onClick={() => void save()}
          disabled={isLoading || isSaving}
          variant="primary"
          size="lg"
        >
          {isSaving ? "Saving..." : "Save Characters"}
        </Button>
      </div>
    </div>
  );
}
