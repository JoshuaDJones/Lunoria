import { useState } from "react";
import { CharacterDto, CharacterSpellDto } from "../../types/character";
import BoolListItemRow from "./BoolListItemRow";
import ListItemRow, { RowTextSize } from "./ListItemRow";
import Text, { TextColor, TextSize } from "../typography/Text";
import Title, { TitleColor, TitleSize } from "../typography/Title";
import AppButton, {
  AppButtonSize,
  AppButtonVariant,
} from "../buttons/AppButton";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faCaretSquareDown,
  faCaretSquareUp,
} from "@fortawesome/free-solid-svg-icons";

interface JourneyCharacterListProps {
  selectedCharacterIds: number[];
  characters: CharacterDto[];
  onSelectChange: (selectedCharacterIds: number[]) => void;
}

const JourneyCharacterList = ({
  selectedCharacterIds,
  characters,
  onSelectChange,
}: JourneyCharacterListProps) => {
  return (
    <div className="flex-1">
      {characters.map((c) => (
        <JourneyCharacterListItem
          key={c.id}
          selectedCharacterIds={selectedCharacterIds}
          character={c}
          onSelectChange={onSelectChange}
        />
      ))}
    </div>
  );
};

interface JourneyCharacterListItemProps {
  selectedCharacterIds: number[];
  character: CharacterDto;
  onSelectChange: (selectedCharacterIds: number[]) => void;
}

const JourneyCharacterListItem = ({
  selectedCharacterIds,
  character,
  onSelectChange,
}: JourneyCharacterListItemProps) => {
  const isSelected =
    selectedCharacterIds.find((id) => id === character.id) ?? false;

  const addPlayer = () => {
    if (!isSelected) {
      onSelectChange([...selectedCharacterIds, character.id]);
    }
  };

  const removePlayer = () => {
    const updated = selectedCharacterIds.filter((id) => id !== character.id);
    onSelectChange(updated);
  };

  return (
    <div className="rounded-xl border-2 mb-4 p-3 flex flex-col bg-stone-800 w-full">
      <div className="flex flex-1 gap-2 items-start">
        <div className="w-[60%] flex-col">
          <Title
            className="flex-1 break-words"
            size={TitleSize.medium}
            color={TitleColor.white}
          >
            {character.name}
          </Title>
          <Text
            size={TextSize.xl}
            textColor={TextColor.custom}
            className="text-gray-400 break-words line-clamp-3 mb-2"
          >
            {character.description}
          </Text>
          <ListItemRow
            title={"Max Hp"}
            value={character.maxHp.toString()}
            titleColor={TextColor.red}
            valueColor={TextColor.red}
          />
          <ListItemRow
            title={"Max Mp"}
            value={character.maxMp.toString()}
            className="mb-2"
            titleColor={TextColor.green}
            valueColor={TextColor.green}
          />
          <ListItemRow
            title={"Melee Attack Damage"}
            value={character.meleeAttackDamage?.toString() ?? "N/A"}
          />
          <ListItemRow
            title={"Bow Attack Damage"}
            value={character.bowAttackDamage?.toString() ?? "N/A"}
          />
          <ListItemRow
            title={"Movement"}
            value={character.movement?.toString()}
          />
          <ListItemRow
            title={"Max Inventory"}
            value={character.maxInventory?.toString()}
          />
        </div>
        <img
          src={character.photoUrl}
          className="w-[40%] object-cover rounded-md"
        />
      </div>
      <JourneyCharacterSpellsItem spells={character.characterSpells} />

      {isSelected ? (
        <AppButton
          className="self-center mt-2"
          title={"Remove"}
          variant={AppButtonVariant.warning}
          size={AppButtonSize.md}
          onClick={removePlayer}
        />
      ) : (
        <AppButton
          className="self-center mt-2"
          title={"Add"}
          variant={AppButtonVariant.go}
          size={AppButtonSize.md}
          onClick={addPlayer}
        />
      )}
    </div>
  );
};

interface JourneyCharacterSpellItemsProps {
  spells: CharacterSpellDto[] | null;
}

export const JourneyCharacterSpellsItem = ({
  spells,
}: JourneyCharacterSpellItemsProps) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div
      className="flex flex-col mt-2 gap-2 items-center active:opacity-60 cursor-pointer"
      onClick={() => setIsOpen(!isOpen)}
    >
      <div className="flex w-full gap-2">
        <FontAwesomeIcon
          icon={isOpen ? faCaretSquareUp : faCaretSquareDown}
          color="white"
          size="xl"
        />
        <Text textColor={TextColor.white} className="font-bold tracking-widest">
          Spells
        </Text>
      </div>

      {isOpen && (
        <div>
          {spells?.map((s) => (
            <div className="flex flex-1 gap-2 items-start rounded-lg mt-2 bg-gray-600 p-2">
              <div className="flex-col flex-1">
                <Text
                  size={TextSize.lg}
                  textColor={TextColor.white}
                  className="break-words line-clamp-3 mb-2 font-bold"
                >
                  {s.spell.name}
                </Text>
                <Text
                  size={TextSize.sm}
                  textColor={TextColor.custom}
                  className="text-gray-400 break-words line-clamp-3 mb-2"
                >
                  {s.spell.description}
                </Text>
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Cost"}
                  value={s.spell.mpCost?.toString() ?? "N/A"}
                />
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Range"}
                  value={s.spell.range.toString()}
                />
                <BoolListItemRow
                  title={"Is Radius"}
                  isSelected={s.spell.isRadius}
                />
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Damage Effect"}
                  value={s.spell.damageEffect?.toString() ?? "N/A"}
                />
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Health Effect"}
                  value={s.spell.healthEffect?.toString() ?? "N/A"}
                />
                <ListItemRow
                  size={RowTextSize.sm}
                  title={"Magic Effect"}
                  value={s.spell.magicEffect?.toString() ?? "N/A"}
                />
              </div>
              <img
                src={s.spell.photoUrl}
                className="w-[80px] object-cover rounded-md"
              />
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default JourneyCharacterList;
