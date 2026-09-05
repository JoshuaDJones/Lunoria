import { useState } from "react";
import { useModalStack } from "@/app/providers";
import { Button } from "@/components/ui";
import { CharacterPicker } from "@/features/characters/components/CharacterPicker";
import type { Character } from "@/features/characters/types";

interface CharacterSelectionFieldProps {
  id: string;
  selectedId: number | null;
  initialSelectedCharacter?: Character | null;
  pickerTitle?: string;
  emptyMessage?: string;
  disabled?: boolean;
  disabledMessage?: string;
  loadCharacters: () => Promise<Character[]>;
  onChange: (characterId: number | null) => void;
}

export function CharacterSelectionField({
  id,
  selectedId,
  initialSelectedCharacter,
  pickerTitle = "Choose character",
  emptyMessage,
  disabled = false,
  disabledMessage,
  loadCharacters,
  onChange,
}: CharacterSelectionFieldProps) {
  const modalStack = useModalStack();
  const [selectedCharacter, setSelectedCharacter] = useState<Character | null>(
    initialSelectedCharacter ?? null,
  );

  const resolvedSelectedCharacter = [
    selectedCharacter,
    initialSelectedCharacter,
  ].find((character) => character?.id === selectedId);

  const openPicker = () => {
    modalStack.push({
      title: pickerTitle,
      placement: "center",
      content: (
        <CharacterPicker
          loadCharacters={loadCharacters}
          selectedId={selectedId}
          emptyMessage={emptyMessage}
          onSelect={(character) => {
            setSelectedCharacter(character);
            onChange(character.id);
            modalStack.pop();
          }}
        />
      ),
    });
  };

  return (
    <div
      id={id}
      role="group"
      aria-labelledby={`${id}-label`}
      className="rounded-xl border border-border bg-surface p-4"
    >
      {resolvedSelectedCharacter ? (
        <div className="flex items-center gap-4">
          {resolvedSelectedCharacter.photoUrl && (
            <img
              src={resolvedSelectedCharacter.photoUrl}
              alt=""
              className="size-20 shrink-0 rounded-lg object-cover"
            />
          )}
          <div className="min-w-0 flex-1">
            <p className="truncate text-lg font-semibold text-content">
              {resolvedSelectedCharacter.name}
            </p>
            <p className="line-clamp-2 text-sm text-content-secondary">
              {resolvedSelectedCharacter.description}
            </p>
          </div>
        </div>
      ) : (
        <p className="text-sm text-content-muted">No character selected.</p>
      )}

      {disabled && disabledMessage && (
        <p className="mt-3 text-sm text-content-muted">{disabledMessage}</p>
      )}

      <div className="mt-4 flex flex-wrap gap-3">
        <Button
          variant="primary"
          inverted
          disabled={disabled}
          onClick={openPicker}
        >
          {resolvedSelectedCharacter ? "Change character" : "Choose character"}
        </Button>
        {selectedId !== null && (
          <Button
            variant="danger"
            disabled={disabled}
            onClick={() => {
              setSelectedCharacter(null);
              onChange(null);
            }}
          >
            Clear selection
          </Button>
        )}
      </div>
    </div>
  );
}
