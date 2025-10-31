import { useEffect, useRef, useState } from "react";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "./buttons/AppButton";
import Text, { TextColor, TextSize } from "./typography/Text";
import { CharacterDto } from "../types/character";
import { useToast } from "../providers/ToastProvider";
import { BASE_URL, useApi } from "../hooks/useApi";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowCircleLeft,
  faArrowCircleRight,
  faClose,
} from "@fortawesome/free-solid-svg-icons";
import clsx from "clsx";

interface AlternateFormSelectionProps {
  selectedAlternateFormId: number | null;
  selectedAlternateFormPhotoUrl: string | null;
  onSelect: (alternateFormId: number | null) => void;
}

const AlternateFormSelection = ({
  selectedAlternateFormId,
  selectedAlternateFormPhotoUrl,
  onSelect,
}: AlternateFormSelectionProps) => {
  const [showCharacterSelect, setShowCharacterSelect] = useState(false);
  const [internalAlternatePhotoUrl, setInternalAlternatePhotoUrl] = useState(
    selectedAlternateFormPhotoUrl,
  );

  const hasAlternate = selectedAlternateFormId;
  console.log(`This is the has atlernate: ${selectedAlternateFormId}`);

  return (
    <div className="mt-4 flex flex-col">
      <Text size={TextSize.xl} textColor={TextColor.white}>
        Alternate Form
      </Text>

      <div className="flex gap-2">
        <AppButton
          title={hasAlternate ? "Replace Alternate" : "Add Alternate"}
          type="button"
          variant={
            hasAlternate ? AppButtonVariant.warning : AppButtonVariant.primary
          }
          size={AppButtonSize.sm}
          onClick={() => setShowCharacterSelect(true)}
          className="self-start"
        />

        {internalAlternatePhotoUrl && (
          <div className="relative">
            <img
              src={internalAlternatePhotoUrl}
              className="h-[85px] rounded-xl"
            />
            <button
              type="button"
              className="absolute -top-3 -right-1"
              onClick={() => {
                onSelect(null);
                setInternalAlternatePhotoUrl(null);
              }}
            >
              <FontAwesomeIcon
                icon={faClose}
                className="text-red-500 text-2xl"
              />
            </button>
          </div>
        )}
      </div>

      <CharacterSelect
        isOpen={showCharacterSelect}
        onSelect={(characterId, characterPhotoUrl) => {
          setInternalAlternatePhotoUrl(characterPhotoUrl);
          onSelect(characterId);
        }}
      />
    </div>
  );
};

interface CharacterSelectProps {
  isOpen: boolean;
  selectedCharacterId?: number;
  onSelect: (characterId: number, characterPhotoUrl: string) => void;
}

const CharacterSelect = ({
  isOpen,
  selectedCharacterId,
  onSelect,
}: CharacterSelectProps) => {
  const { showError } = useToast();
  const { get } = useApi();
  const [characters, setCharacters] = useState<CharacterDto[]>([]);
  const scrollRef = useRef<HTMLDivElement>(null);

  const scroll = (direction: "left" | "right") => {
    if (!scrollRef.current) return;

    const scrollAmount = 300;
    scrollRef.current.scrollBy({
      left: direction === "left" ? -scrollAmount : scrollAmount,
      behavior: "smooth",
    });
  };

  const getCharacters = async () => {
    try {
      const characters = await get(`${BASE_URL}/Character`);
      setCharacters(characters);
    } catch (err) {
      showError("Unable to get characters.");
      console.error(err);
    }
  };

  useEffect(() => {
    getCharacters();
  }, []);

  if (!isOpen) return null;

  return (
    <div className="relative w-full">
      <button
        type="button"
        onClick={() => scroll("left")}
        className="absolute left-0 top-1/2 -translate-y-1/2 z-10 bg-black/50 hover:bg-black/70 text-white px-2 py-1 rounded-full"
      >
        <FontAwesomeIcon icon={faArrowCircleLeft} />
      </button>

      <div
        ref={scrollRef}
        className="flex w-full overflow-x-auto scrollbar-hide items-end pb-1 my-2 scroll-smooth"
      >
        {characters.map((c) => (
          <div
            key={c.id}
            className={clsx(
              "flex flex-col flex-shrink-0 mr-4 border rounded-md justify-center items-center w-[110px] p-2 overflow-hidden",
              {
                "border-gray-700": selectedCharacterId !== c.id,
                "border-blue-400": selectedCharacterId === c.id,
              },
            )}
          >
            <img
              src={c.photoUrl}
              alt={c.name}
              className="h-[100px] object-cover rounded-lg"
            />
            <Text
              textColor={TextColor.white}
              className="p-2 text-center break-all"
            >
              {c.name}
            </Text>
            <AppButton
              title={"Select"}
              variant={AppButtonVariant.primary}
              size={AppButtonSize.xs}
              type="button"
              onClick={() => onSelect(c.id, c.photoUrl)}
            />
          </div>
        ))}
      </div>

      <button
        onClick={() => scroll("right")}
        type="button"
        className="absolute right-0 top-1/2 -translate-y-1/2 z-10 bg-black/50 hover:bg-black/70 text-white px-2 py-1 rounded-full"
      >
        <FontAwesomeIcon icon={faArrowCircleRight} />
      </button>
    </div>
  );
};

export default AlternateFormSelection;
