import clsx from "clsx";

export enum CharacterSelection {
  players,
  npcs,
  enemies,
}

export interface CharacterTabsProps {
  selection: CharacterSelection;
  onSelection: (selection: CharacterSelection) => void;
}

export const CharacterTabs = ({
  selection,
  onSelection,
}: CharacterTabsProps) => {
  return (
    <div className="flex items-center justify-center">
      <div className="flex gap-2 p-2 rounded-xl bg-stone-800/75">
        <CharacterTabButton
          title={"Players"}
          color={TabButtonColor.blue}
          isSelected={selection === CharacterSelection.players}
          onClick={() => onSelection(CharacterSelection.players)}
        />
        <CharacterTabButton
          title={"NPCs"}
          color={TabButtonColor.green}
          isSelected={selection === CharacterSelection.npcs}
          onClick={() => onSelection(CharacterSelection.npcs)}
        />
        <CharacterTabButton
          title={"Enemies"}
          color={TabButtonColor.red}
          isSelected={selection === CharacterSelection.enemies}
          onClick={() => onSelection(CharacterSelection.enemies)}
        />
      </div>
    </div>
  );
};

export enum TabButtonColor {
  green,
  blue,
  red,
}

export interface CharacterTabButtonProps {
  title: string;
  color: TabButtonColor;
  isSelected: boolean;
  onClick: () => void;
}

export const CharacterTabButton = ({
  title,
  color,
  isSelected,
  onClick,
}: CharacterTabButtonProps) => {
  return (
    <button
      onClick={onClick}
      className={clsx(
        "text-white font-cinzel py-3 text-2xl rounded-lg w-[150px] tracking-wide hover:outline hover:outline-white",
        {
          "bg-blue-700": color === TabButtonColor.blue && isSelected,
          "bg-blue-700/25": color === TabButtonColor.blue && !isSelected,
          "bg-green-700": color === TabButtonColor.green && isSelected,
          "bg-green-700/25": color === TabButtonColor.green && !isSelected,
          "bg-red-700": color === TabButtonColor.red && isSelected,
          "bg-red-700/25": color === TabButtonColor.red && !isSelected,
          "font-bold": isSelected,
        },
      )}
    >
      {title}
    </button>
  );
};
