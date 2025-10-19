import { useEffect, useState } from "react";
import clsx from "clsx";
import { BASE_URL, useApi } from "../../hooks/useApi";
import { CharacterDto } from "../../types/character";
import { ToastType, useToast } from "../../providers/ToastProvider";
import EnemyNPCActivationCard from "../cards/EnemyNPCActivationCard";

enum CharacterSelectionType {
  enemies,
  npcs,
}

interface EnemyNPCActivationListProps {
  sceneId: number;
  onRefreshRequest: () => void;
}

const EnemyNPCActivationList = ({
  sceneId,
  onRefreshRequest,
}: EnemyNPCActivationListProps) => {
  const { get } = useApi();
  const { showToast } = useToast();

  const [characterSelection, setCharacterSelection] = useState(
    CharacterSelectionType.enemies,
  );
  const [availableCharacters, setAvailableCharacters] = useState<
    CharacterDto[]
  >([]);

  const getCharacters = async () => {
    try {
      const characters: CharacterDto[] = await get(`${BASE_URL}/Character`, {
        typeFilter:
          characterSelection === CharacterSelectionType.enemies
            ? "Enemy"
            : "NPC",
      });

      setAvailableCharacters(characters);
    } catch (err) {
      showToast(
        "Error",
        "Unable to get activation characters.",
        ToastType.error,
        3000,
      );
      console.error(err);
    }
  };

  useEffect(() => {
    getCharacters();
  }, [characterSelection]);

  return (
    <div className="flex-1 flex flex-col gap-2">
      <div className="flex flex-1 flex-col gap-2 overflow-y-auto scrollbar-hide p-5">
        {availableCharacters.map((c) => (
          <EnemyNPCActivationCard
            sceneId={sceneId}
            character={c}
            onRefreshRequest={onRefreshRequest}
          />
        ))}
      </div>
      <div className="flex gap-2 p-2 rounded-xl bg-stone-700/75 mt-2 self-center">
        <CharacterTypeTabButton
          type="enemy"
          isSelected={characterSelection === CharacterSelectionType.enemies}
          title={"Enemies"}
          onClick={() => setCharacterSelection(CharacterSelectionType.enemies)}
        />
        <CharacterTypeTabButton
          type="npc"
          isSelected={characterSelection === CharacterSelectionType.npcs}
          title={"NPCs"}
          onClick={() => setCharacterSelection(CharacterSelectionType.npcs)}
        />
      </div>
    </div>
  );
};

interface CharacterTypeTabButtonProps {
  title: string;
  isSelected: boolean;
  type: "enemy" | "npc";
  onClick: () => void;
}

const CharacterTypeTabButton = ({
  title,
  isSelected,
  type,
  onClick,
}: CharacterTypeTabButtonProps) => {
  return (
    <button
      className={clsx(
        "text-white font-cinzel py-2 text-2xl rounded-lg w-[150px] tracking-wide hover:outline hover:outline-white font-bold",
        {
          "bg-red-900": isSelected && type === "enemy",
          "bg-green-900": isSelected && type === "npc",
        },
      )}
      onClick={onClick}
    >
      {title}
    </button>
  );
};

export default EnemyNPCActivationList;
