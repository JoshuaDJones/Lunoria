import Text, { TextColor, TextSize } from "../typography/Text";

export const ToCharacterType = (
  isPlayer: boolean,
  isNPC: boolean,
  isEnemy: boolean,
): CharacterType => {
  if (isPlayer === true) return CharacterType.player;

  if (isNPC === true) return CharacterType.npc;

  if (isEnemy === true) return CharacterType.enemy;

  return CharacterType.npc;
};

export enum CharacterType {
  player,
  npc,
  enemy,
}

interface CharacterTypeSelectProps {
  type: CharacterType;
  onSelect: (type: CharacterType) => void;
}

const CharacterTypeSelect = ({ type, onSelect }: CharacterTypeSelectProps) => {
  return (
    <div className="flex flex-col bg-gray-700 rounded-lg p-3 mt-6">
      <Text size={TextSize.xl} textColor={TextColor.white}>
        Character Type
      </Text>
      <div className="flex gap-2 ml-3 mt-4">
        <Text size={TextSize.lg} textColor={TextColor.white}>
          Is Player:{" "}
        </Text>
        <input
          type="radio"
          checked={type === CharacterType.player}
          className="h-5 w-5"
          onChange={() => onSelect(CharacterType.player)}
        />
      </div>
      <div className="flex gap-2 ml-3">
        <Text size={TextSize.lg} textColor={TextColor.white}>
          Is NPC:{" "}
        </Text>
        <input
          type="radio"
          checked={type === CharacterType.npc}
          className="h-5 w-5"
          onChange={() => onSelect(CharacterType.npc)}
        />
      </div>
      <div className="flex gap-2 ml-3">
        <Text size={TextSize.lg} textColor={TextColor.white}>
          Is Enemy:{" "}
        </Text>
        <input
          type="radio"
          checked={type === CharacterType.enemy}
          className="h-5 w-5"
          onChange={() => onSelect(CharacterType.enemy)}
        />
      </div>
    </div>
  );
};

export default CharacterTypeSelect;
