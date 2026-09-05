import { useEffect, useRef } from "react";
import { listAlternateCharacters } from "@/features/characters/api/charactersApi";
import { CharacterSelectionField } from "@/features/characters/components/CharacterSelectionField";
import type {
  Character,
  CharacterType,
} from "@/features/characters/types";

interface AlternateCharacterFieldProps {
  id: string;
  characterType: CharacterType;
  excludeCharacterId?: number;
  selectedId: number | null;
  initialSelectedCharacter?: Character | null;
  onChange: (characterId: number | null) => void;
}

export function AlternateCharacterField({
  id,
  characterType,
  excludeCharacterId,
  selectedId,
  initialSelectedCharacter,
  onChange,
}: AlternateCharacterFieldProps) {
  const previousCharacterTypeRef = useRef(characterType);

  useEffect(() => {
    if (previousCharacterTypeRef.current === characterType) return;

    previousCharacterTypeRef.current = characterType;
    onChange(null);
  }, [characterType, onChange]);

  return (
    <CharacterSelectionField
      id={id}
      selectedId={selectedId}
      initialSelectedCharacter={initialSelectedCharacter}
      pickerTitle="Choose alternate character"
      emptyMessage="No compatible alternate characters are available."
      loadCharacters={() =>
        listAlternateCharacters({ characterType, excludeCharacterId })
      }
      onChange={onChange}
    />
  );
}
