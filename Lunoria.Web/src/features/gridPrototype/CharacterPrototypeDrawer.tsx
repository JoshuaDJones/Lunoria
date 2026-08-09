import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import type { GridPrototypeCharacter } from "@/features/gridPrototype/types";

interface CharacterPrototypeDrawerProps {
  characters: GridPrototypeCharacter[];
  loading: boolean;
  onAdd: (character: GridPrototypeCharacter) => void;
  onClose: () => void;
}

export function CharacterPrototypeDrawer({
  characters,
  loading,
  onAdd,
  onClose,
}: CharacterPrototypeDrawerProps) {
  return (
    <Drawer title="Add characters" onClose={onClose}>
      {loading && <p className="text-content-muted">Loading characters…</p>}
      {!loading && characters.length === 0 && (
        <p className="text-content-muted">No characters are available.</p>
      )}
      <div className="grid gap-3">
        {characters.map((character) => (
          <article
            key={character.id}
            className="flex items-center gap-4 rounded-xl border border-border bg-surface p-3"
          >
            <div className="flex size-16 shrink-0 items-center justify-center overflow-hidden rounded-lg bg-canvas">
              {character.imageUrl ? (
                <img
                  src={character.imageUrl}
                  alt=""
                  className="size-full object-contain"
                />
              ) : (
                <span className="text-xs text-content-muted">No image</span>
              )}
            </div>
            <div className="min-w-0 flex-1">
              <h3 className="truncate font-semibold text-content">
                {character.name}
              </h3>
              <p className="text-xs text-content-muted">
                Add as many copies as needed.
              </p>
            </div>
            <Button variant="add" size="sm" onClick={() => onAdd(character)}>
              Add
            </Button>
          </article>
        ))}
      </div>
    </Drawer>
  );
}
