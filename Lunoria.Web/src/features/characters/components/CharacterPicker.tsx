import { useEffect, useState } from "react";
import { ApiLoadError, Button, Input } from "@/components/ui";
import type { Character } from "@/features/characters/types";
import { getApiError } from "@/lib/apiClient";

interface CharacterPickerProps {
  loadCharacters: () => Promise<Character[]>;
  selectedId: number | null;
  emptyMessage?: string;
  onSelect: (character: Character) => void;
}

export function CharacterPicker({
  loadCharacters,
  selectedId,
  emptyMessage = "No characters are available.",
  onSelect,
}: CharacterPickerProps) {
  const [characters, setCharacters] = useState<Character[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");
  const [search, setSearch] = useState("");
  const query = search.trim().toLocaleLowerCase();
  const filteredCharacters = characters.filter((character) =>
    character.name.toLocaleLowerCase().includes(query),
  );

  const load = async () => {
    setIsLoading(true);
    setError("");

    try {
      setCharacters(await loadCharacters());
    } catch (requestError) {
      setError(getApiError(requestError).message);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    let isCurrent = true;

    void loadCharacters()
      .then((loadedCharacters) => {
        if (isCurrent) setCharacters(loadedCharacters);
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
  }, [loadCharacters]);

  if (isLoading) {
    return <p className="text-content-secondary">Loading characters...</p>;
  }

  if (error) {
    return <ApiLoadError error={error} onRetry={load} />;
  }

  if (characters.length === 0) {
    return <p className="text-content-muted">{emptyMessage}</p>;
  }

  return (
    <div className="flex min-h-0 flex-1 flex-col gap-4">
      <div className="shrink-0">
        <Input
          type="search"
          aria-label="Search characters"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          placeholder="Search by name..."
        />
      </div>
      <div
        className="min-h-0 flex-1 overflow-y-auto overscroll-contain pr-2"
        role="region"
        aria-label="Characters"
        tabIndex={0}
      >
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          {filteredCharacters.map((character) => {
            const isSelected = character.id === selectedId;

            return (
              <article
                key={character.id}
                className={`flex flex-col overflow-hidden rounded-xl border bg-surface/90 shadow-lg transition ${
                  isSelected ? "border-brand" : "border-border"
                }`}
              >
                {character.photoUrl && (
                  <img
                    src={character.photoUrl}
                    alt=""
                    className="h-40 w-full bg-surface object-contain"
                  />
                )}
                <div className="flex flex-1 flex-col p-4">
                  <h3 className="text-xl font-semibold text-content">
                    {character.name}
                  </h3>
                  <p className="mt-1 line-clamp-3 text-sm text-content-secondary">
                    {character.description}
                  </p>
                  <Button
                    variant={isSelected ? "accent" : "primary"}
                    className="mt-4 w-full"
                    onClick={() => onSelect(character)}
                  >
                    {isSelected ? "Keep selected" : "Select character"}
                  </Button>
                </div>
              </article>
            );
          })}
        </div>
      </div>
      <p
        role="status"
        className="shrink-0 border-t border-border pt-3 text-sm text-content-muted"
      >
        {filteredCharacters.length === 0
          ? "No characters match your search."
          : `${filteredCharacters.length} of ${characters.length} characters`}
      </p>
    </div>
  );
}
